using System.Threading.Tasks;
using Autofac;
using Vojnushka.EditorLogger;
using VojnushkaShared.Logger;

namespace Vojnushka.Infrastructure
{
    public class AppContext
    {
        public IContainer Container { get; private set; }

        public void Run()
        {
            Container.Resolve<StartupLoader>();
        }
        
        public void RegisterDependencies()
        {
            var containerBuilder = new ContainerBuilder();
            RegisterLogger(containerBuilder);
            RegisterStartupLoader(containerBuilder);
            Container = containerBuilder.Build();
        }

        private void RegisterLogger(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<DebugLoggerAdapter>()
                .As<ILogger>();
        }

        private void RegisterStartupLoader(ContainerBuilder containerBuilder)
        {
            containerBuilder
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