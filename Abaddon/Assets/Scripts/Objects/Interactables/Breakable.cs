using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(SfxPlayerBetter))]

public class Breakable : MonoBehaviour, Hurtable
{
    public enum BreakableType
    {
        Pot,
        Barrel
    }

    [SerializeField] BreakableType type = BreakableType.Pot;
    [SerializeField] float health = 1;
    SfxPlayerBetter sfxPlayer;

    public void Start()
    {
        sfxPlayer = GetComponent<SfxPlayerBetter>();
    }

    public bool Hurt(float damage)
    {
        sfxPlayer.PlaySound("break");
        health -= damage;
        if (health <= 0) Die();
        return true;
    }

    public void Die()
    {
        switch (type)
        {
            case BreakableType.Pot:
                Instantiate((GameObject)Resources.Load("Prefabs/Environment/PotBreak"), transform.position, Quaternion.identity);
                break;
            case BreakableType.Barrel:
                Instantiate((GameObject)Resources.Load("Prefabs/Environment/BarrelBreak"), transform.position, Quaternion.identity);
                break;
        }
        GetComponent<ItemDropper>().Die();
        Destroy(gameObject);
    }
}