using Autofac;
using UnityEngine;
using Vojnushka.Game;
using Vojnushka.WebSocket;
using VojnushkaShared.Net;

namespace Vojnushka.Infrastructure
{
    public class GameSceneContext : SceneContext
    {
        protected override void RegisterDependencies(ContainerBuilder containerBuilder)
        {
            RegisterNetwork(containerBuilder);
            RegisterGameWorld(containerBuilder);
        }

        private void RegisterNetwork(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterInstance(new NetConnectConfig("127.0.0.1", 6969))
                .As<INetConnectConfig>()
                .SingleInstance();
            
            var wsClientObject = new GameObject(nameof(WebSocketClient));
            var wsClient = wsClientObject.AddComponent<WebSocketClient>();
            containerBuilder
                .RegisterInstance(wsClient)
                .As<INetClient>();
        }

        private void RegisterGameWorld(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<GameWorld>();
        }
    }
}