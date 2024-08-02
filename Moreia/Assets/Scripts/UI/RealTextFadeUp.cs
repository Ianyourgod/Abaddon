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
        if (!textObject) return;
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(minimum, maximum, t), 0);
        textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, Mathf.Lerp(1f, 0f, t));

        t += 0.5f * Time.deltaTime;
        if (t > timeLimit)
        {
            Destroy(gameObject);
        }
    }

    void SetColor(Color color)
    {
        textObject.color = color;
    }

    void SetBorderColor(Color color, float width)
    {
        textObject.outlineWidth = width;
        textObject.outlineColor = color;
    }

    public void SetText(string set_text, Color color, Color borderColor, float borderWidth)
    {
        SetColor(color);
        SetBorderColor(borderColor, borderWidth);
        textObject.text = set_text;
    }
}
