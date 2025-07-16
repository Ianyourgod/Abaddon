using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableSfx : SfxPlayer
{
    [Header("BreakableSfx References")]
    [Tooltip("Include all break sound effects for this item")]
    [SerializeField]
    AudioClip[] breakSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomBreakSound = false;

    [Space]
    [Header("BreakableSfx Attributes")]
    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your break sound effects"
    )]
    [SerializeField]
    float addedBreakVolume = 0f;

    void Start()
    {
        audSource = AudioManagerBetter.main.deathSfxPlayer;
    }

    public void PlayBreakSound()
    {
        if (randomBreakSound)
        {
            PlayRandomSound(breakSfx, addedBreakVolume);
            return;
        }

        PlaySfx(breakSfx[0], addedBreakVolume);
    }
}
