using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, CanFight
{
    public Boss1 boss;

    [SerializeField]
    int health = 20;

    [SerializeField]
    GameObject explosionPrefab;

    [SerializeField]
    Animator animator;

    private bool isActive = false;

    public void Attack()
    {
        return; // The statue does not attack
    }

    public EnemyType GetEnemyType()
    {
        return EnemyType.Statue;
    }

    public void Start()
    {
        Controller.main.onDie += () =>
        {
            if (isActive)
            {
                isActive = true;
                PlayAnimation("idle");
            }
        };
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
        if (!isActive)
        {
            Debug.LogWarning("Statue is not active, cannot take damage.");
            return health; // Return current health without taking damage
        }

        if (damage >= health)
        {
            Die();
        }
        health -= damage;
        Debug.Log("statue health " + health);

        int damage_level =
            health < (health / 3) ? 3
            : health < (2 * health / 3) ? 2
            : 1;

        PlayAnimation($"damage{damage_level}");
        return health; // Return remaining health
    }

    public void Die()
    {
        boss.OnStatueDestroyed();
        GameObject explosion = Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );
        if (explosion.TryGetComponent(out Animator animator))
        {
            animator.Play("explosion");
        }
        Destroy(gameObject);
    }

    private void PlayAnimation(string action)
    {
        string animation = $"statue_animation_{action}";

        animator.Play(animation);
    }

    public void Activate()
    {
        isActive = true;
        PlayAnimation("activating");
    }
}
