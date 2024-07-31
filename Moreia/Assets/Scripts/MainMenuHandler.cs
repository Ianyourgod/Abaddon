using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] string StartPlayScene = "Level1";
    [SerializeField] Image background;
    [SerializeField] float fadeSpeed = 10;
    private bool startFadingOut;

    void Update() {
        if (startFadingOut) {
            background.color = Color.Lerp(background.color, Color.black, fadeSpeed * Time.deltaTime);
            if (background.color.a >= 0.9f) SwitchScene();
        }
    }

    public void SwitchScene() {
        SceneManager.LoadScene(StartPlayScene);
    }
}
