using Models;
using ReactUnity;
using ReactUnity.Services;
using System.Net;

namespace Controllers
{
    public class TrainerController : Controller<TrainerModel>
    {
        public ISceneService _sceneService;
        public IStaticDataService _staticDataService;
        public IFirebaseService _fireaseService;
        public IS3Service _s3Service;
        
        public WebClient WebClient { get; set; }

        TrainerController(ISceneService sceneService, IStaticDataService staticDataService, IFirebaseService firebaseService, IS3Service s3Service)
        {
            _sceneService = sceneService;
            _staticDataService = staticDataService;
            _fireaseService = firebaseService;
            _s3Service = s3Service;
        }

        public override void Start()
        {
            SetState(
                _sceneService.GetState<TrainerModel>()
            );
            WebClient = new WebClient();
        }

        public void TakePhoto(byte[] photo)
        {
            var subject = State.Subject;

            // Save the photo
            _staticDataService.WriteToDisk(photo, extension: "jpg", subdirectory: subject);

            SetState(new TrainerModel()
            {
                Subject = subject,
                SampleCount = State.SampleCount + 1
            });
        }

        public void FinishedCapture()
        {
            _fireaseService.UpdateModel(State.Subject, State.SampleCount, 0);
            _sceneService.GoBack();
        }

        public void SetSubject(string subject)
        {
            SetState(new TrainerModel()
            {
                Subject = subject,
                SampleCount = State.SampleCount
            });

            _fireaseService.NewModel(State.Subject);
        }
    }
}
