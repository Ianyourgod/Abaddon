using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour {
    [SerializeField] public uint damage = 1;

    public virtual void Attack(EnemyMovement.Direction direction) {
        Controller.main.DamagePlayer(damage);
    }
}