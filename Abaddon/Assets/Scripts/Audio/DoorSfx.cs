using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSfx : SfxPlayer
{
    [Header("DoorSfx References")]
    [Tooltip("Include all locked door sound effects for this door")]
    [SerializeField]
    AudioClip[] lockedDoorSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomLockedSound = false;

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

    [Header("DoorSfx Attributes")]
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

    void Start()
    {
        if (Controller.main == null)
            return;

        audSource = Controller.main.GetComponent<AudioSource>();
    }

    public void PlayLockedSound()
    {
        if (randomLockedSound)
        {
            PlayRandomSound(lockedDoorSfx, addedLockedVolume);
            return;
        }

        PlaySfx(lockedDoorSfx[0], addedLockedVolume);
    }

    public void PlayUnlockedSound()
    {
        if (randomUnlockedSound)
        {
            PlayRandomSound(unlockedSfx, addedUnlockedVolume);
            return;
        }

        PlaySfx(unlockedSfx[0], addedUnlockedVolume);
    }

    public void PlayUnlockLockedSound()
    {
        if (randomUnlockLockedSound)
        {
            PlayRandomSound(unlockLockedSfx, addedUnlockLockedVolume);
            return;
        }

        PlaySfx(unlockLockedSfx[0], addedUnlockLockedVolume);
    }
}
