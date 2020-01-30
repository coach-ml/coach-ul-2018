using Zenject;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using Amazon;

namespace ReactUnity
{
    public class App : MonoInstaller<App>, IApp
    {
        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        /// <summary>
        /// Our apps main entry point
        /// </summary>
        private void Awake()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);
        }

        private void Update()
        {
            Action action;
            while (_actions.TryDequeue(out action))
            {
                action();
            }
        }

        public void RunOnUnityThread(Action action)
        {
            _actions.Enqueue(action);
        }

        /// <summary>
        /// Shouldn't ever have to invoke this manually.
        /// Binds the Services classes to their respective Interfaces
        /// </summary>
        public override void InstallBindings()
        {
            var assembly =
                Assembly.GetExecutingAssembly().GetTypes().Where(a => a.Namespace == "ReactUnity.Services");
            foreach (var _interface in assembly)
            {
                if (_interface.IsInterface)
                {
                    var _class =
                        assembly.SingleOrDefault(a =>
                            a.IsClass &&
                            a.Name == _interface.Name.Remove(0, 1));
                    Container.Bind(_interface).To(_class).AsSingle();
                }
            }
        }

        public T Init<T>()
        {
            return Container.Instantiate<T>();
        }
    }
}