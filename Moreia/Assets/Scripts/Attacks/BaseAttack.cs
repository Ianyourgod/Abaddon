using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour {
    [SerializeField] uint damage = 1;

    public void Attack(EnemyMovement.Direction direction) {
        Controller.main.DamagePlayer(damage);
    }
}