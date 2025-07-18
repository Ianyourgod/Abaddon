using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionButton : MonoBehaviour
{
    [SerializeField]
    private float lerpSpeed = 0.05f;

    private Vector2 onScreenPosition;
    private Vector2 offScreenPosition;
    private bool shouldBeOnScreen = false;

    void Start()
    {
        // image.enabled = false;
        onScreenPosition = transform.localPosition;
        offScreenPosition = onScreenPosition + new Vector2(0, -100);
        transform.localPosition = offScreenPosition;
    }

    void Update()
    {
        if (Controller.main == null)
            return;

        shouldBeOnScreen = Controller.main.enabled && Controller.main.ShouldShowInteractionButton();

        if (shouldBeOnScreen)
        {
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,
                onScreenPosition,
                lerpSpeed
            );
        }
        else
        {
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,
                offScreenPosition,
                lerpSpeed
            );
        }
    }
}
