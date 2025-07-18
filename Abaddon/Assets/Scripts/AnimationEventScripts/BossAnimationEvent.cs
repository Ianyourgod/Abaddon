using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationEvent : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    public void FinishAnimation()
    {
        animator.Play("BOSS1_animation_idle");
    }
}
