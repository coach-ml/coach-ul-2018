using Models;
using ReactUnity;
using ReactUnity.Services;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    class ItemController : Controller<ItemModel>
    {
        private IStaticDataService _staticDataService;
        private ISceneService _sceneService;

        ItemController(IStaticDataService staticDataService, ISceneService sceneService)
        {
            _staticDataService = staticDataService;
            _sceneService = sceneService;
        }

        public override void Start()
        {
            
        }

        public void DownloadAndPlay(Action finished = null)
        {
            // TODO
            finished?.Invoke();
            Play();
        }

        // Load up the play scene
        public void Play()
        {
            /*
            string path = ""; // _staticDataService.FullBinaryPath(State.Name);
            _sceneService.LoadScene<Scenes.WebcamScene>(new WebcamModel()
            {
                BinaryPath = path,
                Labels = State.Labels,
                Title = State.Name.ToUpper()[0] + State.Name.Substring(1) + " Recognizer"
            }, LoadSceneMode.Additive);
            */
        }
    }
}