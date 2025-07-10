using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSfx : CharacterSfx
{
    [Header("PlayerSfx References")]
    [SerializeField]
    AudioClip[] dodgeSfx;

    [SerializeField]
    bool randomDodgeSound = false;

    [Header("PlayerSfx Attributes")]
    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your dodge sound effects"
    )]
    [SerializeField]
    float addedDodgeVolume = 0f;

    void Awake()
    {
        playableOffScreen = true;
    }

    public void PlayDodgeSound()
    {
        if (randomDodgeSound)
        {
            PlayRandomSound(dodgeSfx, addedDodgeVolume);
            return;
        }

        PlaySfx(dodgeSfx[0], addedDodgeVolume);
    }
}
