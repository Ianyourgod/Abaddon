using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : DamageTaker
{
    [HideInInspector] public Boss1 boss;
    [SerializeField] int health = 20;
    [SerializeField] Animator animator;

    public override bool TakeDamage(uint damage) {
        health -= (int) damage;
        Debug.Log("statue health " + health);
        if (health <= 0) {
            boss.StatueDestroyed();
            Die();
        } else {
            base.TakeDamage(damage); // this is so the damage text appears
            int damage_level = health < (20/3) ?
                                3
                                : health < (2*20/3) ?
                                    2
                                    : 1;

            PlayAnimation($"damage{damage_level}");
        }

        return true;
    }

    private void Die() {
        Destroy(gameObject);
    }

    private void PlayAnimation(string action)
    {
        string animation = $"statue_animation_{action}";

        animator.Play(animation);
    }
}
