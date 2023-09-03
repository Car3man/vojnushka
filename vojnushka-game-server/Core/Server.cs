﻿using VojnushkaGameServer.Logger;
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
        _world.PeerRequest += OnWorldPeerRequest;
    }

    public void Dispose()
    {
        _network.OnPeerConnect -= OnPeerConnect;
        _network.OnPeerMessage -= OnPeerMessage;
        _network.OnPeerDisconnect -= OnPeerDisconnect;
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

    public void Send(IPeer peer, ServerMessage message)
    {
        _network.Send(peer, message);
    }

    public void Broadcast(ServerMessage message)
    {
        _network.Broadcast(message);
    }

    public void OnPeerConnect(IPeer peer)
    {
        _logger.Log($"Peer connected, guid: {peer.Id}");
        
        _world.AddPeer(peer);
    }

    public void OnPeerMessage(IPeer peer, ServerMessage message)
    {
        _logger.Log($"Peer message, guid: {peer.Id}");
        
        _world.AddPeerMessage(peer, message);
    }

    public void OnPeerDisconnect(IPeer peer)
    {
        _logger.Log($"Peer disconnected, guid: {peer.Id}");
        
        _world.RemovePeer(peer);
    }
    
    private void OnWorldPeerRequest(IPeer peer, ServerMessage message)
    {
        _logger.Log($"Peer world request, guid: {peer.Id}");
        
        Send(peer, message);
    }

    private bool ShouldTerminate()
    {
        return false;
    }
}