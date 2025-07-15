using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSfx : SfxPlayer
{
    [SerializeField]
    AudioClip purchaseSFX;

    [SerializeField]
    AudioClip selectSFX;

    /*
    [Space]
    [Tooltip("Include all unlocked door sound effects for this door")]
    [SerializeField]
    AudioClip[] unlockedSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomUnlockedSound = false;

    [Space]
    [Tooltip("Include all unlocking door sound effects for this door")]
    [SerializeField]
    AudioClip[] unlockLockedSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomUnlockLockedSound = false;
    */

    /*
    [Header("Attributes")]
    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your locked door sound effects"
    )]
    [SerializeField]
    float addedLockedVolume = 0;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your unlocked door sound effects"
    )]
    [SerializeField]
    float addedUnlockedVolume = 0;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your unlocking door sound effects"
    )]
    [SerializeField]
    float addedUnlockLockedVolume = 0;
    */

    void Start()
    {
        if (Controller.main == null)
            return;

        audSource = Controller.main.GetComponent<AudioSource>();
    }

    public void PlayPurchaseSound()
    {
        PlaySfx(purchaseSFX, 1);
    }

    public void PlaySelectSound()
    {
        PlaySfx(selectSFX, 2);
    }
}
