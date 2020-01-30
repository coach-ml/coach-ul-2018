using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    
    private InputField Input { get; set; }
    private Outline Outline { get; set; }

    public Text Dismiss;

    public void Start()
    {
        Input = gameObject.GetComponentInChildren<InputField>();
        Outline = gameObject.GetComponentInChildren<Outline>();

        Input.onValueChanged.AddListener((s) =>
        {
            Dismiss.gameObject.SetActive(s != "");
        });
    }

    public void Hide()
    {
        if (Input.text == "")
            Outline.effectColor = new Color(255, 0, 0, 255);
        else
            gameObject.SetActive(false);
    }
}
