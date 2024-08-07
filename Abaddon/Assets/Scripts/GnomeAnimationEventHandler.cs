using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class GnomeAnimationEventHandler : AnimationEventHandler
{
    public Action onAttackEnd = () => print("attack end");
    public Action onAttackStart = () => print("attack start");
    public Action onIdleStart = () => print("idle start");
    public Action onIdleEnd = () => print("idle end");

    public Action onWalkStart = () => print("walk start");
    public Action onWalkEnd = () => print("walk end");

    new private void Awake()
    {
        endActions.Add(new EventKeyAndAction { 
            key = "Goblin_animation_back_attack", 
            action = onAttackEnd 
        });

        endOfActions = new Dictionary<string, Action> {
            {"Goblin_animation_back_attack", onAttackEnd},
            {"Goblin_animation_back_idle", onIdleEnd},
            {"Goblin_animation_back_walk", onWalkEnd},
            {"Goblin_animation_front_attack", onAttackEnd},
            {"Goblin_animation_front_idle", onIdleEnd},
            {"Goblin_animation_front_walk", onWalkEnd},
            {"Goblin_animation_left_attack", onAttackEnd},
            {"Goblin_animation_left_idle", onIdleEnd},
            {"Goblin_animation_left_walk", onWalkEnd},
            {"Goblin_animation_right_attack", onAttackEnd},
            {"Goblin_animation_right_idle", onIdleEnd},
            {"Goblin_animation_right_walk", onWalkEnd}
        };

        startOfActions = new Dictionary<string, Action> {
            {"Goblin_animation_back_attack", onAttackStart},
            {"Goblin_animation_back_idle", onIdleStart},
            {"Goblin_animation_back_walk", onWalkStart},
            {"Goblin_animation_front_attack", onAttackStart},
            {"Goblin_animation_front_idle", onIdleStart},
            {"Goblin_animation_front_walk", onWalkStart},
            {"Goblin_animation_left_attack", onAttackStart},
            {"Goblin_animation_left_idle", onIdleStart},
            {"Goblin_animation_left_walk", onWalkStart},
            {"Goblin_animation_right_attack", onAttackStart},
            {"Goblin_animation_right_idle", onIdleStart},
            {"Goblin_animation_right_walk", onWalkStart}
        };

        base.Awake();
    }
}
