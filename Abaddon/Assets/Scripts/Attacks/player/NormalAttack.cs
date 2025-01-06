using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : BaseAbility {
    public override void Attack(Collider2D hit, Vector2 direction, Animator animator, PlayerSfx sfxPlayer) {
        // rahh im attacking!!

        // check if the hit object has "BossScript" component
        if (hit.gameObject.GetComponent<Boss1>() != null) {
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
            sfxPlayer.PlaySwoosh();
            Controller.main.PlayAnimation("attack", direction);
            return;
        }

        hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(Convert.ToUInt32(Controller.main.attackDamage), hit.gameObject.tag);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        sfxPlayer.PlayAttackSound();
        Controller.main.PlayAnimation("attack", direction);
    }
}