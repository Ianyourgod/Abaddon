using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSfx : SfxPlayer
{
    [Header("CharacterSfx References")]
    [Tooltip("Include all walking sound effects for this character")]
    [SerializeField]
    AudioClip[] walkingSfx;

    public AudioClip dashSound;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomWalkingSound = true;

    [Space]
    [Tooltip("Include all hurt sound effects for this character")]
    [SerializeField]
    AudioClip[] hurtSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomHurtSound = false;

    [Space]
    [Tooltip("Include all attack sound effects for this character")]
    [SerializeField]
    AudioClip[] attackSfx;

    [Space]
    [Tooltip("Include all swoosh sound effects for this character")]
    [SerializeField]
    AudioClip[] swooshSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomSwooshSound = false;

    [Space]
    [Tooltip("Include all death sound effects for this character")]
    [SerializeField]
    AudioClip[] deathSfx;

    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField]
    bool randomDeathSound = false;

    [Space]
    [Header("CharacterSfx Attributes")]
    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your walk sound effects"
    )]
    [SerializeField]
    float addedWalkVolume = 0;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your hurt sound effects"
    )]
    [SerializeField]
    float addedHurtVolume = 0;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your attack sound effects"
    )]
    [SerializeField]
    float addedAttackVolume = 0;

    [Tooltip(
        "(volume is in 0.01 scale) How much volume you want to add to your death sound effects"
    )]
    [SerializeField]
    float addedDeathVolume = 0;

    public void PlayDashSound()
    {
        if (dashSound != null)
        {
            PlaySfx(dashSound, 0);
        }
    }

    public void PlayWalkSound()
    {
        if (walkingSfx.Length == 0)
            return;

        if (randomWalkingSound)
            PlayRandomSound(walkingSfx, addedWalkVolume);
        else
            PlaySfx(walkingSfx[0], addedWalkVolume);
    }

    public void PlayHurtSound()
    {
        if (randomHurtSound)
        {
            PlayRandomSound(hurtSfx, addedHurtVolume);
            return;
        }

        PlaySfx(hurtSfx[0], addedHurtVolume);
    }

    public void PlayAttackSound(int? index = null)
    {
        if (index == null)
        {
            PlayRandomSound(attackSfx, addedAttackVolume);
        }
        else
        {
            PlaySfx(attackSfx[index.Value], addedAttackVolume);
        }
    }

    public void PlayDeathSound()
    {
        if (randomDeathSound)
        {
            PlayRandomSound(deathSfx, addedDeathVolume);
            return;
        }

        PlaySfx(deathSfx[0], addedDeathVolume);
    }

    public void PlaySwoosh()
    {
        if (randomSwooshSound)
        {
            PlayRandomSound(swooshSfx, 0);
            return;
        }

        PlaySfx(swooshSfx[0], 0);
    }
}
