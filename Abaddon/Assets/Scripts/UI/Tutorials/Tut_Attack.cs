using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tut_Attack : TutorialPopup
{
    new void Start()
    {
        shouldBeOnScreen = false;
        base.Start();
    }

    new void Update()
    {
        shouldBeOnScreen = Controller.main.hasRotated && !Controller.main.hasAttacked;

        base.Update();
    }
}
