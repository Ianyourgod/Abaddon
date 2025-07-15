using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transistion", menuName = "New Transistion", order = 1)]
public class TransistionScriptableObject : ScriptableObject
{
    public TransitionType transitionType = TransitionType.Fade;
    public Color transistionColor = new Color(0, 0, 0, 1);
    public float timeDelay = 0f;
    public float timeToFade = 1f;

    public enum TransitionType
    {
        Fade,
        Swipe_Left,
        Swipe_Right,
    }
}
