using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField]
    private float lerpSpeed = 0.05f;

    private Vector2 onScreenPosition;
    private Vector2 offScreenPosition;
    protected bool shouldBeOnScreen = false;

    protected void Start()
    {
        // image.enabled = false;
        onScreenPosition = transform.localPosition;
        offScreenPosition = onScreenPosition + new Vector2(0, 200);
        transform.localPosition = offScreenPosition;
    }

    public void Enable()
    {
        shouldBeOnScreen = true;
    }

    public void Disable()
    {
        shouldBeOnScreen = false;
    }

    protected void Update()
    {
        if (shouldBeOnScreen)
        {
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,
                onScreenPosition,
                lerpSpeed * Time.deltaTime * 60f
            );
        }
        else
        {
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,
                offScreenPosition,
                lerpSpeed * Time.deltaTime * 60f
            );
        }
    }
}
