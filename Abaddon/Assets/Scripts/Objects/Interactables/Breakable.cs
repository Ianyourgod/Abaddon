using System.Collections;
using System.Collections.Generic;
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

    public void Start()
    {
        sfxPlayer = GetComponent<SfxPlayerBetter>();
        dropper = GetComponent<ItemDropper>();
    }

    public int Hurt(int damage)
    {
        sfxPlayer.PlaySound("break");
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
        dropper.DropRandomItem();
        Destroy(gameObject);
    }
}
