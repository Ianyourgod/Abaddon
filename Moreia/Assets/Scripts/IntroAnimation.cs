using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroAnimation : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 10;
    [SerializeField] Image darkener;
    [SerializeField] Color startingColor;

    void Awake() {
        if (!enabled) enabled = true;
        darkener.color = startingColor;
    }

    private void Update()
    {
        darkener.color = new Color(darkener.color.r, darkener.color.g, darkener.color.b, Mathf.Lerp(darkener.color.a, 0, fadeSpeed * Time.deltaTime));
        Debug.Log(name);
        if (darkener.color.a <= 0.1f) {
            Destroy(this, 0.1f);
        }
    }
}
