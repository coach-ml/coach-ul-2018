using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public interface IScene { string Name { get; } }

    public class SplashScene : IScene
    {
        public string Name { get { return "Splash"; } }
    }

    public class ListScene : IScene
    {
        public string Name { get { return "List"; } }
    }

    public class ItemInfoScene : IScene
    {
        public string Name { get { return "Item"; } }
    }

    public class TrainerScene : IScene
    {
        public string Name { get { return "Trainer"; } }
    }

    public class TestScene : IScene
    {
        public string Name { get { return "Test"; } }
    }
}

namespace ReactUnity.Services
{
    public class Scene
    {
        public string Name { get; private set; }
        public IModel State { get; private set; }

        public Scene(string name, IModel state)
        {
            this.Name = name;
            this.State = state;
        }

        public M GetState<M>() where M : IModel, new()
        {
            if (State?.GetType() != typeof(M))
                return new M();

            return (M)Convert.ChangeType(State, typeof(M));
        }
    }

    public interface ISceneService : IService
    {
        M GetState<M>() where M : IModel, new();
        void LoadScene<S>(IModel newState = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where S : Scenes.IScene, new();
        void GoBack();
    }

    class SceneService : ISceneService
    {
        private List<string> SceneNameHistory { get; set; }
        private IModel State { get; set; }

        SceneService()
        {
            SceneNameHistory = new List<string>();
        }

        public M GetState<M>() where M : IModel, new()
        {
            if (typeof(M) != State.GetType())
                return new M();

            return (M)Convert.ChangeType(State, typeof(M));
        }

        public void LoadScene<S>(IModel newState = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where S : Scenes.IScene, new()
        {
            string sceneName = new S().Name;
            if (!SceneNameHistory.Contains(sceneName))
                SceneNameHistory.Add(sceneName);
            
            State = newState;
            
            SceneManager.LoadScene(sceneName, loadSceneMode);
        }

        public void GoBack()
        {
            string currentScene = SceneNameHistory.Count <= 1 ? null : SceneNameHistory[SceneNameHistory.Count - 1];
            SceneManager.UnloadSceneAsync(currentScene);
            SceneNameHistory.RemoveAt(SceneNameHistory.Count - 1);
        }

        public void GoBack<T>() where T : Presenter<Controller<IModel>, IModel>
        {
            GoBack();
            var presenter = GameObject.FindGameObjectWithTag("Presenter").GetComponent<T>();
            presenter.OnRefocus();
        }
    }
}
