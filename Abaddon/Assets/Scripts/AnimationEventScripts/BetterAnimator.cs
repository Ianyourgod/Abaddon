using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class BetterAnimator : MonoBehaviour
{
    public BetterAnimation currentAnimation = null;
    protected Animator animator;
    private List<BetterAnimation> queuedAnimations = new List<BetterAnimation>();
    private bool playingDefaultAnimation = true;
    public BetterAnimation defaultAnimation = null;

    public void QueueAnimation(BetterAnimation animation) => queuedAnimations.Add(animation);
    public void ClearAnimationQueue() => queuedAnimations.Clear();
    public void RemoveAnimation(BetterAnimation animation) => queuedAnimations.Remove(animation);
    public void ClearAnimationsBelowPriority(int priority) {
        for (int i = 0; i < queuedAnimations.Count; i++) {
            if (queuedAnimations[i].priority < priority) queuedAnimations.RemoveAt(i--);
        }
    }

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (defaultAnimation is null) animator.enabled = false;
    }

    private void PlayAnimation(BetterAnimation animation)
    {
        animator.enabled = true;
        if (currentAnimation is not null && currentAnimation.name == animation.name) return;

        currentAnimation?.onAnimationStop?.Invoke();
        currentAnimation?.onAnimationInterupt?.Invoke();
        animation.onAnimationStart?.Invoke();

        playingDefaultAnimation = animation == defaultAnimation;
        currentAnimation = new BetterAnimation(animation);
        animator.Play(animation);
    }

    private bool TryToPlayQueuedAnimation() {
        if (queuedAnimations.Count > 0) {
            BetterAnimation bestChoice = queuedAnimations.OrderByDescending(x => x.priority).ToList()[0];
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

        return false;
    }

    private bool DoneWithCurrentAnimation() {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && currentAnimation != null && !currentAnimation.shouldLoop;
    }

    private void Update()
    {
        if (DoneWithCurrentAnimation()) {
            currentAnimation.onAnimationEnd?.Invoke();
            currentAnimation.onAnimationStop?.Invoke();
            if (currentAnimation.chainedAnimation != null) PlayAnimation(currentAnimation.chainedAnimation);
            else currentAnimation = null;
        }

        TryToPlayQueuedAnimation();

        if (currentAnimation is null || playingDefaultAnimation) {
            PlayDefault();
        }
    }

    private void PlayDefault() {
        if (defaultAnimation is not null) {
            PlayAnimation(defaultAnimation);
        }
        else {
            animator.enabled = false;
        }
    }
}

public class BetterAnimation {
        public string name { get => (action == null) ? "" : action(); }
        private Func<string> action;
        public int priority;
        public BetterAnimation chainedAnimation;
        public bool shouldLoop;
        public bool persistUntilPlayed;
        public Action onAnimationEnd;
        public Action onAnimationInterupt;
        public Action onAnimationStop;
        public Action onAnimationStart;

        public BetterAnimation(
            Func<string> action, 
            int priority, 
            bool shouldLoop, 
            bool persistUntilPlayed, 
            BetterAnimation chainedAnimation = null,
            Action onAnimationStart = null,
            Action onAnimationStop = null,
            Action onAnimationEnd = null,
            Action onAnimationInterupt = null
        ) {
            this.action = action;
            this.priority = priority;
            this.shouldLoop = shouldLoop;
            this.chainedAnimation = chainedAnimation;
            this.persistUntilPlayed = persistUntilPlayed;
            this.onAnimationStart = onAnimationStart;
            this.onAnimationStop = onAnimationStop;
            this.onAnimationEnd = onAnimationEnd;
            this.onAnimationInterupt = onAnimationInterupt;
        }

        public BetterAnimation(
            string animation_name, 
            int priority, 
            bool shouldLoop, 
            bool persistUntilPlayed, 
            BetterAnimation chainedAnimation = null,
            Action onAnimationStart = null,
            Action onAnimationStop = null,
            Action onAnimationEnd = null,
            Action onAnimationInterupt = null
        ) {
            this.action = () => animation_name;
            this.priority = priority;
            this.shouldLoop = shouldLoop;
            this.chainedAnimation = chainedAnimation;
            this.persistUntilPlayed = persistUntilPlayed;

            this.onAnimationStart = onAnimationStart;
            this.onAnimationStop = onAnimationStop;
            this.onAnimationEnd = onAnimationEnd;
            this.onAnimationInterupt = onAnimationInterupt;
        }

        public BetterAnimation(BetterAnimation animation) {
            priority = animation.priority;
            shouldLoop = animation.shouldLoop;
            chainedAnimation = animation.chainedAnimation;
            persistUntilPlayed = animation.persistUntilPlayed;

            onAnimationStart = animation.onAnimationStart;
            onAnimationStop = animation.onAnimationStop;
            onAnimationEnd = animation.onAnimationEnd;
            onAnimationInterupt = animation.onAnimationInterupt;
            UpdateAction(animation.action());
        }

        public void UpdateAction(string action) { this.action = () => action; }
        public void UpdateAction(Func<string> action) { this.action = action; }

        public static implicit operator string(BetterAnimation queuedAnimation) => queuedAnimation.name;
    }