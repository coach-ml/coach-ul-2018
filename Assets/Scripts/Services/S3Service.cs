using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using System.Threading.Tasks;

namespace ReactUnity.Services
{
    public interface IS3Service : IService
    {
        void Initialize(CoachUser user);
        Task<bool> UploadFile(string filename, string label);
        void WatchRoot();
    }

    public class S3Service : IS3Service
    {
        private AmazonS3Client Client { get; set; }
        private string Bucket { get; set; }
        private string ClientName { get; set; }

        IFirebaseService _firebaseService;

        public S3Service(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public void Initialize(CoachUser user)
        {
            Bucket = user.bucket;
            ClientName = user.clientName;
            Client = new AmazonS3Client(user.s3Key, user.s3Secret, RegionEndpoint.USWest2);
        }

        public async void WatchRoot()
        {
            var path = Path.Combine(Application.persistentDataPath, "upload_data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                var label = Path.GetFileName(dir);
                var files = Directory.GetFiles(dir);
                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var didSucceed = await UploadFile(file, label);
                    if (didSucceed) {
                        await _firebaseService.UpdateModel(label);
                        File.Delete(file);
                    }
                }
            }
        }

        public Task<bool> UploadFile(string fileName, string label)
        {
            var tcs = new TaskCompletionSource<bool>();

            var stream = new FileStream(
                Path.Combine(Application.persistentDataPath, fileName),
                FileMode.Open, FileAccess.Read, FileShare.Read
            );

            var request = new PostObjectRequest()
            {
                Bucket = Bucket,
                Key = $"uploads/{ClientName}/{label}/{Path.GetFileName(fileName)}",
                InputStream = stream,
                CannedACL = S3CannedACL.Private
            };
            Client.PostObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                {
                    Debug.Log("Uploaded " + fileName);
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }
    }
}
