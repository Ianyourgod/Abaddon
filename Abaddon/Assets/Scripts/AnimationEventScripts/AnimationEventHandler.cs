using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Analytics;

[RequireComponent(typeof(Animator))]
public abstract class AnimationEventHandler : MonoBehaviour
{
    struct QueuedAnimation {
        public string animation;
        public int priority;
        public bool shouldInterupt;

        public QueuedAnimation(string animation, int priority, bool shouldInterupt) {
            this.animation = animation;
            this.priority = priority;
            this.shouldInterupt = shouldInterupt;
        }
    }

    protected Animator animator;
    public static Dictionary<int, string> hashesToString = new Dictionary<int, string>();
    public string currentAnimation;
    public string animation_prefix;
    private List<QueuedAnimation> queuedAnimations = new List<QueuedAnimation>();

    public string defaultAnimationAction { get; private set; } = "idle";

    protected void Awake() {
        animation_prefix = "Goblin";
        animator = GetComponent<Animator>();
        foreach (string animationName in GetAnimationNames()) {
            if (hashesToString.ContainsValue(animationName)) continue;
            hashesToString.Add(Animator.StringToHash(animationName), animationName);
        }
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

    public abstract Action GetStartActions(string animationName);
    public abstract Action GetEndActions(string animationName);
    public abstract string[] GetAnimationNames();

    public void OnStateEnter(int stateHash)
    {
        if (!TryHashToString(stateHash, out string name)) return;
        GetStartActions(name)?.Invoke();
    }

    public void OnStateExit(int stateHash)
    {
        if (!TryHashToString(stateHash, out string name)) return; 
        GetEndActions(name)?.Invoke();
        currentAnimation = "";
    }

    private void PlayAnimation(string animation)
    {
        print("Playing animation: " + animation);
        currentAnimation = animation;
        animator.Play(animation);
    }

    private void PlayNextAnimation() {
        if (queuedAnimations.Count == 0) return;
        foreach (QueuedAnimation queuedAnimation in queuedAnimations.OrderBy(x => x.priority)) {
            print("Checking queued animation: " + queuedAnimation.animation);
            if (currentAnimation == queuedAnimation.animation) continue;

            if (queuedAnimation.shouldInterupt) {
                PlayAnimation(queuedAnimation.animation);
                queuedAnimations.Clear();
                return;
            }

            if (currentAnimation == "") {
                PlayAnimation(queuedAnimation.animation);
                queuedAnimations.Clear();
                return;
            }
        }

        PlayAnimation(defaultAnimationAction);
    }

    public void QueueAnimation(string action, Vector2 direction, int priority = 0, bool shouldInterupt = false)
    {
        if (action == "death") {
            queuedAnimations.Add(new QueuedAnimation(
                $"{animation_prefix}_animation_death",
                priority,
                shouldInterupt
            ));
            return;
        }

        print("animation prefix: " + $"'{animation_prefix}'");
        queuedAnimations.Add(new QueuedAnimation(
            animation: $"{animation_prefix}_animation_{direction.DirectionToString()}_{action}",
            priority,
            shouldInterupt
        ));
    }

    private void Update()
    {
        PlayNextAnimation();
    }
}