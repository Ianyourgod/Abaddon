using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : DamageTaker
{
    [HideInInspector] public Boss1 boss;
    [SerializeField] uint health = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeDamage(uint damage) {
        health -= damage;
        Debug.Log("statue health " + health);
        if (health <= 0) {
            boss.Die();
        }
    }
}
