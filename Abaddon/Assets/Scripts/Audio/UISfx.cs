using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISfx : SfxPlayer
{
    [Header("UISfx References")]
    [Tooltip("Include all pickup item sound effects for this inventory")]
    [SerializeField]
    AudioClip[] pickupItemSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomPickupSound = false;

    [Space]
    [Tooltip("Include all place item sound effects for this inventory")]
    [SerializeField]
    AudioClip[] placeItemSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomPlaceSound = false;

    [Space]
    [Tooltip("Include all open inventory sound effects for this inventory")]
    [SerializeField]
    AudioClip[] openSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomOpenSound = false;

    [Space]
    [Tooltip("Include all close inventory sound effects for this inventory")]
    [SerializeField]
    AudioClip[] closeSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomCloseSound = false;

    [Space]
    [Tooltip("Include all equip item sound effects for this inventory")]
    [SerializeField]
    AudioClip[] equipSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomEquipSound = false;

    [Space]
    [Tooltip("Include all use item sound effects for this item")]
    [SerializeField]
    AudioClip[] useItemSfx;

    [Space]
    [Header("UISfx Attributes")]
    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your pickup item sound effects"
    )]
    [SerializeField]
    float addedPickupVolume = 0f;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your place item sound effects"
    )]
    [SerializeField]
    float addedPlaceVolume = 0f;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your open inventory sound effects"
    )]
    [SerializeField]
    float addedOpenVolume = 0f;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your close inventory sound effects"
    )]
    [SerializeField]
    float addedCloseVolume = 0f;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your close inventory sound effects"
    )]
    [SerializeField]
    float addedEquipVolume = 0f;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your use item sound effects"
    )]
    [SerializeField]
    float addedUseVolume = 0f;

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

    public void PlayOpenSound()
    {
        if (randomOpenSound)
        {
            PlayRandomSound(openSfx, addedOpenVolume);
            return;
        }

        PlaySfx(openSfx[0], addedOpenVolume);
    }

    public void PlayCloseSound()
    {
        if (randomCloseSound)
        {
            PlayRandomSound(closeSfx, addedCloseVolume);
            return;
        }

        PlaySfx(closeSfx[0], addedCloseVolume);
    }

    public void PlayEquipSound()
    {
        if (randomEquipSound)
        {
            PlayRandomSound(equipSfx, addedEquipVolume);
            return;
        }

        PlaySfx(equipSfx[0], addedEquipVolume);
    }

    public void PlayUseSound(int soundNumber)
    {
        PlaySfx(useItemSfx[soundNumber], addedUseVolume);
    }
}
