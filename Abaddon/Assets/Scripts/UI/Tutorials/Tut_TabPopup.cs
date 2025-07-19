using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tut_TabPopup : TutorialPopup
{
    new void Start()
    {
        shouldBeOnScreen = false;
        base.Start();
    }

    new void Update()
    {
        shouldBeOnScreen =
            Controller.main.hasPickedUp
            && Controller.main.hasMoved
            && Controller.main.hasRotated
            && Controller.main.hasAttacked
            && !Controller.main.hasOpenedInventory;

        base.Update();
    }
}
