using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class AnimationEventHandler : MonoBehaviour
{
    public class Animation {
        public string name { get => (action == null) ? "" : action(); }
        private Func<string> action;
        public int priority;
        public Animation chainedAnimation;
        public bool shouldLoop;
        public bool persistUntilPlayed;

        public Animation(Func<string> action, int priority, bool shouldLoop, bool persistUntilPlayed, Animation chainedAnimation = null) {
            this.action = action;
            this.priority = priority;
            this.shouldLoop = shouldLoop;
            this.chainedAnimation = chainedAnimation;
            this.persistUntilPlayed = persistUntilPlayed;
        }

        public Animation(string action, int priority, bool shouldLoop, bool persistUntilPlayed, Animation chainedAnimation = null) {
            this.action = () => action;
            this.priority = priority;
            this.shouldLoop = shouldLoop;
            this.chainedAnimation = chainedAnimation;
            this.persistUntilPlayed = persistUntilPlayed;
        }

        public Animation(Animation animation) {
            priority = animation.priority;
            shouldLoop = animation.shouldLoop;
            chainedAnimation = animation.chainedAnimation;
            UpdateAction(animation.action());
        }

        public void UpdateAction(string action) { this.action = () => action; }
        public void UpdateAction(Func<string> action) { this.action = action; }

        public static implicit operator string(Animation queuedAnimation) => queuedAnimation.name;
    }

    public AnimationHandlerData data;
    public Animation currentAnimation = null;
    protected Animator animator;
    private List<Animation> queuedAnimations = new List<Animation>();
    private bool playingDefaultAnimation = true;

    protected void Awake() {
        animator = GetComponent<Animator>();
    }

    private void PlayAnimation(Animation animation)
    {
        if (currentAnimation != null && currentAnimation.name == animation.name) return;

        playingDefaultAnimation = animation == data.defaultAnimation;
        currentAnimation = new Animation(animation);
        data.GetStartActions(animation)?.Invoke();
        animator.Play(animation);
    }

    private bool TryToPlayQueuedAnimation() {
        if (queuedAnimations.Count > 0) {
            Animation bestChoice = queuedAnimations.OrderByDescending(x => x.priority).ToList()[0];
            if (currentAnimation is null || bestChoice.priority > currentAnimation.priority) {
                queuedAnimations.Remove(bestChoice);
                PlayAnimation(bestChoice);
            }
        }
        int index = 0;
        while (index < queuedAnimations.Count) {
            if (!queuedAnimations[index].persistUntilPlayed) queuedAnimations.Remove(queuedAnimations[index]);
            else index++;
        }

        // foreach (Animation queuedAnimation in queuedAnimations.OrderByDescending(x => x.priority)) {
        //     if (currentAnimation is null || queuedAnimation.priority > currentAnimation.priority) {
        //         PlayAnimation(queuedAnimation);
        //         playingDefaultAnimation = false;
        //         CleanQueue();
        //         return true;
        //     }
        // }
        return false;
    }

    public void QueueAnimation(Animation animation) {
        queuedAnimations.Add(animation);
    } 

    private bool DoneWithCurrentAnimation() {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && currentAnimation != null && !currentAnimation.shouldLoop;
    }

    private void Update()
    {
        if (DoneWithCurrentAnimation()) {
            data.GetEndActions(currentAnimation)?.Invoke();
            if (currentAnimation.chainedAnimation != null) PlayAnimation(currentAnimation.chainedAnimation);
            else currentAnimation = null;
        }

        TryToPlayQueuedAnimation();

        if (currentAnimation is null || playingDefaultAnimation) {
            PlayAnimation(data.defaultAnimation);
        }
    }
}