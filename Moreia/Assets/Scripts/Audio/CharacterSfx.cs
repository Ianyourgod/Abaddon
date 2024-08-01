using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSfx : SfxPlayer
{
    [Header("CharacterSfx References")]
    [Tooltip("Include all walking sound effects for this character")]
    [SerializeField] AudioClip[] walkingSfx;
    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField] bool randomWalkingSound = true;
    [Space]

    [Tooltip("Include all hurt sound effects for this character")]
    [SerializeField] AudioClip[] hurtSfx;
    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField] bool randomHurtSound = false;
    [Space]

    [Tooltip("Include all attack sound effects for this character")]
    [SerializeField] AudioClip[] attackSfx;
    [Tooltip("Whether the action will play a random sound effect from the list or not")]
    [SerializeField] bool randomAttackSound = false;

    [Header("CharacterSfx Attributes")]
    [Tooltip("(volume is in 0.01 scale) How much volume you want to add to your walk sound effects")]
    [SerializeField] float addedWalkVolume = 0;
    [Tooltip("(volume is in 0.01 scale) How much volume you want to add to your hurt sound effects")]
    [SerializeField] float addedHurtVolume = 0;
    [Tooltip("(volume is in 0.01 scale) How much volume you want to add to your attack sound effects")]
    [SerializeField] float addedAttackVolume = 0;

    public void PlayWalkSound(){
        if (randomWalkingSound){
            PlayRandomSound(walkingSfx, addedWalkVolume);
            return;
        }

        PlaySfx(walkingSfx[0], addedWalkVolume);
    }

    public void PlayHurtSound(){
        if (randomHurtSound){
            PlayRandomSound(hurtSfx, addedHurtVolume);
            return;
        }

        PlaySfx(hurtSfx[0], addedHurtVolume);
    }

    public void PlayAttackSound(){
        if (randomAttackSound){
            PlayRandomSound(attackSfx, addedAttackVolume);
            return;
        }

        PlaySfx(attackSfx[0], addedAttackVolume);
    }
}
