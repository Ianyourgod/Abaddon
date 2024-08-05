using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FountainSfx))]

public class Fountain : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] int HealthStored = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HealthStored <= 0)
        {
            HealthStored = 0;
            animator.Play("Fountain_empty");
        }
    }

    public void Heal()
    {
        // this solution is actually kinda pretty and i love it
        HealthStored = Controller.main.HealPlayer(HealthStored);
    }
}
