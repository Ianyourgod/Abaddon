using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeInDeathScreen : MonoBehaviour
{
    [SerializeField] Transform deathScreenaParent;
    [SerializeField] UnityEngine.UI.Image deathScreenImage;
    [SerializeField] UnityEngine.UI.Image restartButtonImage;
    [SerializeField] UnityEngine.UI.Image darkener;
    [SerializeField] float timeToFadeIn = 2f;
    [SerializeField] float timeToFadeOut = 2f;

    private float startTimeOfFadeIn;
    private float startTimeOfFadeOut;

    private void Start()
    {
        Controller.main.onDie += OnDie;
        deathScreenaParent.gameObject.SetActive(false);
    }

    void Update() {
        if (startTimeOfFadeIn > 0) {
            deathScreenaParent.gameObject.SetActive(true);
            float t = (Time.time - startTimeOfFadeIn) / timeToFadeIn;
            deathScreenImage.color = new Color(1, 1, 1, t);
            restartButtonImage.color = new Color(1, 1, 1, t);
            Controller.main.enabled = false;
            if (t >= 1) {
                startTimeOfFadeIn = 0;
            }
        }

        else if (startTimeOfFadeOut > 0) {
            float t = 1 - ((Time.time - startTimeOfFadeOut) / timeToFadeOut);
            deathScreenImage.color = new Color(1, 1, 1, t);
            restartButtonImage.color = new Color(1, 1, 1, t);
            if (t <= 0) {
                startTimeOfFadeOut = 0;
                deathScreenaParent.gameObject.SetActive(false);
                Controller.main.enabled = true;
            }
        }
    }
    
    public void OnDie() {
        startTimeOfFadeIn = Time.time;
        Controller.main.enabled = false;
    }

    public void FadeOut() {
        startTimeOfFadeOut = Time.time;
    }
}
