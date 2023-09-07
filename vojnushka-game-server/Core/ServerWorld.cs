using Arch.Core;
using VojnushkaGameServer.Domain.Avatar;
using VojnushkaGameServer.Domain.PingPong;
using VojnushkaGameServer.Domain.SessionSnapshot;
using VojnushkaGameServer.Logger;
using VojnushkaGameServer.Network;
using VojnushkaProto.Core;

namespace VojnushkaGameServer.Core;

public class ServerWorld : IServerWorld, IDisposable
{
    private readonly World _world = World.Create();
    private readonly List<ISystem> _systems = new();
    private readonly List<ITickSystem> _tickSystems = new();
    private readonly List<IPeerEventSystem> _peerEventSystems = new();
    private readonly Dictionary<EntityReference, IPeer> _entityRefToPeerMap = new();
    private readonly Dictionary<IPeer, EntityReference> _peerToEntityRefMap = new();
    private readonly Queue<NetPeerMessage> _peerMessageQueue = new();

    public event ServerWorldBroadcastRequestDelegate? BroadcastRequest;
    public event ServerWorldPeerRequestDelegate? PeerRequest;

    public ServerWorld(ILogger logger)
    {
        RegisterSystem(new PingPongSystem(logger));
        RegisterSystem(new CreateAvatarSystem());
        RegisterSystem(new AvatarSyncSystem());
        RegisterSystem(new AvatarsSnapshotSystem());
        RegisterSystem(new SessionSnapshotSystem());
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

        ProcessBroadcastRequests();
        ProcessPeerRequests();
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
            Id = peer.Id,
            IdNumber = peer.IdNumber
        });
        
        foreach (var system in _peerEventSystems)
        {
            system.OnPeerConnect(_world, entityRef);
        }
        
        _entityRefToPeerMap.Add(entityRef, peer);
        _peerToEntityRefMap.Add(peer, entityRef);
    }

    public void AddPeerMessage(IPeer peer, ServerProtoMsg message)
    {
        var peerEntityRef = _peerToEntityRefMap[peer];
        var peerMessage = new NetPeerMessage
        {
            EntityRef = peerEntityRef,
            Message = message
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
        
        foreach (var system in _peerEventSystems)
        {
            system.OnPeerDisconnect(_world, _peerToEntityRefMap[peer]);
        }
        
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

    private void ProcessBroadcastRequests()
    {
        var requestQuery = new QueryDescription()
            .WithAll<NetBroadcastRequest>();
        
        _world.Query(in requestQuery, (ref NetBroadcastRequest netBroadcastRequest) =>
        {
            BroadcastRequest?.Invoke(netBroadcastRequest.Message);
        });
        
        _world.Destroy(requestQuery);
    }

    private void ProcessPeerRequests()
    {
        var requestQuery = new QueryDescription()
            .WithAll<NetPeerRequest>();
        var peerQuery = new QueryDescription()
            .WithAll<NetPeer>();
        
        _world.Query(in requestQuery, (ref NetPeerRequest netRequest) =>
        {
            var netRequestEntityRef = netRequest.EntityRef;
            var netRequestMessage = netRequest.Message;
            
            _world.Query(in peerQuery, (in Entity netPeerEntity) =>
            {
                if (netPeerEntity.Id != netRequestEntityRef.Entity.Id)
                {
                    return;
                }

                if (_entityRefToPeerMap.TryGetValue(netRequestEntityRef, out var peer))
                {
                    PeerRequest?.Invoke(peer, netRequestMessage);
                }
            });
        });
        
        _world.Destroy(requestQuery);
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