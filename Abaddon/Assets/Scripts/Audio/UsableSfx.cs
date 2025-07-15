using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableSfx : ItemSfx
{
    [Header("UsableSfx References")]
    [Tooltip("Include all use item sound effects for this item")]
    [SerializeField]
    AudioClip[] useItemSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomUseSound = false;

    [Space]
    [Header("UseableSfx Attributes")]
    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your use item sound effects"
    )]
    [SerializeField]
    float addedUseVolume = 0f;

    void Start() { }

    public void PlayUseSound()
    {
        if (randomUseSound)
        {
            PlayRandomSound(useItemSfx, addedUseVolume);
            return;
        }

        PlaySfx(useItemSfx[0], addedUseVolume);
    }
}
