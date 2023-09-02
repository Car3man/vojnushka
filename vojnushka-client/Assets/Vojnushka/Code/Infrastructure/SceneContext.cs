using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using UnityEngine;

namespace Vojnushka.Infrastructure
{
    [DefaultExecutionOrder(-1000)]
    public abstract class SceneContext : MonoBehaviour, IAsyncDisposable
    {
        [SerializeField] protected bool injectToGameObjects;
        
        protected ILifetimeScope Scope { get; private set; }

        private void Awake()
        {
            var hasContext = App.HasContext;
            var appContext = App.Context;
            if (hasContext)
            {
                Scope = appContext
                    .Container
                    .BeginLifetimeScope(RegisterDependencies);

                if (injectToGameObjects)
                {
                    InjectToGameObjects();
                }
                
                Run();
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void InjectToGameObjects()
        {
            var monoBehaviours = FindObjectsOfType<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                InjectToGameObject(monoBehaviour);    
            }
        }

        private void InjectToGameObject(MonoBehaviour monoBehaviour)
        {
            var methods = monoBehaviour.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0)
                .ToArray();
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var arguments = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    arguments[i] = Scope.Resolve(parameter.ParameterType);
                }
                method.Invoke(monoBehaviour, arguments);
            }
        }

        protected virtual void Run()
        {
        }

        protected abstract void RegisterDependencies(ContainerBuilder builder);

        public void Dispose()
        {
            Scope?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (Scope != null)
            {
                await Scope.DisposeAsync();
            }
        }
    }
}