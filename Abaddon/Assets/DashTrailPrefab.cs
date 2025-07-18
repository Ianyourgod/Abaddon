using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrailPrefab : MonoBehaviour
{
    public Vector2 direction = Vector2.zero;
    public float scaler = 1f;
    public float lifetime = 0.2f;
    private float timeAlive = 0f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position += (Vector3)direction * Time.deltaTime * scaler;
        spriteRenderer.color = new Color(
            235f / 255f,
            18f / 255f,
            18f / 255f,
            Mathf.Lerp(1f, 0f, timeAlive / lifetime)
        );
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
