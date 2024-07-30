using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour {
    [SerializeField] public uint damage = 1;

    public virtual bool WillAttack(Collider2D collider, EnemyMovement.Direction direction) {
        // we just check if the collider is the player, and if it is, we return true - direction is for if children of this need it
        if (collider == null) return false;
        
        return collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }

    public virtual void Attack(EnemyMovement.Direction direction) {
        Controller.main.DamagePlayer(damage);
    }
}