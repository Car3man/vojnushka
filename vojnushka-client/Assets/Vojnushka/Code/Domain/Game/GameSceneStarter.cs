using UnityEngine;
using Vojnushka.Infrastructure;
using Vojnushka.Network;

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
        }

        private void OnMessage(byte[] data)
        {
            
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