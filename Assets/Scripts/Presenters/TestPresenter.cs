using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReactUnity;
using ReactUnity.Services;
using Coach;
using System;
using Presenters;

public class TestPresenter : Presenter<TestController, TestModel>
{
    public Text Prediction;
    public DeviceCameraController CameraController;

    protected override void Render(TestModel viewModel)
    {
        if (viewModel.result.Label != null)
        {
            var result = $"{viewModel.result.Label}: {Math.Round(viewModel.result.Confidence * 100, 2)}%";
            Debug.LogWarning(result);
            Prediction.text = result;
        }
    }

    private void Update()
    {
        var photo = CameraController.GetWebcamPhoto();
        Controller.Predict(photo);
    }

    public void GoBack()
    {
        CameraController.Dispose();
        Controller.GoBack();
    }
}

public class TestController : Controller<TestModel>
{
    public ISceneService _sceneService;
    public ICoachService _coachService;

    TestController(ISceneService sceneService, ICoachService coachService)
    {
        _sceneService = sceneService;
        _coachService = coachService;
    }

    public override void Start()
    {
        SetState(
            _sceneService.GetState<TestModel>()
        );
    }

    public IEnumerator PredictAsync(Texture2D photo)
    {
        if (State.model != null)
            yield return State.model.PredictAsync(photo);
    }

    public void Predict(Texture2D photo)
    {
        var r = State.model.Predict(photo, true);
        SetState(new TestModel()
        {
            model = State.model,
            result = r.Best()
        });
    }

    public void GetResultsAsync()
    {
        if (State.model != null)
        {
            var results = State.model.GetPredictionResultAsync(true);
            if (results != null)
            {
                var best = results.Best();
                SetState(new TestModel()
                {
                    model = State.model,
                    result = best
                });
            }
        }
    }

    public void GoBack()
    {
        State.model.CleanUp();
        _sceneService.GoBack();

        var listPresenter = GameObject.FindGameObjectWithTag("Presenter").GetComponent<ListPresenter>();
        listPresenter.OnRefocus();
    }
}

public class TestModel : IModel
{
    public CoachModel model;
    public LabelProbability result;
}