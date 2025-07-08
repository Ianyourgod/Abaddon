using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : BaseAbility
{
    // public override void Attack(CanFight fightable, Vector2 direction, Animator animator, PlayerSfx sfxPlayer)
    public override void Attack(CanFight enemy, Vector2 direction, Animator animator, PlayerSfx sfxPlayer)
    {
        enemy.Hurt((uint)Controller.main.attackDamage);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");

        sfxPlayer.PlayAttackSound();

        Controller.main.PlayAnimation("attack", direction);
    }
}