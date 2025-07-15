using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RealTextFadeUp : MonoBehaviour
{
    [SerializeField]
    public TMP_Text textObject;

    public float minimum = 0f;
    public float maximum = 1f;
    public float speed = 0.5f;
    public float timeLimit = 2f;

    [HideInInspector]
    public float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        minimum += transform.localPosition.y;
        maximum += transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!textObject)
            return;

        float normalizedTime = Mathf.Clamp01(t / timeLimit);

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            Mathf.Lerp(minimum, maximum, normalizedTime),
            transform.localPosition.z
        );
        textObject.color = new Color(
            textObject.color.r,
            textObject.color.g,
            textObject.color.b,
            Mathf.Lerp(1f, 0f, normalizedTime)
        );

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

    public void SetFontSize(int size)
    {
        textObject.fontSize = size;
    }
}
