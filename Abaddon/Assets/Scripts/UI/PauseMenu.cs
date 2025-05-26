using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Transform targetPosition;
    [SerializeField] float lerpSpeed;
    [SerializeField, Range(0, 1)] float pausedDarknessLevel;


    private Vector3 startingPosition;
    private bool reachedPosition = true;
    private bool paused = false;

    private void OnEnable()
    {
        if (startingPosition == Vector3.zero) startingPosition = transform.position;
        transform.position = TargetPosition();
    }

    private void Awake()
    {
        startingPosition = transform.position;
    }

    public bool IsPaused() => paused;

    public void Pause()
    {
        reachedPosition = false;
        paused = true;
        UIStateManager.singleton.FadeInDarkener(0, pausedDarknessLevel, lerpSpeed);
        // UIStateManager.singleton.SetDarkenedBackground(true);
        // UIStateManager.singleton.darkenerOpacity = 0;
    }

    public void Unpause()
    {
        paused = false;
        reachedPosition = false;
        UIStateManager.singleton.FadeInDarkener(pausedDarknessLevel, 0, lerpSpeed);
    }

    Vector2 TargetPosition() => new Vector3(targetPosition.position.x, targetPosition.position.y, targetPosition.position.z);

    void Update()
    {
        if (!reachedPosition)
        {
            Vector3 endingPosition = TargetPosition();
            transform.position = Vector3.Lerp(transform.position, endingPosition, lerpSpeed * Time.deltaTime);
            // UIStateManager.singleton.darkenerOpacity = Mathf.Lerp(UIStateManager.singleton.darkenerOpacity, pausedDarknessLevel, lerpSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, endingPosition) <= 1f)
            {
                reachedPosition = true;
                transform.position = endingPosition;
                // UIStateManager.singleton.darkenerOpacity = pausedDarknessLevel;
                if (!paused) gameObject.SetActive(false);
                print("reached end point");
            }
        }
    }
}