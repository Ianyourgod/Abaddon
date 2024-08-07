using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class AnimationStateController : StateMachineBehaviour
{
    // Called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out AnimationEventHandler animationEventHandler))
        {
            animationEventHandler.OnStateEnter(stateInfo.shortNameHash);
        }
    }

    // Called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out AnimationEventHandler animationEventHandler))
        {
            animationEventHandler.OnStateExit(stateInfo.shortNameHash);
        }
    }
}
