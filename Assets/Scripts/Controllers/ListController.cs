using Models;
using ReactUnity;
using ReactUnity.Services;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Coach;

namespace Controllers
{
    public class ListController : Controller<ListModel>
    {
        public IStaticDataService _staticDataService;
        public ISceneService _sceneService;
        public IS3Service _s3Service;
        public IFirebaseService _firebaseService;
        public ICoachService _coachService;

        private CoachUser UserDetails { get;  set; }

        ListController(IStaticDataService staticDataService, ISceneService sceneService, IS3Service s3Service, IFirebaseService firebaseService, ICoachService coachService)
        {
            _staticDataService = staticDataService;
            _sceneService = sceneService;
            _s3Service = s3Service;
            _firebaseService = firebaseService;
            _coachService = coachService;
        }

        public override async void Start()
        {
            UserDetails = await _firebaseService.GetUserDetails();
            _s3Service.Initialize(UserDetails);
            _s3Service.WatchRoot();

            await _coachService.Initialize(UserDetails.coachApi);
            GetModels();
        }

        public void OnFocus()
        {
            _s3Service.WatchRoot();
        }

        public Task<CoachModel> DownloadModel(string modelName)
        {
            return _coachService.GetModel(modelName);
        }

        public async void GetModels()
        {
            var models = await _coachService.GetModels();
            var listModel = new List<ItemModel>();

            var firebaseModels = (await _firebaseService.GetBuiltModels());

            foreach (var model in models)
            {
                if (firebaseModels.ContainsKey(model)) {
                    listModel.Add(new ItemModel()
                    {
                        Name = model
                    });
                }
            }

            SetState(new ListModel() { Models = listModel });
        }

        public void Logout()
        {
            // TODO: This is a hack, do this proper sometime, will need to refactor Splash -> List
            _firebaseService.Logout();
            Application.Quit(0);
        }
    }
}
