using System.Collections.Generic;
using JetBrains.Annotations;
using NativeWebSocket;
using UnityEngine;
using VojnushkaShared.Net;

namespace Vojnushka.WebSocket
{
    public class WebSocketClient : MonoBehaviour, INetClient
    {
        private bool _tryToConnect;
        private NativeWebSocket.WebSocket _webSocket;
        private readonly Queue<byte[]> _sendQueue = new();

        public bool Connected { get; private set; }
        
        [CanBeNull] public event ConnectDelegate OnConnect;
        [CanBeNull] public event MessageDelegate OnMessage;
        [CanBeNull] public event DisconnectDelegate OnDisconnect;
        
        public async void Connect(INetConnectConfig netConnectConfig)
        {
            if (Connected || _tryToConnect)
            {
                return;
            }

            _tryToConnect = true;
            _webSocket = new NativeWebSocket.WebSocket($"ws://{netConnectConfig.Ip}:{netConnectConfig.Port}");
            _webSocket.OnOpen += OnSocketOpen;
            _webSocket.OnMessage += OnSocketMessage;
            _webSocket.OnError += OnSocketError;
            _webSocket.OnClose += OnSocketClose;
            await _webSocket.Connect();
        }

        public void Send(byte[] data)
        {
            _sendQueue.Enqueue(data);
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
            OnMessage?.Invoke(data);
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
            
            while (_sendQueue.Count > 0)
            {
                _webSocket.Send(_sendQueue.Dequeue());
            }
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

        public void Dispose()
        {
            Disconnect();
        }
    }
}