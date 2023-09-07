using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using UnityEngine;

namespace Vojnushka.Infrastructure
{
    [DefaultExecutionOrder(-1000)]
    public abstract class SceneContext : MonoBehaviour, IAsyncDisposable
    {
        [SerializeField] protected bool injectToGameObjects = true;
        private readonly List<IDisposable> _gameObjectsToDispose = new();
        
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

            if (methods.Length > 0 && monoBehaviour is IDisposable disposable)
            {
                _gameObjectsToDispose.Add(disposable);
            }
        }

        protected virtual void Run()
        {
        }

        protected abstract void RegisterDependencies(ContainerBuilder builder);

        private void DisposeGameObjects()
        {
            foreach (var disposable in _gameObjectsToDispose)
            {
                disposable.Dispose();
            }
            
            _gameObjectsToDispose.Clear();
        }
        
        public void Dispose()
        {
            DisposeGameObjects();
            Scope?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            DisposeGameObjects();
            
            if (Scope != null)
            {
                await Scope.DisposeAsync();
            }
        }
    }
}