using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(BreakableSfx))]

public class Breakable : MonoBehaviour, Interactable
{
    public enum BreakableType {
        Pot,
        Barrel
    }

    [SerializeField] BreakableType type;
    BreakableSfx sfxPlayer;

    public void Start() {
        sfxPlayer = GetComponent<BreakableSfx>();
    }

    public void Interact()
    {
        sfxPlayer.PlayBreakSound();
        switch (type)
        {
            case BreakableType.Pot:
                Instantiate((GameObject)Resources.Load("Prefabs/PotBreak"), transform.position, Quaternion.identity);
                break;
            case BreakableType.Barrel:
                Instantiate((GameObject)Resources.Load("Prefabs/BarrelBreak"), transform.position, Quaternion.identity);
                break;
        }
        GetComponent<ItemDropper>().Die();
        Destroy(gameObject);
    }
}