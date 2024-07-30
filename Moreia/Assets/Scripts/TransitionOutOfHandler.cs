using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class TransitionOutOfHandler : MonoBehaviour
{
    [SerializeField] TransistionScriptableObject transistionScriptableObject;
    [SerializeField] UnityEngine.UI.Image panel;
    private float timeElapsed = 0;

    void Start() {
        enabled = true;
        panel.enabled = true;
        panel.gameObject.SetActive(true);
    }

    void Update()
    {
        Debug.Log("Starting");
        Color start = transistionScriptableObject.transistionColor;
        Color end = new Color(start.r, start.g, start.b, 0);
        float t = timeElapsed / transistionScriptableObject.timeToFade;
        panel.color = Color.Lerp(start, end, t);
        print("frame");
        
        if (panel.color.a <= 0.05f) {
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
            Destroy(this);
            print("done");
        }
        timeElapsed += Time.deltaTime;
    }
}
