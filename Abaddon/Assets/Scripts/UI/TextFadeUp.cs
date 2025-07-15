using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFadeUp : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    public float minimum = 0f;
    public float maximum = 1f;
    public float speed = 0.5f;
    public float timeLimit = 2f;

    [HideInInspector]
    public float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        minimum += transform.position.y;
        maximum += transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(minimum, maximum, t), 0);
        GetComponent<SpriteRenderer>().color = new Color(
            GetComponent<SpriteRenderer>().color.r,
            GetComponent<SpriteRenderer>().color.g,
            GetComponent<SpriteRenderer>().color.b,
            Mathf.Lerp(1f, 0f, t)
        );

        t += 0.5f * Time.deltaTime;
        if (t > timeLimit)
        {
            Destroy(gameObject);
        }
    }
}
