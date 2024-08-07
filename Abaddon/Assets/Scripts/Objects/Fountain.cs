using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FountainSfx))]

public class Fountain : MonoBehaviour, Interactable
{
    [SerializeField] Animator animator;
    [SerializeField] int HealthStored = 50;


    public void Interact()
    {        
        HealthStored = Controller.main.HealPlayer(HealthStored);
    }
}
