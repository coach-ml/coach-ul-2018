using ReactUnity;
using Controllers;
using Models;
using UnityEngine;
using UnityEngine.UI;

namespace Presenters
{
    class ItemPresenter : Presenter<ItemController, ItemModel>
    {
        public Text ModelName;
        public Image ModelPreview;
        public Button DownloadPlay;

        protected override void Render(ItemModel state)
        {
            if (state != null)
            {
                ModelName.text = state.Name;

                // Important to call this before adding a listener otherwise we can have multiple listeners on 1 button
                DownloadPlay.onClick.RemoveAllListeners();
                DownloadPlay.onClick.AddListener(() =>
                {
                    Controller.DownloadAndPlay(() => Close());
                });
            }
        }

        public void Close()
        {
            Destroy(this.gameObject);
        }
    }
}