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
        onScreenPosition = transform.position;
        offScreenPosition = onScreenPosition + new Vector2(0, -100);
        transform.position = offScreenPosition;
    }

    void Update()
    {
        if (Controller.main == null)
            return;

        shouldBeOnScreen = Controller.main.enabled && Controller.main.ShouldShowInteractionButton();

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
