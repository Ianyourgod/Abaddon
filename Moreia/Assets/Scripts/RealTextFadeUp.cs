using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RealTextFadeUp : MonoBehaviour
{
    [SerializeField] public TMP_Text textObject;

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
        GetComponent<TMP_Text>().color = new Color(GetComponent<TMP_Text>().color.r, GetComponent<TMP_Text>().color.g, GetComponent<TMP_Text>().color.b, Mathf.Lerp(1f, 0f, t));

        t += 0.5f * Time.deltaTime;
        if (t > timeLimit)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string set_text) {
        textObject.text = set_text;
    }
}
