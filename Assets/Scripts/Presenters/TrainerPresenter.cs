using Controllers;
using Models;
using UnityEngine;
using System;
using Utilities;
using UnityEngine.UI;
using ReactUnity;
using System.Net;

namespace Presenters
{
    class TrainerPresenter : Presenter<TrainerController, TrainerModel>
    {
        public DeviceCameraController CameraController;

        public InputField NameField;
        public Text ScanHeading;

        public Text Dismiss;

        private Outline Outline;
        public GameObject Tutorial;
        public GameObject TopPanel;
        public GameObject TapDismiss;

        private bool shutterDown { get; set; }

        protected override void Render(TrainerModel state)
        {
            if (state.Subject != "")
            {
                ScanHeading.enabled = true;
                ScanHeading.text = $"{state.Subject}: {state.SampleCount}";
            }
        }

        private void Awake()
        {
            NameField.onValueChanged.AddListener((s) =>
            {
                Dismiss.gameObject.SetActive(s != "");
            });
            ScanHeading.enabled = false;
            Outline = GetComponentInChildren<Outline>();
        }

        public void ShutterUp()
        {
            shutterDown = false;
        }

        public void ShutterDown()
        {
            shutterDown = true;
        }

        public void Update()
        {
            if (shutterDown)
                TakePhoto();
        }

        public void TakePhoto()
        {
            if (NameField.text != "")
            {
                var photo = CameraController.GetWebcamPhoto();
                byte[] bytes = photo.EncodeToJPG();
                UnityEngine.Object.Destroy(photo);

                Controller.TakePhoto(bytes);
            }
        }

        public void GoBack()
        {
            Controller.FinishedCapture();
            CameraController.Dispose();

            var listPresenter = GameObject.FindGameObjectWithTag("Presenter").GetComponent<ListPresenter>();
            listPresenter.OnRefocus();
        }

        protected override void OnPresenterDestroy()
        {
            CameraController.Dispose();
        }

        public void HideTutorial()
        {
            if (NameField.text == "")
                Outline.effectColor = new Color(255, 0, 0, 255);
            else
            {
                TopPanel.SetActive(true);
                Tutorial.SetActive(false);
                Controller.SetSubject(NameField.text);
            }
        }

        public void ShowDismiss()
        {
            TapDismiss.SetActive(true);
        }
    }
}
