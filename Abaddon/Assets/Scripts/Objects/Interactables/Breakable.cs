using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(SfxPlayerBetter))]
public class Breakable : MonoBehaviour, CanBeKilled
{
    public enum BreakableType
    {
        Pot,
        Barrel,
    }

    [SerializeField]
    int health = 1;
    private SfxPlayerBetter sfxPlayer;
    private ItemDropper dropper;

    [SerializeField]
    private GameObject explosionPrefab;

    public void Start()
    {
        sfxPlayer = GetComponent<SfxPlayerBetter>();
        dropper = GetComponent<ItemDropper>();
    }

    public int Hurt(int damage)
    {
        int num = Random.Range(1, 4);
        sfxPlayer.PlaySound($"break{num}");
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        return health; // No healing overflow, as breakables do not heal.
    }

    public int Heal(int amount)
    {
        // Breakables cannot be healed, so this method does nothing.
        Debug.LogWarning("Breakables cannot be healed.");
        return health; // Return current health without changes.
    }

    public void Die()
    {
        // dropper.DropRandomItem();
        // print("spawning explosion");
        GameObject explosion = Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );
        if (explosion.TryGetComponent(out ExplosionEvents explosionEvents))
        {
            explosionEvents.AddComponent<ItemDropper>();
            explosionEvents.GetComponent<ItemDropper>().dropTable =
                GetComponent<ItemDropper>().dropTable;
        }
        else
        {
            Debug.LogWarning("Explosion prefab does not have ExplosionEvents component.");
        }
        if (explosion.TryGetComponent(out Animator animator))
        {
            animator.Play("explosion");
        }
        Destroy(gameObject);
    }
}
