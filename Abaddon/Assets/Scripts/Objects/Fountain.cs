using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SfxPlayerBetter))]
public class Fountain : MonoBehaviour, CanBeInteractedWith
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    int healthStored = 50;
    private SfxPlayerBetter sfxPlayer;

    void Start()
    {
        sfxPlayer = GetComponent<SfxPlayerBetter>();
    }

    void Update()
    {
        if (healthStored <= 0)
            animator.Play("Fountain_empty");
    }

    public void Interact()
    {
        if (Controller.main == null)
            return;

        healthStored = Math.Max(0, Controller.main.HealPlayer(healthStored));
        sfxPlayer.PlaySound("drink");
    }
}
