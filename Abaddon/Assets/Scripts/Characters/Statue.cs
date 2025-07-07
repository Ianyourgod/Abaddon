using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, CanFight
{
    [HideInInspector] public Boss1 boss;
    [SerializeField] uint health = 20;
    [SerializeField] Animator animator;

    public void Attack()
    {
        return; // The statue does not attack
    }

    public uint Heal(uint amount)
    {
        health += amount;
        Debug.Log("statue health " + health);
        PlayAnimation("heal");
        return 0; // the statue does not have max health 
    }

    public void Hurt(uint damage)
    {
        if (damage >= health)
        {
            boss.StatueDestroyed();
            Die();
        }
        health -= damage;
        Debug.Log("statue health " + health);

        int damage_level = health < (20 / 3) ?
                            3
                            : health < (2 * 20 / 3) ?
                                2
                                : 1;

        PlayAnimation($"damage{damage_level}");
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
