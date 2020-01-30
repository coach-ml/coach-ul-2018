using Models;
using ReactUnity;
using ReactUnity.Services;
using UnityEngine;

namespace Controllers
{
    public class SplashController : Controller<SplashModel>
    {
        private ISceneService _sceneService;
        private IFirebaseService _firebaseService;

        public SplashController(ISceneService sceneService, IFirebaseService firebaseService)
        {
            _sceneService = sceneService;
            _firebaseService = firebaseService;
        }

        public override void Start()
        {
            _firebaseService.Initialize(AuthStateChanged);
            Debug.Log("GPU NAME: " + SystemInfo.graphicsDeviceName);
            Debug.Log("GPU TYPE: " + SystemInfo.graphicsDeviceType);
        }

        public void SignIn(string email, string password)
        {
            _firebaseService.SignIn(email, password);
        }

        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (_firebaseService.User != null)
            {
                _sceneService.LoadScene<Scenes.ListScene>(new SplashModel() { IsReady = true });
            }
        }
    }
}