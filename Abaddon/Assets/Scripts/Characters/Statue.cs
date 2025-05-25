using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, Hurtable
{
    [HideInInspector] public Boss1 boss;
    [SerializeField] int health = 20;
    [SerializeField] Animator animator;

    public bool Hurt(float damage)
    {
        health -= (int)damage;
        Debug.Log("statue health " + health);
        if (health <= 0)
        {
            boss.StatueDestroyed();
            Die();
        }
        else
        {
            int damage_level = health < (20 / 3) ?
                                3
                                : health < (2 * 20 / 3) ?
                                    2
                                    : 1;

            PlayAnimation($"damage{damage_level}");
        }

        return true;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void PlayAnimation(string action)
    {
        string animation = $"statue_animation_{action}";

        animator.Play(animation);
    }
}
