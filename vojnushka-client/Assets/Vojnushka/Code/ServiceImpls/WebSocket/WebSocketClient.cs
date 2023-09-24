using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NativeWebSocket;
using UnityEngine;
using VojnushkaShared.Net;

namespace Vojnushka.WebSocket
{
    public class WebSocketClient : MonoBehaviour, INetClient
    {
        private bool _tryToConnect;
        private TaskCompletionSource<object> _tryConnectCompletionSource;
        private readonly Queue<byte[]> _sendQueue = new();
        private NativeWebSocket.WebSocket _webSocket;

        public bool Connected { get; private set; }
        
        [CanBeNull] public event ConnectDelegate OnConnect;
        [CanBeNull] public event MessageDelegate OnMessage;
        [CanBeNull] public event DisconnectDelegate OnDisconnect;
        
        public async Task ConnectAsync(INetConnectConfig netConnectConfig)
        {
            if (Connected || _tryToConnect)
            {
                throw new Exception("Cannot connect due already connected or trying to connect.");
            }

            _tryToConnect = true;
            _tryConnectCompletionSource = new TaskCompletionSource<object>();
            
            _webSocket = new NativeWebSocket.WebSocket($"ws://{netConnectConfig.Ip}:{netConnectConfig.Port}");
            
            _webSocket.OnOpen += OnSocketOpen;
            _webSocket.OnMessage += OnSocketMessage;
            _webSocket.OnError += OnSocketError;
            _webSocket.OnClose += OnSocketClose;
            
#pragma warning disable CS4014
            _webSocket.Connect();
#pragma warning restore CS4014

            await _tryConnectCompletionSource.Task;
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
            _tryConnectCompletionSource.SetResult(null);
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
            _tryConnectCompletionSource.SetException(new SystemException(errorMsg));
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