using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] GameObject droppedItem;
    [SerializeField] GameObject afterBroken;
    [SerializeField] float dropChance = 0.5f;
    [SerializeField] float health = 1;

    public void TakeHit(float damage) {
        print("being hit");
        print("this can be deleted");
        health -= damage;
        if (health <= 0) {
            if (Random.value < dropChance) {
                if (afterBroken != null) Instantiate(afterBroken, transform.position, Quaternion.identity);
                Instantiate(droppedItem, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
