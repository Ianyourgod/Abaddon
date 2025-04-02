using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] public Transform defaultFollowTarget;
    [SerializeField] public float default_seconds;
    [SerializeField] public float default_wait_time;
    [SerializeField] public float default_lerp_speed=0.25f;
    [SerializeField] AnimationCurve camera_moveto_curve;
    private float seconds;
    private float wait_time = 0;
    private Transform currentTarget;
    private System.Action onComplete;
    private bool is_lerp_mode = true;
    private float current_movement_time = 0;
    private float current_wait_time = 0;
    private Vector2 start_position;

    void Awake() {
        currentTarget = defaultFollowTarget;
        seconds = 0;
    }

    Vector2 CalcNextPosition()
    {
        float time = current_movement_time / seconds;
        if (time > 1) {
            time = 1;
        }
        if (float.IsNaN(time)) {
            time = 0;
        }
        current_movement_time += Time.deltaTime;
        if (!is_lerp_mode) {
            time = camera_moveto_curve.Evaluate(time);
            return Vector3.Lerp(start_position, currentTarget.position + new Vector3(0, 0, -10), time);
        }
        return Vector3.Lerp(transform.position, currentTarget.position + new Vector3(0, 0, -10), default_lerp_speed);
    }

    void Update()
    {
        if (Controller.main == null) return;
        //print("panning to " + currentTarget.gameObject.name);
        Vector3 newPosition = CalcNextPosition();
        if ((Vector2.Distance(transform.position, currentTarget.position) < 0.5f && is_lerp_mode) ||
           (current_movement_time > seconds && !is_lerp_mode)) {
            if (current_wait_time < wait_time) {
                current_wait_time += Time.deltaTime;
            } else {
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                onComplete?.Invoke();
                onComplete = null;
                ResetTarget();
            }
        } else {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }

    public void PanToPoint(Vector2 point, float seconds, float wait_time, bool is_lerp_mode=true)
    {
        this.is_lerp_mode = is_lerp_mode;
        defaultFollowTarget = Instantiate(new GameObject("Follow Target").transform);
        defaultFollowTarget.position = point;
        this.seconds = seconds;
        this.wait_time = wait_time;
        current_movement_time = 0;
        current_wait_time = 0;
    }

    public void ChangeTarget(Transform target, float seconds, float wait_time, System.Action onComplete, bool is_lerp_mode=true)
    {
        this.is_lerp_mode = is_lerp_mode;
        current_movement_time = 0;
        currentTarget = target;
        this.seconds = seconds;
        this.wait_time = wait_time;
        this.onComplete = onComplete;
        this.start_position = transform.position;
        current_movement_time = 0;
        current_wait_time = 0;
    }

    public void ResetTarget()
    {
        this.is_lerp_mode = true;
        current_movement_time = 0;
        current_wait_time = 0;
        wait_time = default_wait_time;
        seconds = default_seconds;
        currentTarget = defaultFollowTarget;
        this.start_position = transform.position;
    }
}