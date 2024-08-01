using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Versioning;

[RequireComponent(typeof(AudioSource))]

public class SfxPlayer : MonoBehaviour
{
    [Header("SfxPlayer References")]
    [Tooltip("The audio source on the object that will play the sound effects")]
    public AudioSource audSource;

    [Header("SfxPlayer Attributes")]
    [Tooltip("(If the object is a player, ignore this) Determines whether the sound effects can be heard when the object is off screen")]
    public bool playableOffScreen = false;

    private int randomGenerator;

    void Awake(){
        audSource = GetComponent<AudioSource>();
    }

    public void PlaySfx(AudioClip soundFx, float addedSound = 0f){
        audSource.PlayOneShot(soundFx, AudioManager.main.sfxVolume);
    }

    public void PlayRandomSound(AudioClip[] audioClips, float addedSound = 0f){
        randomGenerator = Random.Range(0, audioClips.Length);

        PlaySfx(audioClips[randomGenerator], addedSound);
    }
}
