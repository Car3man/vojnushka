using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using NativeWebSocket;
using UnityEngine;
using Vojnushka.Network;
using VojnushkaProto.Core;

namespace Vojnushka.WebSocketNetwork
{
    public class WebSocketClient : MonoBehaviour, INetworkClient
    {
        private bool _tryToConnect;
        private WebSocket _webSocket;
        private readonly Queue<ServerProtoMsg> _sendQueue = new();

        private const int MaxMessageSize = 4096;

        public bool Connected { get; private set; }
        
        public event ConnectDelegate OnConnect;
        public event MessageDelegate OnMessage;
        public event DisconnectDelegate OnDisconnect;
        
        public async void Connect(string query)
        {
            if (Connected || _tryToConnect)
            {
                return;
            }

            _tryToConnect = true;
            _webSocket = new WebSocket(query);
            _webSocket.OnOpen += OnSocketOpen;
            _webSocket.OnMessage += OnSocketMessage;
            _webSocket.OnError += OnSocketError;
            _webSocket.OnClose += OnSocketClose;
            await _webSocket.Connect();
        }

        public void Send(ServerProtoMsg serverMessage)
        {
            _sendQueue.Enqueue(serverMessage);
        }

        public async void Disconnect()
        {
            if (!Connected)
            {
                return;
            }

            await _webSocket.Close();
        }

        private void OnSocketOpen()
        {
            _tryToConnect = false;
            Connected = true;
            OnConnect?.Invoke();
        }

        private void OnSocketMessage(byte[] data)
        {
            var wsMessage = WebSocketProtoMsg.Parser.ParseFrom(data);
            foreach (var serverMessage in wsMessage.Messages)
            {
                OnMessage?.Invoke(serverMessage);   
            }
        }

        private void OnSocketError(string errorMsg)
        {
            _webSocket.OnOpen -= OnSocketOpen;
            _webSocket.OnMessage -= OnSocketMessage;
            _webSocket.OnClose -= OnSocketClose;
            
            _tryToConnect = false;
        }

        private void OnSocketClose(WebSocketCloseCode closeCode)
        {
            _webSocket.OnOpen -= OnSocketOpen;
            _webSocket.OnMessage -= OnSocketMessage;
            _webSocket.OnClose -= OnSocketClose;
            
            Connected = false;
            OnDisconnect?.Invoke();
        }

        private void DispatchSendQueue()
        {
            if (_sendQueue.Count <= 0)
            {
                return;
            }

            var wsMessage = new WebSocketProtoMsg();
            var currBatchSize = 0;
            
            while (_sendQueue.Count > 0)
            {
                var message = _sendQueue.Peek();
                var messageSize = message.CalculateSize();
                if (currBatchSize + messageSize >= MaxMessageSize)
                {
                    break;
                }
                wsMessage.Messages.Add(_sendQueue.Dequeue());
            }

            using var memStream = new MemoryStream();
            wsMessage.WriteTo(memStream);
            _webSocket.Send(memStream.ToArray());
        }

        private void Update()
        {
            if (Connected)
            {
                DispatchSendQueue();
#if !UNITY_WEBGL || UNITY_EDITOR
                _webSocket.DispatchMessageQueue();
#endif
            }
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}