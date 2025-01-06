using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] float lerpSpeed = 0.1f;
    [SerializeField] float targetFPS = 60;

    private bool panning = false;
    private Vector2 panStart;
    private Vector2 panTarget;
    private float stayTime = 0.5f;
    private float panSpeed = 0.05f;
    private bool smooth = false;
    private float time = 1.0f;
    private float current_time = 0.0f;

    void Update() {
        if (Controller.main == null) return;

        if (panning) {
            PanInternal();
            return;
        }

        Vector2 playerPosition = Controller.main.transform.position;
        Vector2 cameraPosition = transform.position;

        Vector2 newPosition = Vector2.Lerp(cameraPosition, playerPosition, lerpSpeed * (Time.deltaTime * targetFPS));

        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    public void PanTo(Vector2 target, float speed, float stay) {
        panStart = transform.position;
        panTarget = target;
        panSpeed = speed;
        stayTime = stay;
        smooth = false;
        panning = true;
        Controller.main.done_with_tick = false;
    }

    public void PanToSmooth(Vector2 target, float time_, float stay) {
        panStart = transform.position;
        panTarget = target;
        time = time_;
        current_time = 0.0f;
        stayTime = stay;
        smooth = true;
        panning = true;
        Controller.main.done_with_tick = false;
    }

    private float sigmoid(float t, float steepness) {
        return 1 / (1 + Mathf.Exp(-steepness * (t - 0.5f)));
    }

    private Vector2 smoothLerp(Vector2 a, Vector2 b, float t, float steepness = 10) {
        return Vector2.Lerp(a, b, sigmoid(t, steepness));
    }

    private void PanInternal() {
        Vector2 cameraPosition = transform.position;

        Vector2 newPosition;

        if (smooth) {
            current_time += Time.deltaTime;

            newPosition = smoothLerp(panStart, panTarget, current_time / time);
        } else {
            newPosition = Vector2.Lerp(cameraPosition, panTarget, panSpeed * (Time.deltaTime * targetFPS));
        }

        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

        if (Vector2.Distance(newPosition, panTarget) < 0.5f) {
            stayTime -= Time.deltaTime;
            if (stayTime <= 0) {
                panning = false;
                Controller.main.done_with_tick = true;
            }
        }
    }
}