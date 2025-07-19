using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : BaseAbility {
    public override void Attack(Collider2D hit, Vector2 direction, Animator animator, PlayerSfx sfxPlayer) {
        // rahh im attacking!!
        hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(Convert.ToUInt32(Controller.main.attackDamage), hit.gameObject.tag);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        sfxPlayer.PlayAttackSound();
        Controller.main.PlayAnimation("attack", direction);
    }
}