using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    float timeToEnter = 2f;

    [SerializeField, Range(0, 1)]
    float pausedDarknessLevel;

    public Vector3 onScreenPosition = new Vector3(0, 0, 0);
    public Vector3 offScreenPosition = new Vector3(0, -100, 0);

    public bool shouldBePaused = false;

    void Awake()
    {
        transform.localPosition = offScreenPosition;
    }

    public void Pause()
    {
        if (Controller.main == null)
            return;

        shouldBePaused = true;
        UIStateManager.singleton.FadeInDarkener(timeToEnter, pausedDarknessLevel);
        Controller.main.enabled = false;
    }

    public void Unpause()
    {
        if (Controller.main == null)
            return;

        shouldBePaused = false;
        UIStateManager.singleton.FadeOutDarkener(timeToEnter);
        Controller.main.enabled = true;
    }

    void Update()
    {
        if (shouldBePaused)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                onScreenPosition,
                timeToEnter * Time.deltaTime * 4f
            );
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                offScreenPosition,
                timeToEnter * Time.deltaTime / 8f
            );
        }
    }
}
