using Google.Protobuf;
using VojnushkaGameServer.Network;
using VojnushkaProto.Core;
using WatsonWebsocket;

namespace VojnushkaGameServer.WebSocketNetwork;

public class WebSocketServer : INetworkServer
{
    private readonly WatsonWsServer _ws = new();
    private readonly Dictionary<Guid, WebSocketPeer> _peers = new();
    private int _peerCounter;

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

    public async void Send(IPeer peer, ServerProtoMsg message)
    {
        var wsPeer = (WebSocketPeer)peer;
        var wsMessage = new WebSocketProtoMsg
        {
            Messages = { message }
        };
        using var memStream = new MemoryStream();
        wsMessage.WriteTo(memStream);
        await _ws.SendAsync(wsPeer.Guid,  memStream.ToArray());
    }
    
    public async void Broadcast(ServerProtoMsg message)
    {
        var wsMessage = new WebSocketProtoMsg
        {
            Messages = { message }
        };
        using var memStream = new MemoryStream();
        wsMessage.WriteTo(memStream);
        var sendTasks = new List<Task>();
        foreach (var peerGuid in _peers.Keys)
        {
            var sendTask = _ws.SendAsync(peerGuid, memStream.ToArray());
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
        var wsMessage = WebSocketProtoMsg.Parser.ParseFrom(e.Data.ToArray());
        foreach (var serverMessage in wsMessage.Messages)
        {
            OnPeerMessage?.Invoke(peer, serverMessage);   
        }
    }

    private void ClientDisconnected(object? sender, DisconnectionEventArgs e)
    {
        var clientGuid = e.Client.Guid;
        var peer = _peers[clientGuid];
        _peers.Remove(clientGuid);
        OnPeerDisconnect?.Invoke(peer);
    }
}