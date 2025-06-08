using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Transform targetPosition;
    [SerializeField] float timeToEnter;
    [SerializeField, Range(0, 1)] float pausedDarknessLevel;


    private Vector3 startingPosition;
    private enum PauseState { Paused, Unpaused, TravelingToPause, TravelingToUnpause }
    private PauseState pauseState = PauseState.Unpaused;

    private void Awake()
    {
        startingPosition = transform.position;
    }

    public void Pause()
    {
        pauseState = PauseState.TravelingToPause;
        UIStateManager.singleton.FadeInDarkener(timeToEnter, pausedDarknessLevel);
        Controller.main.enabled = false;
    }

    public void Unpause()
    {
        pauseState = PauseState.TravelingToUnpause;
        UIStateManager.singleton.FadeOutDarkener(timeToEnter);
        Controller.main.enabled = true;
    }

    void Update()
    {
        if (pauseState == PauseState.TravelingToUnpause)
        {
            Vector3 endingPosition = startingPosition;
            transform.position = Vector3.Lerp(transform.position, endingPosition, timeToEnter * Time.deltaTime);

            if (Vector2.Distance(transform.position, endingPosition) <= 1f)
            {
                pauseState = PauseState.Unpaused;
                transform.position = new Vector3(endingPosition.x, endingPosition.y, transform.position.z);
            }
        }

        if (pauseState == PauseState.TravelingToPause)
        {
            Vector3 endingPosition = targetPosition.position;
            transform.position = Vector3.Lerp(transform.position, endingPosition, timeToEnter * Time.deltaTime);

            if (Vector2.Distance(transform.position, endingPosition) <= 1f)
            {
                pauseState = PauseState.Paused;
                transform.position = new Vector3(endingPosition.x, endingPosition.y, transform.position.z);
            }
        }
    }
}