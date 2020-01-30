using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coach;
using System.Threading.Tasks;
using System.Linq;

namespace ReactUnity.Services
{
    public interface ICoachService : IService
    {
        Task Initialize(string apiKey);
        Task<List<string>> GetModels();
        Task<CoachModel> GetModel(string modelName);
    }

    public class CoachService : ICoachService
    {
        private CoachClient Client;

        public async Task Initialize(string apiKey)
        {
            Client = await new CoachClient().Login(apiKey);
        }

        public async Task<CoachModel> GetModel(string modelName)
        {
            return await Client.GetModelRemote(modelName, workers: 4);
        }

        public async Task<List<string>> GetModels()
        {
            Profile profile = await Client.GetProfile();
            return profile.models.Select(p => p.name).ToList();
        }
    }
}