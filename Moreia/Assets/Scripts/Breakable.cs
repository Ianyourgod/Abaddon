using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]

public class Breakable : MonoBehaviour
{
    [SerializeField] float health = 1;

    public void TakeHit(float damage) {
        health -= damage;
        if (health <= 0) {
            GetComponent<ItemDropper>().Die();
            Destroy(gameObject);
        }
    }
}