using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WASDPopup : TutorialPopup
{
    new void Update()
    {
        shouldBeOnScreen = !Controller.main.hasMoved;

        base.Update();
    }
}
