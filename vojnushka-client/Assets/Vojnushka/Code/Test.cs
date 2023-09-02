using System.Text;
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
                _websocket.Send(Encoding.UTF8.GetBytes("Hello from client!"));
            };
            _websocket.OnError += e => { Debug.Log("Error! " + e); };
            _websocket.OnClose += _ => { Debug.Log("Connection closed!"); };
            _websocket.OnMessage += bytes =>
            {
                Debug.Log(System.Text.Encoding.UTF8.GetString(bytes));
            };
            
            // waiting for messages
            await _websocket.Connect();
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