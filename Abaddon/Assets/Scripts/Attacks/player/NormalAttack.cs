using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : BaseAbility
{
    public override void Attack(Fightable fightable, Vector2 direction, Animator animator, PlayerSfx sfxPlayer)
    {
        // rahh im attacking!!

        bool success = fightable.TakeDamage((uint)Controller.main.attackDamage);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");

        if (success) sfxPlayer.PlayAttackSound();
        else sfxPlayer.PlaySwoosh();

        Controller.main.PlayAnimation("attack", direction);
    }
}