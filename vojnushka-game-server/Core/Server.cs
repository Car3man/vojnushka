using VojnushkaGameServer.Logger;
using VojnushkaGameServer.Network;

namespace VojnushkaGameServer.Core;

public class Server : IServer, IDisposable
{
    private readonly ILogger _logger;
    private readonly INetworkServer _network;
    private readonly IServerWorld _world;

    private float _time;
    private float _lastTickTime;

    private const int TickRate = 24;
    private const int TickDelayMs = (int)(1f / TickRate * 1000);

    public Server(
        ILogger logger,
        INetworkServer network,
        IServerWorld world
        )
    {
        _logger = logger;
        _network = network;
        _world = world;
        
        _network.OnPeerConnect += OnPeerConnect;
        _network.OnPeerMessage += OnPeerMessage;
        _network.OnPeerDisconnect += OnPeerDisconnect;
        _world.BroadcastRequest += OnWorldBroadcastRequest;
        _world.PeerRequest += OnWorldPeerRequest;
    }

    public void Dispose()
    {
        _network.OnPeerConnect -= OnPeerConnect;
        _network.OnPeerMessage -= OnPeerMessage;
        _network.OnPeerDisconnect -= OnPeerDisconnect;
        _world.BroadcastRequest -= OnWorldBroadcastRequest;
        _world.PeerRequest -= OnWorldPeerRequest;
    }
    
    public async Task Run()
    {
        _network.Start();
        await GameWorldLoop();
        _network.Stop();
    }

    private async Task GameWorldLoop()
    {
        _world.Start();

        while (!ShouldTerminate())
        {
            _world.Tick(_time - _lastTickTime);
            _lastTickTime = _time;
            
            await Task.Delay(TickDelayMs);
            _time += TickDelayMs;
        }

        _world.Stop();
    }

    public void Send(IPeer peer, byte[] data)
    {
        _network.Send(peer, data);
    }

    public void Broadcast(byte[] data)
    {
        _network.Broadcast(data);
    }

    public void OnPeerConnect(IPeer peer)
    {
        _logger.Log($"Peer connected, guid: {peer.Id}");
        
        _world.AddPeer(peer);
    }

    public void OnPeerMessage(IPeer peer, byte[] data)
    {
        _logger.Log($"Peer message, guid: {peer.Id}");
        
        _world.AddPeerMessage(peer, data);
    }

    public void OnPeerDisconnect(IPeer peer)
    {
        _logger.Log($"Peer disconnected, guid: {peer.Id}");
        
        _world.RemovePeer(peer);
    }

    private void OnWorldBroadcastRequest(byte[] data)
    {
        // _logger.Log("Broadcast world request");
        
        Broadcast(data);
    }
    
    private void OnWorldPeerRequest(IPeer peer, byte[] data)
    {
        _logger.Log($"Peer world request, guid: {peer.Id}");
        
        Send(peer, data);
    }

    private bool ShouldTerminate()
    {
        return false;
    }
}