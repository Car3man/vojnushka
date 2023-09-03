using System.IO;
using System.Text;
using Google.Protobuf;
using NativeWebSocket;
using UnityEngine;

namespace Vojnushka
{
    public class Test : MonoBehaviour
    {
        private WebSocket _websocket;
        
        private async void Start()
        {
            _websocket = new WebSocket("ws://localhost:9000");
            _websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
                _websocket.Send(CreatePingMessageBytes());
            };
            _websocket.OnError += e =>
            {
                Debug.Log("Error! " + e);
            };
            _websocket.OnClose += _ =>
            {
                Debug.Log("Connection closed!");
            };
            _websocket.OnMessage += bytes =>
            {
                var pongMessage = ParsePongMessage(bytes);
                Debug.Log(pongMessage.Message);
            };
            
            // waiting for messages
            await _websocket.Connect();
        }

        private byte[] CreatePingMessageBytes()
        {
            var pingMessage = new PingMessage
            {
                Message = "Hello from client!"
            };
            using var pingMessageMemStream = new MemoryStream();
            pingMessage.WriteTo(pingMessageMemStream);
            var pingMessageByteString = ByteString.CopyFrom(pingMessageMemStream.ToArray());
            var serverMessage = new ServerMessage
            {
                Type = ServerMessageType.Ping,
                Data = pingMessageByteString
            };
            var wsMessage = new WebSocketMessage
            {
                Messages = { serverMessage }
            };
            using var wsMemStream = new MemoryStream();
            wsMessage.WriteTo(wsMemStream);
            return wsMemStream.ToArray();
        }
        
        private PongMessage ParsePongMessage(byte[] data)
        {
            var wsMessage = WebSocketMessage.Parser.ParseFrom(data);
            var serverMessage = wsMessage.Messages[0];
            return PongMessage.Parser.ParseFrom(serverMessage.Data);
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _websocket.DispatchMessageQueue();
#endif
        }

        private async void OnApplicationQuit()
        {
            await _websocket.Close();
        }
    }
}