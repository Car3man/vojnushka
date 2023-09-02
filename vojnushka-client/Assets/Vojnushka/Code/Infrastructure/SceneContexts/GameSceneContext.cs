using Autofac;

namespace Vojnushka.Infrastructure
{
    public class GameSceneContext : SceneContext
    {
        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance("Hello mono!");
        }
    }
}