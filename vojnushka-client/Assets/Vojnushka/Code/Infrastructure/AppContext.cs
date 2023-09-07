using System;
using System.Threading.Tasks;
using Autofac;

namespace Vojnushka.Infrastructure
{
    public class AppContext : IAsyncDisposable
    {
        public IContainer Container { get; private set; }

        public void Run()
        {
            Container.Resolve<StartupLoader>();
        }
        
        public IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            RegisterDependencies(builder);
            Container = builder.Build();
            return Container;
        }

        private void RegisterDependencies(ContainerBuilder builder)
        {
            builder
                .RegisterType<StartupLoader>()
                .InstancePerLifetimeScope();
        }

        public void Dispose()
        {
            Container?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (Container != null) await Container.DisposeAsync();
        }
    }
}