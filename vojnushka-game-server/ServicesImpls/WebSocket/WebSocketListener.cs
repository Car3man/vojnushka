﻿using VojnushkaShared.Net;
using WatsonWebsocket;

namespace VojnushkaGameServer.WebSocket
{
    internal class WebSocketListener : INetListener
    {
        private readonly Dictionary<Guid, WebSocketPeer> _peers = new();
        private WatsonWsServer? _ws;
        private int _peerCounter;

        public event PeerConnectDelegate? OnPeerConnect;
        public event PeerMessageDelegate? OnPeerMessage;
        public event PeerDisconnectDelegate? OnPeerDisconnect;

        public bool IsListening => _ws is { IsListening: true };

        public void Start(string ip, int port)
        {
            ThrowIfStarted();
            
            _ws = new WatsonWsServer(ip, port);
            _ws.ClientConnected += ClientConnected;
            _ws.ClientDisconnected += ClientDisconnected;
            _ws.MessageReceived += MessageReceived; 
            _ws.Start();
        }

        public void Stop()
        {
            ThrowIfNotStarted();
            
            _ws!.Stop();
            _ws.ClientConnected -= ClientConnected;
            _ws.ClientDisconnected -= ClientDisconnected;
            _ws.MessageReceived -= MessageReceived; 
        }

        public async void Send(IPeer peer, byte[] data)
        {
            ThrowIfNotStarted();
            
            var wsPeer = (WebSocketPeer)peer;
            await _ws!.SendAsync(wsPeer.Guid,  data);
        }
    
        public async void Broadcast(byte[] data)
        {
            ThrowIfNotStarted();
            
            var sendTasks = new List<Task>();
            foreach (var peerGuid in _peers.Keys)
            {
                var sendTask = _ws!.SendAsync(peerGuid, data);
                sendTasks.Add(sendTask);
            }
            await Task.WhenAll(sendTasks);
        }

        private void ClientConnected(object? sender, ConnectionEventArgs e)
        {
            var client = e.Client;
            var peer = new WebSocketPeer(client.Guid, client.IpPort, _peerCounter++);
            _peers.Add(client.Guid, peer);
            OnPeerConnect?.Invoke(peer);
        }

        private void MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            var clientGuid = e.Client.Guid;
            var peer = _peers[clientGuid];
            OnPeerMessage?.Invoke(peer, e.Data.ToArray());
        }

        private void ClientDisconnected(object? sender, DisconnectionEventArgs e)
        {
            var clientGuid = e.Client.Guid;
            var peer = _peers[clientGuid];
            _peers.Remove(clientGuid);
            OnPeerDisconnect?.Invoke(peer);
        }

        public void Dispose()
        {
            _ws?.Dispose();
        }

        private void ThrowIfStarted()
        {
            if (_ws != null)
            {
                throw new InvalidOperationException($"{nameof(WebSocketListener)} already started.");
            }
        }

        private void ThrowIfNotStarted()
        {
            if (_ws == null)
            {
                throw new InvalidOperationException($"{nameof(WebSocketListener)} didn't started.");
            }
        }
    }
}