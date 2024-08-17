using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class AnimationHandlerData : ScriptableObject
{   
    public abstract string animation_prefix { get; }
    
    public abstract string[] GetAnimationNames();

    public abstract Action GetStartActions(string animationName);

    public abstract Action GetEndActions(string animationName);
}
