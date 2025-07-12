using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    Transform targetPosition;

    [SerializeField]
    float timeToEnter;

    [SerializeField, Range(0, 1)]
    float pausedDarknessLevel;

    private Vector3 startingPosition;

    private enum PauseState
    {
        Paused,
        Unpaused,
        TravelingToPause,
        TravelingToUnpause,
    }

    private PauseState pauseState = PauseState.Unpaused;

    private void Awake()
    {
        startingPosition = transform.position;
    }

    public void Pause()
    {
        if (Controller.main == null)
            return;

        pauseState = PauseState.TravelingToPause;
        UIStateManager.singleton.FadeInDarkener(timeToEnter, pausedDarknessLevel);
        Controller.main.enabled = false;
    }

    public void Unpause()
    {
        if (Controller.main == null)
            return;

        pauseState = PauseState.TravelingToUnpause;
        UIStateManager.singleton.FadeOutDarkener(timeToEnter);
        Controller.main.enabled = true;
    }

    void Update()
    {
        Vector3 endingPosition;
        bool going_to_paused = false;
        if (pauseState == PauseState.TravelingToUnpause)
        {
            endingPosition = startingPosition;
        }
        else if (pauseState == PauseState.TravelingToPause)
        {
            endingPosition = targetPosition.position;
            going_to_paused = true;
        }
        else
        {
            return;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            endingPosition,
            timeToEnter * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, endingPosition) <= 1f)
        {
            pauseState = going_to_paused ? PauseState.Paused : PauseState.Unpaused;
            transform.position = new Vector3(
                endingPosition.x,
                endingPosition.y,
                transform.position.z
            );
        }
    }
}
