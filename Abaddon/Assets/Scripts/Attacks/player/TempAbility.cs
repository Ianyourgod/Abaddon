using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAbility : BaseAbility {
    public override void Attack(Collider2D hit, Vector2 direction, Animator animator, PlayerSfx sfxPlayer) {
        // rahh im attacking!!
        Debug.Log("Rahh soul-steal!!");
        Controller.main.FinishTick();
    }
}