using System;
using System.Linq;

public class GnomeAnimationEventHandler : AnimationEventHandler
{
    public Action onAttackEnd, onAttackStart;
    private string[] attackAnimations = new string[] {
        "Goblin_animation_back_attack",
        "Goblin_animation_front_attack",
        "Goblin_animation_left_attack",
        "Goblin_animation_right_attack"
    };

    public Action onWalkStart, onWalkEnd;
    private string[] walkAnimations = new string[] {
        "Goblin_animation_left_walk",
        "Goblin_animation_back_walk",
        "Goblin_animation_front_walk",
        "Goblin_animation_right_walk"
    };

    public Action onIdleStart, onIdleEnd;
    private string[] idleAnimations = new string[] {
        "Goblin_animation_back_idle",
        "Goblin_animation_front_idle",
        "Goblin_animation_left_idle",
        "Goblin_animation_right_idle"
    };

    public Action onHurtStart, onHurtEnd;
    private string[] hurtAnimations = new string[] {
        "Goblin_animation_back_hurt",
        "Goblin_animation_front_hurt",
        "Goblin_animation_left_hurt",
        "Goblin_animation_right_hurt"
    };

    
    public override string[] GetAnimationNames() {
        return attackAnimations
            .Concat(walkAnimations)
            .Concat(idleAnimations)
            .Concat(hurtAnimations)
            .ToArray();
    }

    public override Action GetStartActions(string animationName) {
        if (animationName.IsInList(attackAnimations)) return onAttackStart;
        if (animationName.IsInList(walkAnimations)) return onWalkStart;
        if (animationName.IsInList(idleAnimations)) return onIdleStart;
        if (animationName.IsInList(hurtAnimations)) return onHurtStart;
        
        return null;
    }

    public override Action GetEndActions(string animationName) {
        if (animationName.IsInList(attackAnimations)) return onAttackEnd;
        if (animationName.IsInList(walkAnimations)) return onWalkEnd;
        if (animationName.IsInList(idleAnimations)) return onIdleEnd;
        if (animationName.IsInList(hurtAnimations)) return onHurtEnd;
        
        return null;
    }


}
