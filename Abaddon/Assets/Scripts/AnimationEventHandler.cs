using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(Animator))]
public abstract class AnimationEventHandler : MonoBehaviour
{
    public static Dictionary<int, string> hashesToString = new Dictionary<int, string>();

    protected void Awake() {
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
    }
}