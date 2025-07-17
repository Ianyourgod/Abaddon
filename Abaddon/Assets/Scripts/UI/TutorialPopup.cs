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
        onScreenPosition = transform.position;
        offScreenPosition = onScreenPosition + new Vector2(0, 150);
        transform.position = offScreenPosition;
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
            transform.position = Vector2.Lerp(transform.position, onScreenPosition, lerpSpeed);
        }
        else
        {
            transform.position = Vector2.Lerp(transform.position, offScreenPosition, lerpSpeed);
        }
    }
}
