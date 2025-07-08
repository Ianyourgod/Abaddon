using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Video;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform defaultFollowTarget;
    [SerializeField] private float default_seconds;
    [SerializeField] private float default_wait_time;
    [SerializeField] private float default_lerp_speed = 0.25f;
    [SerializeField] private bool default_useSmoothMovement = true;
    [SerializeField] private AnimationCurve camera_moveto_curve;
    private bool useSmoothMovement = true;
    private float seconds;
    private float wait_time = 0;
    private Transform currentTarget;
    private Action onComplete;
    private float current_movement_time = 0;
    private float current_wait_time = 0;
    private Vector2 start_position;
    private readonly float thresholdDistance = 0.01f;
    private CameraState currentState = CameraState.MovingToTarget;
    private enum CameraState
    {
        MovingToTarget,
        Waiting,
        FollowingTarget
    }

    private Action onWidenFOVComplete;
    private float target_fov = 5;
    private float initial_fov = 5;
    private float timeSpentWideningFOV = 0;
    private float timeToWidenFOV = 0;

    void Awake()
    {
        currentTarget = defaultFollowTarget;
        seconds = default_seconds;
        initial_fov = Camera.main.orthographicSize;
    }

    Vector2 CalcNextPosition()
    {
        float time = current_movement_time / seconds;
        if (time > 1)
        {
            time = 1;
        }
        if (float.IsNaN(time))
        {
            time = 0;
        }
        current_movement_time += Time.deltaTime;
        if (useSmoothMovement)
        {
            time = camera_moveto_curve.Evaluate(time);
            if (currentTarget != null)
                return Vector3.Lerp(start_position, currentTarget.position + new Vector3(0, 0, -10), time);
            else
                return transform.position;
        }
        return Vector3.Lerp(transform.position, currentTarget.position + new Vector3(0, 0, -10), default_lerp_speed * Time.deltaTime * 60f);
    }

    void Update()
    {
        if (Controller.main == null) return;

        void onReachedTarget()
        {
            currentState = CameraState.FollowingTarget;
            onComplete?.Invoke();
        }

        if (timeToWidenFOV > 0)
        {
            timeSpentWideningFOV += Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Lerp(initial_fov, target_fov, timeSpentWideningFOV / timeToWidenFOV);
            if (timeSpentWideningFOV >= timeToWidenFOV)
            {
                timeToWidenFOV = 0;
                timeSpentWideningFOV = 0;
                Camera.main.orthographicSize = target_fov;
                onWidenFOVComplete?.Invoke();
            }
        }

        Vector3 newPosition = CalcNextPosition();
        switch (currentState)
        {
            case CameraState.MovingToTarget:
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                if (Vector2.Distance(transform.position, currentTarget.position) < thresholdDistance)
                {
                    if (wait_time > 0)
                    {
                        currentState = CameraState.Waiting;
                        current_wait_time = 0;
                    }
                    else onReachedTarget();
                }
                break;
            case CameraState.Waiting:
                current_wait_time += Time.deltaTime;
                if (current_wait_time > wait_time) onReachedTarget();
                break;
            case CameraState.FollowingTarget:
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                break;
        }
    }

    public void ChangeTarget(Transform target, float seconds, float wait_time = 0, bool useSmoothMovement = true, System.Action onComplete = null)
    {
        this.seconds = seconds;
        this.wait_time = wait_time;
        this.onComplete = onComplete;
        this.useSmoothMovement = useSmoothMovement;
        currentTarget = target;

        currentState = CameraState.MovingToTarget;
        start_position = transform.position;
        current_movement_time = 0;
        current_movement_time = 0;

    }

    public void ResetTarget(float? seconds = null, bool? useSmoothMovement = null, System.Action onComplete = null) => ChangeTarget(defaultFollowTarget, seconds ?? default_seconds, 0, useSmoothMovement ?? default_useSmoothMovement, onComplete);

    public void UpdateFOV(float size, float seconds = 0, Action onComplete = null)
    {
        target_fov = size;
        timeToWidenFOV = seconds;
        initial_fov = Camera.main.orthographicSize;
        onWidenFOVComplete = onComplete;
    }
}