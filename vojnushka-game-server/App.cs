// ReSharper disable NotAccessedField.Local

using Autofac;
using VojnushkaGameServer.ConsoleLogger;
using VojnushkaGameServer.WebSocket;
using VojnushkaShared.Logger;
using VojnushkaShared.Net;

namespace VojnushkaGameServer;

internal class App
{
    public static async Task Main()
    {
        var containerBuilder = new ContainerBuilder();
        RegisterLogger(containerBuilder);
        RegisterNetwork(containerBuilder);
        RegisterServerWorld(containerBuilder);
        RegisterServer(containerBuilder);
        await using var container = containerBuilder.Build();
        using var server = container.Resolve<Server>();
        await server.Run();
    }

    private static void RegisterLogger(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .RegisterType<Logger>()
            .As<ILogger>()
            .SingleInstance();
    }

    private static void RegisterNetwork(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .RegisterType<WebSocketListener>()
            .As<INetListener>()
            .SingleInstance();
        containerBuilder
            .RegisterInstance(new NetConnectConfig("127.0.0.1", 6969))
            .As<INetConnectConfig>()
            .SingleInstance();
    }

    private static void RegisterServerWorld(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<ServerWorld>().SingleInstance();
    }
    
    private static void RegisterServer(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<Server>().SingleInstance();
    }
}