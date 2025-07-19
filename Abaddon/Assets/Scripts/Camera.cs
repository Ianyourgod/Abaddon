using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] float lerpSpeed = 0.1f;

    void Update() {
        if (Controller.main == null) return;

        Vector2 playerPosition = Controller.main.transform.position;
        Vector2 cameraPosition = transform.position;

        Vector2 newPosition = Vector2.Lerp(cameraPosition, playerPosition, lerpSpeed);

        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}