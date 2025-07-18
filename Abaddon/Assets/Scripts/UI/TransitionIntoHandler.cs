using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionIntoHandler : MonoBehaviour
{
    [SerializeField]
    TransistionScriptableObject transistionScriptableObject;

    [SerializeField]
    UnityEngine.UI.Image panel;

    [SerializeField]
    string StartPlayScene = "Level 0";
    private bool startFadingOut;
    private float timeElapsed;

    void Update()
    {
        if (startFadingOut)
        {
            Color end = transistionScriptableObject.transistionColor;
            Color start = new Color(end.r, end.g, end.b, 0);
            float t = timeElapsed / transistionScriptableObject.timeToFade;
            panel.color = new Color(start.r, start.g, start.b, t);
            timeElapsed += Time.deltaTime;

            if (panel.color.a > 0.95f)
            {
                SceneManager.LoadScene(StartPlayScene);
            }
        }
    }

    public void SwitchScene()
    {
        startFadingOut = true;
        panel.gameObject.SetActive(true);
        panel.enabled = true;
    }

    public void SwitchSceneToTutorial()
    {
        StartPlayScene = "Real Level copy";
        SwitchScene();
    }

    public void SwitchSceneToCredits()
    {
        StartPlayScene = "Credits";
        SwitchScene();
    }

    public void SwitchSceneToMainMenu()
    {
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        StartPlayScene = "Main Menu";
        SwitchScene();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
