using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePixieAttack : BaseAttack {
    public override void Attack(EnemyMovement.Direction direction) {
        Debug.Log("im a fire pixie and im stupid");
        Controller.main.DamagePlayer(damage, false);
    }
}