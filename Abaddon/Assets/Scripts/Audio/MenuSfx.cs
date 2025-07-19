using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSfx : SfxPlayer
{
    [Header("MenuSfx References")]
    [Tooltip("Include all press button sound effects for this item")]
    [SerializeField] AudioClip[] pressSfx;
    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField] bool randomPressSound = false;
    [Space]

    [Header("MenuSfx Attributes")]
    [Tooltip("(volume is in 0.01 scale) How much volume you want to add to your press button sound effects")]
    [SerializeField] float addedPressVolume = 0f;

    public void PlayPressSound()
    {
        if (randomPressSound)
        {
            PlayRandomSound(pressSfx, addedPressVolume);
            return;
        }

        PlaySfx(pressSfx[0], addedPressVolume);
    }
}
