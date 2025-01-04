using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] float lerpSpeed = 0.1f;
    [SerializeField] float targetFPS = 60;

    private bool panning = false;
    private Vector2 panTarget;
    private float stayTime = 0.5f;
    private float panSpeed = 0.05f;

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
        panTarget = target;
        panSpeed = speed;
        stayTime = stay;
        panning = true;
        Controller.main.done_with_tick = false;
    }

    private void PanInternal() {
        Vector2 cameraPosition = transform.position;

        Vector2 newPosition = Vector2.Lerp(cameraPosition, panTarget, panSpeed * (Time.deltaTime * targetFPS));

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