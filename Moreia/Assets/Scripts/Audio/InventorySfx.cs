using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySfx : SfxPlayer
{
    [Header("InventorySfx References")]
    [Tooltip("Include all pickup item sound effects for this inventory")]
    [SerializeField] AudioClip[] pickupItemSfx;
    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField] bool randomPickupSound = false;
    [Space]

    [Tooltip("Include all place item sound effects for this inventory")]
    [SerializeField] AudioClip[] placeItemSfx;
    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField] bool randomPlaceSound = false;
    [Space]

    [Header("InventorySfx Attributes")]
    [Tooltip("(volume is in 0.01 scale) How much volume you want to add to your pickup item sound effects")]
    [SerializeField] float addedPickupVolume = 0f;
    [Tooltip("(volume is in 0.01 scale) How much volume you want to add to your place item sound effects")]
    [SerializeField] float addedPlaceVolume = 0f;

    public void PlayPickupSound()
    {
        if (randomPickupSound)
        {
            PlayRandomSound(pickupItemSfx, addedPickupVolume);
            return;
        }

        PlaySfx(pickupItemSfx[0], addedPickupVolume);
    }

    public void PlayPlaceSound()
    {
        if (randomPlaceSound)
        {
            PlayRandomSound(placeItemSfx, addedPlaceVolume);
            return;
        }

        PlaySfx(placeItemSfx[0], addedPlaceVolume);
    }
}
