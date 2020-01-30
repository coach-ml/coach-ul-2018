using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using UnityEngine;

namespace ReactUnity.Services
{
    public interface IFirebaseService : IService
    {
        FirebaseUser User { get; set; }
        void Initialize(EventHandler authStateChanged);
        Task<FirebaseUser> SignIn(string email, string password);
        void NewModel(string modelName);
        Task UpdateModel(string modelName, int maxSamples, int sampleUploadProgress);
        Task UpdateModel(string modelName);
        void Logout();

        Task<CoachUser> GetUserDetails();
        void WatchModels(EventHandler<ValueChangedEventArgs> onChange);
        Task<Dictionary<string, UploadStruct>> GetModels();
        Task<Dictionary<string, BuiltModel>> GetBuiltModels();
    }
    public class FirebaseService : IFirebaseService
    {
        private FirebaseAuth Auth;
        public FirebaseUser User { get; set; }
        private DatabaseReference BaseRef;

        private event EventHandler AuthStateChanged;

        public void Initialize(EventHandler authStateChanged)
        {
            AuthStateChanged = authStateChanged;

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://coachuploader-ac4d7.firebaseio.com/");
                    BaseRef = FirebaseDatabase.DefaultInstance.RootReference;

                    Auth = FirebaseAuth.DefaultInstance;
                    Auth.StateChanged += _AuthStateChanged;
                    _AuthStateChanged(this, null);

                    Debug.LogWarning("Started Firebase");
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

        void _AuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (Auth.CurrentUser != User)
            {
                bool signedIn = User != Auth.CurrentUser && Auth.CurrentUser != null;
                if (!signedIn && User != null)
                {
                    Debug.Log("Signed out " + User.UserId);
                }
                User = Auth.CurrentUser;
                if (signedIn)
                {
                    AuthStateChanged(sender, eventArgs);
                }
            }
        }

        public async Task<CoachUser> GetUserDetails()
        {
            CoachUser result = null;
            try
            {
                var snapshot = await BaseRef.Child(User.UserId).Child("user").GetValueAsync();
                result = JsonUtility.FromJson<CoachUser>(snapshot.GetRawJsonValue());
            }
            catch (Exception ex)
            {
                // Handle the error...
            }
            return result;
        }

        public System.Threading.Tasks.Task<FirebaseUser> SignIn(string email, string password)
        {
            return Auth.SignInWithEmailAndPasswordAsync(email, password);
        }

        public void NewModel(string modelName)
        {
            var json = JsonUtility.ToJson(new UploadStruct()
            {
                modelName = modelName
            });
            var reff = BaseRef.Child(User.UserId).Child("uploads").Child(modelName);
            reff.SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception.Message);
                }
            });
        }

        public async Task UpdateModel(string modelName, int maxSamples, int sampleUploadProgress)
        {
            var json = JsonUtility.ToJson(new UploadStruct() {
                modelName = modelName,
                maxSamples = maxSamples,
                sampleUploadProgress = sampleUploadProgress
            });
            await BaseRef.Child(User.UserId).Child("uploads").Child(modelName).SetRawJsonValueAsync(json);
        }

        public async Task UpdateModel(string modelName)
        {
            var currentModel = await GetModel(modelName);

            var json = JsonUtility.ToJson(new UploadStruct()
            {
                modelName = currentModel.modelName,
                maxSamples = currentModel.maxSamples,
                sampleUploadProgress = currentModel.sampleUploadProgress + 1
            });
            await BaseRef.Child(User.UserId).Child("uploads").Child(modelName).SetRawJsonValueAsync(json);
        }

        public void WatchModels(EventHandler<ValueChangedEventArgs> onChange)
        {
            BaseRef.Child(User.UserId).Child("uploads").ValueChanged += onChange;
        }

        public async Task<UploadStruct> GetModel(string modelName)
        {
            UploadStruct result = null;
            try
            {
                var snapshot = await BaseRef.Child(User.UserId).Child("uploads").Child(modelName).GetValueAsync();
                if (snapshot != null)
                    result = JsonConvert.DeserializeObject<UploadStruct>(snapshot.GetRawJsonValue());
            }
            catch (Exception ex)
            {
                // Handle the error...
            }
            return result;
        }

        public async Task<Dictionary<string, UploadStruct>> GetModels()
        {
            Dictionary<string, UploadStruct> result = null;
            try
            {
                var snapshot = await BaseRef.Child(User.UserId).Child("uploads").GetValueAsync();
                if (snapshot != null)
                    result = JsonConvert.DeserializeObject<Dictionary<string, UploadStruct>>(snapshot.GetRawJsonValue());
            }
            catch (Exception ex)
            {
                // Handle the error...
            }
            return result;
        }

        public async Task<Dictionary<string, BuiltModel>> GetBuiltModels()
        {
            Dictionary<string, BuiltModel> result = null;
            try
            {
                var snapshot = await BaseRef.Child(User.UserId).Child("builtModels").GetValueAsync();
                result = JsonConvert.DeserializeObject<Dictionary<string, BuiltModel>>(snapshot.GetRawJsonValue());
            }
            catch (Exception ex)
            {
                // Handle the error...
            }
            return result;
        }

        public void Logout()
        {
            Auth.SignOut();
        }
    }

    public class ModelParent
    {
        public string modelName;
        public UploadStruct modelInfo;
    }

    public class UploadStruct
    {
        public string modelName;
        public int maxSamples;
        public int sampleUploadProgress;
    }

    public class BuiltModel
    {
        public bool modelPassed;
    }

    public class CoachUser
    {
        public string s3Key;
        public string coachApi;
        public string s3Secret;
        public string bucket;
        public string clientName;
    }

}