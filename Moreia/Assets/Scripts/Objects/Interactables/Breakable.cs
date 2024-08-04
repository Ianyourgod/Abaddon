using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(BreakableSfx))]

public class Breakable : Interactable
{
    public enum BreakableType
    {
        Pot,
        Barrel
    }

    [SerializeField] BreakableType type = BreakableType.Pot;
    [SerializeField] float health = 1;
    BreakableSfx sfxPlayer;

    public void Start() {
        sfxPlayer = GetComponent<BreakableSfx>();
    }

    public override void Interact(float damage)
    {
        sfxPlayer.PlayBreakSound();
        health -= damage;
        if (health <= 0)
        {
            switch (type)
            {
                case BreakableType.Pot:
                    Instantiate((UnityEngine.GameObject)Resources.Load("Prefabs/PotBreak"), transform.position, Quaternion.identity);
                    break;
                case BreakableType.Barrel:
                    Instantiate((UnityEngine.GameObject)Resources.Load("Prefabs/BarrelBreak"), transform.position, Quaternion.identity);
                    break;
            }
            GetComponent<ItemDropper>().Die();
            Destroy(gameObject);
        }
    }
}