using System;

public abstract class AnimationHandlerData
{   
    public abstract string animationPrefix { get; }
    public AnimationEventHandler.Animation defaultAnimation;

    public abstract Action GetStartActions(string animationName);

    public abstract Action GetEndActions(string animationName);
}
