using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerAnimationHandlerData : AnimationHandlerData
{
    public override string animationPrefix => "Player";

    public Action onAttackEnd, onAttackStart;
    private string[] attackAnimations = new string[] {
        "Player_animation_back_level_0_level_0_attack",
        "Player_animation_front_level_0_attack",
        "Player_animation_left_level_0_attack",
        "Player_animation_right_level_0_attack"
    };

    public Action onWalkStart, onWalkEnd;
    private string[] walkAnimations = new string[] {
        "Player_animation_left_level_0_walk",
        "Player_animation_back_level_0_walk",
        "Player_animation_front_level_0_walk",
        "Player_animation_right_level_0_walk"
    };

    public Action onIdleStart, onIdleEnd;
    private string[] idleAnimations = new string[] {
        "Player_animation_back_level_0_idle",
        "Player_animation_front_level_0_idle",
        "Player_animation_left_level_0_idle",
        "Player_animation_right_level_0_idle"
    };

    public Action onHurtStart, onHurtEnd;
    private string[] hurtAnimations = new string[] {
        "Player_animation_back_level_0_hurt",
        "Player_animation_front_level_0_hurt",
        "Player_animation_left_level_0_hurt",
        "Player_animation_right_level_0_hurt"
    };

    public Action onDeathStart, onDeathEnd;
    private string[] deathAnimations = new string[] {
        "Player_animation_death"
    };

    public override Action GetStartActions(string animationName) {
        if (animationName.IsInList(attackAnimations)) return onAttackStart;
        if (animationName.IsInList(walkAnimations)) return onWalkStart;
        if (animationName.IsInList(idleAnimations)) return onIdleStart;
        if (animationName.IsInList(hurtAnimations)) return onHurtStart;
        if (animationName.IsInList(deathAnimations)) return onDeathStart;
        
        return null;
    }

    public override Action GetEndActions(string animationName) {
        if (animationName.IsInList(attackAnimations)) return onAttackEnd;
        if (animationName.IsInList(walkAnimations)) return onWalkEnd;
        if (animationName.IsInList(idleAnimations)) return onIdleEnd;
        if (animationName.IsInList(hurtAnimations)) return onHurtEnd;
        if (animationName.IsInList(deathAnimations)) return onDeathEnd;
        
        return null;
    }
}