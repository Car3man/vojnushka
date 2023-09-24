// ReSharper disable NotAccessedField.Local

using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Lifestyles;
using VojnushkaGameServer.ConsoleLogger;
using VojnushkaGameServer.Domain;
using VojnushkaGameServer.WebSocketDriver;
using VojnushkaShared.Logger;
using VojnushkaShared.NetDriver;

namespace VojnushkaGameServer;

internal class App
{
    public static async Task Main()
    {
        var container = new Container();
        container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        RegisterLogger(container);
        RegisterNetwork(container);
        RegisterServerWorld(container);
        RegisterServer(container);
        container.Verify();

        await using Scope scope = AsyncScopedLifestyle.BeginScope(container);
        await container.GetInstance<Server>().Run();
    }

    private static void RegisterLogger(Container container)
    {
        container.Register<ILogger, Logger>(Lifestyle.Singleton);
    }

    private static void RegisterNetwork(Container container)
    {
        container.Register<INetListener, WebSocketListener>(Lifestyle.Scoped);
        container.GetRegistration(typeof(INetListener))!.Registration
            .SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "INetListener is disposed by code.");
        container.RegisterInstance<INetConnectConfig>(new NetConnectConfig("127.0.0.1", 6969));
    }

    private static void RegisterServerWorld(Container container)
    {
        container.Register<ServerWorld>(Lifestyle.Scoped);
    }
    
    private static void RegisterServer(Container container)
    {
        container.Register<Server>(Lifestyle.Scoped);
    }
}