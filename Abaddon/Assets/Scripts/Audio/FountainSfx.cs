using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainSfx : SfxPlayer
{
    [Header("FountainSfx References")]
    [Tooltip("Include all fountain sound effects for this item")]
    [SerializeField]
    AudioClip[] fountainSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomFountainSound = false;

    [Space]
    [Header("FountainSfx Attributes")]
    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your fountain sound effects"
    )]
    [SerializeField]
    float addedFountainVolume = 0f;

    public void PlayFountainSound()
    {
        if (randomFountainSound)
        {
            PlayRandomSound(fountainSfx, addedFountainVolume);
            return;
        }

        PlaySfx(fountainSfx[0], addedFountainVolume);
    }
}
