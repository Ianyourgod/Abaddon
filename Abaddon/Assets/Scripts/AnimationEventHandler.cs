using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class AnimationEventHandler : MonoBehaviour
{
    public class EventKeyAndAction
    {
        public string key;
        public Action action;
    }

    public Action GetEndOfAction(string key) {
        foreach (var action in endActions) {
            if (action.key == key) return action.action;
        }
        return null;
    }


    public static Dictionary<int, string> animationStates = new Dictionary<int, string>();
    
    public List<EventKeyAndAction> startActions = new List<EventKeyAndAction>();
    public List<EventKeyAndAction> endActions = new List<EventKeyAndAction>();
    public Dictionary<string, Action> endOfActions;
    public Dictionary<string, Action> startOfActions;

    protected void Awake() {
        foreach (var field in endOfActions.Keys) {
            if (!animationStates.ContainsValue(field)) animationStates.Add(Animator.StringToHash(field), field);
        }
        foreach (var field in startOfActions.Keys) {
            if (!animationStates.ContainsValue(field)) animationStates.Add(Animator.StringToHash(field), field);
        }
    }

    public static bool TryHashToString(int hash, out string name)
    {
        if (animationStates.ContainsKey(hash))
        {
            name = animationStates[hash];
            return true;
        }
        name = "Unknown";
        return false;
    }

    public void OnStateEnter(int stateHash)
    {
        if (!TryHashToString(stateHash, out string name)) return;
        startOfActions[name]?.Invoke();
    }

    public void OnStateExit(int stateHash)
    {
        if (!TryHashToString(stateHash, out string name)) return;
        if (name == "Goblin_animation_right_attack") {
            print("exiting right attack | is null: " + (endOfActions[name] == null));
            print(endOfActions[name].GetInvocationList().Length);
            string[] names = endOfActions[name].GetInvocationList().Select(x => x.Method.Name).ToArray();
            foreach (string n in names) {
                print(n);
            }
        } 

        endOfActions[name]?.Invoke();
    }
}
