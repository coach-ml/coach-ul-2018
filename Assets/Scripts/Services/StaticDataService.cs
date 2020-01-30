using System;
using System.IO;
using System.Net;
using UnityEngine;

namespace ReactUnity.Services
{
    public interface IStaticDataService : IService {
        void DownloadAsset(Uri url, DownloadDataCompletedEventHandler downloadFinished);
        void WriteToDisk(byte[] bytes, string filename = null, string extension = null, string subdirectory = null);
    }

    class StaticDataService : IStaticDataService
    {
        public void DownloadAsset(Uri url, DownloadDataCompletedEventHandler downloadFinished)
        {
            using (var w = new WebClient()) {
                w.DownloadDataCompleted += downloadFinished;
                w.DownloadDataAsync(url);
            }
        }

        public void WriteToDisk(byte[] bytes, string filename = null, string extension = null, string subdirectory = null)
        {
            if (String.IsNullOrEmpty(filename))
            {
                if (!String.IsNullOrEmpty(extension))
                {
                    filename = Path.GetRandomFileName().Split('.')[0] + "." + extension;
                }
            }

            var startingPath = Path.Combine(Application.persistentDataPath, "upload_data");
            if (!Directory.Exists(startingPath))
            {
                Directory.CreateDirectory(startingPath);
            }
            if (!String.IsNullOrEmpty(subdirectory))
                startingPath = Path.Combine(startingPath, subdirectory);

            if (!Directory.Exists(startingPath))
                Directory.CreateDirectory(startingPath);

            File.WriteAllBytes(Path.Combine(startingPath, filename), bytes);
        }
    }
}
