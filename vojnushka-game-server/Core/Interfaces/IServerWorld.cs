﻿using VojnushkaGameServer.Network;

namespace VojnushkaGameServer.Core;

public interface IServerWorld
{
    event ServerWorldPeerRequestDelegate PeerRequest;
    void Start();
    void Stop();
    void Tick(float deltaTime);
    void AddPeer(IPeer peer);
    void AddPeerMessage(IPeer peer, ServerMessage message);
    void RemovePeer(IPeer peer);
}

public delegate void ServerWorldPeerRequestDelegate(IPeer peer, ServerMessage message);