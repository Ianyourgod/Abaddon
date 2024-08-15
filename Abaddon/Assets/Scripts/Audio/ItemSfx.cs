using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSfx : SfxPlayer
{
    [Header("ItemSfx References")]
    [Tooltip("Include all pickup item sound effects for this item")]
    [SerializeField] AudioClip[] pickupItemSfx;
    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField] bool randomPickupSound = false;
    [Space]

    [Header("ItemSfx Attributes")]
    [Tooltip("(volume is in 0.01 scale) How much volume you want to add to your pickup item sound effects")]
    [SerializeField] float addedPickupVolume = 0f;
    [Tooltip("Determines whether the item is a useable item")]
    public bool usable = false;

    void Start()
    {
        audSource = Controller.main.GetComponent<AudioSource>();
    }

    public void PlayPickupSound()
    {
        if (randomPickupSound)
        {
            PlayRandomSound(pickupItemSfx, addedPickupVolume);
            return;
        }

        PlaySfx(pickupItemSfx[0], addedPickupVolume);
    }
}
