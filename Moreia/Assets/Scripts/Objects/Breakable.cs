using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]

public class Breakable : MonoBehaviour
{
    public enum BreakableType { 
        Pot,
        Barrel
    }

    [SerializeField] BreakableType type = BreakableType.Pot;
    [SerializeField] float health = 1;

    public void TakeHit(float damage) {
        health -= damage;
        if (health <= 0) {
            switch (type) {
                case BreakableType.Pot:
                    Instantiate((UnityEngine.GameObject)Resources.Load("Prefabs/PotBreak"), transform.position, Quaternion.identity);
                    break;
                case BreakableType.Barrel:
                    break;
            }
            GetComponent<ItemDropper>().Die();
            Destroy(gameObject);
        }
    }
}