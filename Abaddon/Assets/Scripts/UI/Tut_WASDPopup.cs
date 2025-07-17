using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tut_WASDPopup : TutorialPopup
{
    new void Start()
    {
        shouldBeOnScreen = !Controller.main.hasMoved;
        base.Start();
    }

    new void Update()
    {
        if (shouldBeOnScreen == Controller.main.hasMoved)
        {
            Tut_RotateX.singleton.Enable();
            Tut_RotateZ.singleton.Enable();
        }

        shouldBeOnScreen = !Controller.main.hasMoved;

        base.Update();
    }
}
