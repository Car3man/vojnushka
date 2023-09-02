using VojnushkaGameServer.Network;
using WatsonWebsocket;

namespace VojnushkaGameServer.WebSocketNetwork;

public class WebSocketServer : INetworkServer
{
    private readonly WatsonWsServer _ws = new();
    private readonly Dictionary<Guid, WebSocketPeer> _peers = new();

    public event PeerConnectDelegate? OnPeerConnect;
    public event PeerMessageDelegate? OnPeerMessage;
    public event PeerDisconnectDelegate? OnPeerDisconnect;

    public void Start()
    {
        _ws.ClientConnected += ClientConnected;
        _ws.ClientDisconnected += ClientDisconnected;
        _ws.MessageReceived += MessageReceived; 
        _ws.Start();
    }

    public void Stop()
    {
        _ws.ClientConnected -= ClientConnected;
        _ws.ClientDisconnected -= ClientDisconnected;
        _ws.MessageReceived -= MessageReceived; 
        _ws.Stop();
    }

    public async void Send(IPeer peer, byte[] data)
    {
        var wsPeer = (WebSocketPeer)peer;
        await _ws.SendAsync(wsPeer.Guid, data);
    }

    public async void Broadcast(byte[] data)
    {
        var sendTasks = new List<Task>();
        foreach (var peerGuid in _peers.Keys)
        {
            var sendTask = _ws.SendAsync(peerGuid, data);
            sendTasks.Add(sendTask);
        }
        await Task.WhenAll(sendTasks);
    }

    private void ClientConnected(object? sender, ConnectionEventArgs e)
    {
        var client = e.Client;
        var peer = new WebSocketPeer(client.Guid, client.IpPort);
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
}