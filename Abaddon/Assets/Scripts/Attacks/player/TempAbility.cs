using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAbility : BaseAbility
{
    public override void Attack(
        CanFight fightable,
        Vector2 direction,
        Animator animator,
        PlayerSfx sfxPlayer
    )
    {
        if (Controller.main == null)
            return;
        // rahh im attacking!!
        Debug.Log("Rahh soul-steal!!");
        Controller.main.FinishTick();
    }
}
