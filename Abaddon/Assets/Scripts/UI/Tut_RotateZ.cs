using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tut_RotateZ : TutorialPopup
{
    public static Tut_RotateZ singleton;

    void Awake()
    {
        if (!singleton)
            singleton = this;
    }

    new void Start()
    {
        shouldBeOnScreen = false;
        base.Start();
    }

    new void Update()
    {
        if (shouldBeOnScreen && Controller.main.hasRotated)
            shouldBeOnScreen = false;

        base.Update();
    }
}
