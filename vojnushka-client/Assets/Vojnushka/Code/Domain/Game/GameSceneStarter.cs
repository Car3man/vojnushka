using UnityEngine;
using Vojnushka.Infrastructure;
using Vojnushka.Network;
using VojnushkaProto;
using VojnushkaProto.Core;
using VojnushkaProto.PingPong;
using VojnushkaProto.Utility;

namespace Vojnushka.Game
{
    public class GameSceneStarter : MonoBehaviour, System.IDisposable
    {
        private bool _constructed;
        private INetworkClient _networkClient;

        [Inject]
        public void Construct(
            INetworkClient networkClient
            )
        {
            _networkClient = networkClient;
            _networkClient.OnConnect += OnConnect;
            _networkClient.OnMessage += OnMessage;
            _networkClient.OnDisconnect += OnDisconnect;
            _constructed = true;
        }

        public void Dispose()
        {
            _networkClient.OnConnect -= OnConnect;
            _networkClient.OnMessage -= OnMessage;
            _networkClient.OnDisconnect -= OnDisconnect;
        }

        private void OnConnect()
        {
            Debug.Log("OnConnect");

            var pingMessage = new PingProtoMsg()
            {
                Message = "Hello server!"
            };

            _networkClient.Send(new ServerProtoMsg
            {
                Type = ServerProtoMsgType.Ping,
                Data = MessageUtility.MessageToByteString(pingMessage)
            });
        }

        private void OnMessage(ServerProtoMsg message)
        {
            switch (message.Type)
            {
                case ServerProtoMsgType.Pong:
                    var pongMessage = PongProtoMsg.Parser.ParseFrom(message.Data);
                    Debug.Log($"Pong from server: {pongMessage.Message}");
                    break;
                case ServerProtoMsgType.Greeting:
                    var greetingMessage = GreetingProtoMsg.Parser.ParseFrom(message.Data);
                    Debug.Log($"Greetings from server, my id is {greetingMessage.Id}");
                    break;

            }
        }

        private void OnDisconnect()
        {
            Debug.Log("OnDisconnect");
        }

        private void Start()
        {
            if (!_constructed)
            {
                return;
            }
            
            _networkClient.Connect("ws://localhost:9000");
        }
    }
}