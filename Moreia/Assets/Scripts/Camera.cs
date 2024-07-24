using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] float lerpSpeed = 0.1f;

    void Update() {
        if (!Controller.main) return;

        Vector2 playerPosition = Controller.main.transform.position;
        Vector2 cameraPosition = transform.position;

        Vector2 newPosition = Lerp(cameraPosition, playerPosition, lerpSpeed);

        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    float Lerp(float a, float b, float t) {
        return a + (b - a) * t;
    }

    Vector2 Lerp(Vector2 a, Vector2 b, float t) {
        return new Vector2(Lerp(a.x, b.x, t), Lerp(a.y, b.y, t));
    }
}
