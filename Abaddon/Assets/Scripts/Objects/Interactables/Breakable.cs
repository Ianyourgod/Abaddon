using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(SfxPlayerBetter))]

public class Breakable : MonoBehaviour, CanBeInteractedWith
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

    public void Interact()
    {
        sfxPlayer.PlaySound("break");
        health -= 1;
        if (health <= 0) Die();
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