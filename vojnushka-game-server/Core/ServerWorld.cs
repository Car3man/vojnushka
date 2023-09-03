using Arch.Core;
using VojnushkaGameServer.Network;

namespace VojnushkaGameServer.Core;

public class ServerWorld : IServerWorld, IDisposable
{
    private readonly World _world;
    private readonly List<ISystem> _systems;
    private readonly List<ITickSystem> _tickSystems;
    private readonly List<IPeerEventSystem> _peerEventSystems;
    private readonly Dictionary<EntityReference, IPeer> _entityRefToPeerMap;
    private readonly Dictionary<IPeer, EntityReference> _peerToEntityRefMap;
    private readonly Queue<NetPeerMessage> _peerMessageQueue;

    public event ServerWorldPeerRequestDelegate? PeerRequest;

    public ServerWorld(ServerWorldConfiguration configuration)
    {
        _world = World.Create();
        _systems = new List<ISystem>();
        _tickSystems = new List<ITickSystem>();
        _peerEventSystems = new List<IPeerEventSystem>();
        _entityRefToPeerMap = new Dictionary<EntityReference, IPeer>();
        _peerToEntityRefMap = new Dictionary<IPeer, EntityReference>();
        _peerMessageQueue = new Queue<NetPeerMessage>();
        
        ConfigureWorld(configuration);
    }

    public void Start()
    {
        foreach (var system in _systems)
        {
            system.OnStart(_world);
        }
    }
    
    public void Tick(float deltaTime)
    {
        CreatePeerMessages();
        
        foreach (var system in _tickSystems)
        {
            system.OnTick(_world, deltaTime);
        }

        CleanUpPeerMessages();
        SendPeerRequests();
    }

    public void Stop()
    {
        foreach (var system in _systems)
        {
            system.OnStop(_world);
        }
    }

    public void AddPeer(IPeer peer)
    {
        var entity = _world.Create();
        var entityRef = _world.Reference(entity);
        
        _world.Add(entity, new NetPeer
        {
            Id = peer.Id
        });
        
        _entityRefToPeerMap.Add(entityRef, peer);
        _peerToEntityRefMap.Add(peer, entityRef);
    }

    public void AddPeerMessage(IPeer peer, byte[] data)
    {
        var peerEntityRef = _peerToEntityRefMap[peer];
        var peerMessage = new NetPeerMessage
        {
            EntityRef = peerEntityRef,
            Data = data
        };
        _peerMessageQueue.Enqueue(peerMessage);
    }

    public void RemovePeer(IPeer peer)
    {
        var query = new QueryDescription()
            .WithAll<NetPeer>();
        
        _world.Query(in query, (in Entity entity, ref NetPeer netPeer) =>
        {
            if (netPeer.Id == peer.Id)
            {
                _world.Destroy(entity);
            }
        });
        
        _entityRefToPeerMap.Remove(_peerToEntityRefMap[peer]);
        _peerToEntityRefMap.Remove(peer);
    }

    private void CreatePeerMessages()
    {
        while (_peerMessageQueue.Count > 0)
        {
            var message = _peerMessageQueue.Dequeue();
            var entity = _world.Create();
            _world.Add(entity, message);
        }
    }

    private void CleanUpPeerMessages()
    {
        var query = new QueryDescription()
            .WithAll<NetPeerMessage>();
        _world.Destroy(query);
    }

    private void SendPeerRequests()
    {
        var requestQuery = new QueryDescription()
            .WithAll<NetPeerRequest>();
        var peerQuery = new QueryDescription()
            .WithAll<NetPeer>();
        
        _world.Query(in requestQuery, (ref NetPeerRequest netRequest) =>
        {
            var netRequestEntityRef = netRequest.EntityRef;
            var netRequestData = netRequest.Data;
            
            _world.Query(in peerQuery, (in Entity netPeerEntity) =>
            {
                if (netPeerEntity.Id != netRequestEntityRef.Entity.Id)
                {
                    return;
                }

                if (_entityRefToPeerMap.TryGetValue(netRequestEntityRef, out var peer))
                {
                    PeerRequest?.Invoke(peer, netRequestData);
                }
            });
        });
        
        _world.Destroy(requestQuery);
    }

    private void ConfigureWorld(ServerWorldConfiguration configuration)
    {
        foreach (var system in configuration.Systems)
        {
            RegisterSystem(system);
        }
    }
    
    private void RegisterSystem(ISystem system)
    {
        _systems.Add(system);

        if (system is ITickSystem tickSystem)
        {
            _tickSystems.Add(tickSystem);
        }

        if (system is IPeerEventSystem peerEventSystem)
        {
            _peerEventSystems.Add(peerEventSystem);
        }
    }

    public void Dispose()
    {
        _world.Dispose();
    }
}