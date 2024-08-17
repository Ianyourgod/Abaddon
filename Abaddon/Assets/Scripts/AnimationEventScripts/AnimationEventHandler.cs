using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Analytics;
using JetBrains.Annotations;

[RequireComponent(typeof(Animator))]
public class AnimationEventHandler : MonoBehaviour
{
    public class Animation {
        public string animation;
        public int priority;
        public bool shouldInterupt;
        public bool shouldLoop;

        public Animation(string animation, int priority = 0, bool shouldInterupt = false, bool shouldLoop = false) {
            this.animation = animation;
            this.priority = priority;
            this.shouldInterupt = shouldInterupt;
            this.shouldLoop = shouldLoop;
        }

        public static implicit operator string(Animation queuedAnimation) {
            return queuedAnimation.animation;
        }
    }

    protected Animator animator;
    public static Dictionary<int, string> hashesToString = new Dictionary<int, string>();
    public Animation currentAnimation; 
    public AnimationHandlerData data;
    private List<Animation> queuedAnimations = new List<Animation>();

    public Animation defaultAnimationAction { get; private set; } = new Animation("Goblin_animation_front_idle", 0, false, true);

    protected void Awake() {
        animator = GetComponent<Animator>();
    }

    public static bool TryHashToString(int hash, out string name)
    {
        if (hashesToString.ContainsKey(hash))
        {
            name = hashesToString[hash];
            return true;
        }
        name = "Unknown";
        return false;
    }

    private void PlayAnimation(Animation animation)
    {
        if (currentAnimation == animation) return;

        print("Playing animation: " + animation + " at time: " + Time.time);
        currentAnimation = animation;
        data.GetStartActions(animation)?.Invoke();
        animator.Play(animation);
    }

    private void PlayNextAnimation() {
        print("# queued animations: " + queuedAnimations.Count);
        foreach (Animation queuedAnimation in queuedAnimations.OrderBy(x => x.priority)) {
            print("queued animation: " + queuedAnimation);
            if (queuedAnimation.shouldInterupt || currentAnimation == null) {
                PlayAnimation(queuedAnimation);
                queuedAnimations.Clear();
                return;
            }
        }

        if (currentAnimation == null) {
            print("playing default animation");
            PlayAnimation(defaultAnimationAction);
        }
    }

    public void QueueAnimation(Animation action, Vector2 direction)
    {
        if (action == "death") action.animation = $"{data.animation_prefix}_animation_death";
        else action.animation = $"{data.animation_prefix}_animation_{direction.DirectionToString()}_{action.animation}";
        
        queuedAnimations.Add(new Animation(
            action.animation,
            action.priority,
            action.shouldInterupt,
            action.shouldLoop
        ));
    }

    private void Update()
    {
        var animationInfo = animator.GetCurrentAnimatorStateInfo(0);
        // print("normalized time: " + animationInfo.normalizedTime);
        if (animationInfo.normalizedTime >= 1) {
            if (!currentAnimation.shouldLoop) {
                print($"done with {currentAnimation.animation} animation at time {Time.time}");
                data.GetEndActions(currentAnimation)?.Invoke();
                currentAnimation = null;
            }
        }

        PlayNextAnimation();
    }
}