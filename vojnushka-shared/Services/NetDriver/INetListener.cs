﻿using System.Threading.Tasks;

namespace VojnushkaShared.NetDriver
{
    public interface INetListener : System.IDisposable
    {
        event PeerConnectDelegate OnPeerConnect;
        event PeerMessageDelegate OnPeerMessage;
        event PeerDisconnectDelegate OnPeerDisconnect;
        bool IsListening { get; }
        Task StartAsync(INetConnectConfig connectConfig);
        void Stop();
        void Send(IPeer peer, byte[] data);
        void Broadcast(byte[] data);
    }

    public delegate void PeerConnectDelegate(IPeer peer);
    public delegate void PeerMessageDelegate(IPeer peer, byte[] data);
    public delegate void PeerDisconnectDelegate(IPeer peer);
}