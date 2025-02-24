using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : DamageTaker
{
    [HideInInspector] public Boss1 boss;
    [SerializeField] int health = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool TakeDamage(uint damage) {
        health -= (int) damage;
        Debug.Log("statue health " + health);
        if (health <= 0) {
            boss.StatueDestroyed();
            Die();
        }

        return true;
    }

    private void Die() {
        Destroy(gameObject);
    }
}
