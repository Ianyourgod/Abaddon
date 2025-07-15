using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, CanFight
{
    [HideInInspector]
    public Boss1 boss;

    [SerializeField]
    int health = 20;

    [SerializeField]
    Animator animator;

    public void Attack()
    {
        return; // The statue does not attack
    }

    public EnemyType GetEnemyType()
    {
        return EnemyType.Statue;
    }

    public int Heal(int amount)
    {
        health += amount;
        Debug.Log("statue health " + health);
        PlayAnimation("heal");
        return 0; // the statue does not have max health
    }

    public int Hurt(int damage)
    {
        if (damage >= health)
        {
            boss.StatueDestroyed();
            Die();
        }
        health -= damage;
        Debug.Log("statue health " + health);

        int damage_level =
            health < (20 / 3) ? 3
            : health < (2 * 20 / 3) ? 2
            : 1;

        PlayAnimation($"damage{damage_level}");
        return health; // Return remaining health
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
