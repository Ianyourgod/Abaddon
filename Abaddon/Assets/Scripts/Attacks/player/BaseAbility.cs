using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour {
    public virtual bool CanUse(Collider2D collider, Vector2 direction) {
        if (collider == null) return false;
        
        return collider.gameObject.layer == LayerMask.NameToLayer("Enemy");
    }

    public virtual void Attack(Collider2D hit, Vector2 direction, Animator animator, PlayerSfx sfxPlayer) {
        // rahh im attacking!!
        
    }
}