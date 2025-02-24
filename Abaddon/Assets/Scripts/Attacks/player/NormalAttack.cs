using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : BaseAbility {
    public override void Attack(Collider2D hit, Vector2 direction, Animator animator, PlayerSfx sfxPlayer) {
        // rahh im attacking!!

        bool success = hit.gameObject.GetComponent<DamageTaker>().TakeDamage(Convert.ToUInt32(Controller.main.attackDamage));
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        if (success) {
            sfxPlayer.PlayAttackSound();
        } else {
            sfxPlayer.PlaySwoosh();
        }
        Controller.main.PlayAnimation("attack", direction);
    }
}