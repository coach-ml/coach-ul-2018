using Controllers;
using Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ReactUnity;

namespace Presenters
{
    // have to pass the ListModel to the presenter again.. TODO
    public class ListPresenter : Presenter<ListController, ListModel>
    {
        // Simple list text for displaying our items
        public GameObject buttonPrefab;
        public GameObject modalParent;
        public GameObject testPanel;
        public GameObject uploadPanel;
        public Text noModelsWarning;
        public Text PanelTitle;

        protected override void Render(ListModel state)
        {
            noModelsWarning.enabled = state.Models?.Count == 0;

            if (state.Models?.Count > 0)
            {
                AddButtons(state.Models);
            }
        }

        public override void OnRefocus()
        {
            Controller.OnFocus();
        }

        public void ShowUploadPanel()
        {
            this.uploadPanel.SetActive(true);
            this.testPanel.SetActive(false);
            this.PanelTitle.text = "Coach Uploads";
        }

        public void ShowTestPanel()
        {
            this.uploadPanel.SetActive(false);
            this.testPanel.SetActive(true);
            this.PanelTitle.text = "Coach Models";
        }

        public RectTransform Container;
        private void AddButtons(List<ItemModel> itemList)
        {
            foreach (Transform child in Container.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                var model = itemList[i];

                GameObject newButton = Instantiate(buttonPrefab);
                newButton.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    DownloadAndPlay(model);
                });
                newButton.GetComponentInChildren<Text>().text = model.Name;
                newButton.transform.SetParent(Container, false);
            }
        }
        
        public async void DownloadAndPlay(ItemModel itemModel)
        {
            var model = await Controller.DownloadModel(itemModel.Name);
            Controller._sceneService.LoadScene<Scenes.TestScene>(new TestModel()
            {
                model = model
            }, LoadSceneMode.Additive);
        }

        public void ShowTrainer()
        {
            Controller._sceneService
                .LoadScene<Scenes.TrainerScene>(new TrainerModel()
                {
                    SampleCount = 0,
                    Subject = ""
                }, LoadSceneMode.Additive);
        }

        public void Logout()
        {
            Controller.Logout();
        }

        private int Counter = 0;
        public void FixedUpdate()
        {
            /*
            Counter++;
            if (Counter % 200 == 0)
                Controller.GetModels();
            */
        }
    }
}