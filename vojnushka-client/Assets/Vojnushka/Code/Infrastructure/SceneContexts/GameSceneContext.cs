using Autofac;
using UnityEngine;
using Vojnushka.Network;
using Vojnushka.WebSocketNetwork;

namespace Vojnushka.Infrastructure
{
    public class GameSceneContext : SceneContext
    {
        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            RegisterGameNetwork(builder);
        }

        private void RegisterGameNetwork(ContainerBuilder builder)
        {
            var webSocketClientObject = new GameObject(nameof(WebSocketClient));
            var webSocketClient = webSocketClientObject.AddComponent<WebSocketClient>();
            builder
                .RegisterInstance(webSocketClient)
                .As<INetworkClient>();
        }
    }
}