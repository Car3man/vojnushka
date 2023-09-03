using Autofac;
using VojnushkaGameServer.Core;
using VojnushkaGameServer.Domain;
using VojnushkaGameServer.Logger;
using VojnushkaGameServer.Network;
using VojnushkaGameServer.WebSocketNetwork;

namespace VojnushkaGameServer.Infrastructure;

public class ServerContext : IDisposable, IAsyncDisposable
{
    private readonly IContainer _container;
    
    public ServerContext()
    {
        _container = RegisterDependencies();
    }

    public Server CreateServer()
    {
        return _container.Resolve<Server>();
    }
    
    private IContainer RegisterDependencies()
    {
        var containerBuilder = new ContainerBuilder();
        RegisterLogger(containerBuilder);
        RegisterNetwork(containerBuilder);
        RegisterServer(containerBuilder);
        RegisterServerWorld(containerBuilder);
        return containerBuilder.Build();
    }

    private void RegisterLogger(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .RegisterType<ConsoleLogger>()
            .As<ILogger>();
    }

    private void RegisterNetwork(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .RegisterType<WebSocketServer>()
            .As<INetworkServer>();
    }

    private void RegisterServer(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .RegisterType<Server>();
    }
    
    private void RegisterServerWorld(ContainerBuilder containerBuilder)
    {
        var configuration = new ServerWorldConfiguration()
        {
            Systems = new ISystem[]
            {
                new PingPongSystem()
            }
        };

        containerBuilder
            .RegisterInstance(configuration);
        
        containerBuilder
            .RegisterType<ServerWorld>()
            .As<IServerWorld>();
    }

    public void Dispose()
    {
        _container.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}