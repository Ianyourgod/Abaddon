using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [Header("SfxPlayer References")]
    public AudioSource audSource;

    void Awake(){
        audSource = GetComponent<AudioSource>();
    }

    public void PlaySfx(AudioClip soundFx){
        audSource.PlayOneShot(soundFx, AudioManager.main.sfxVolume);
    }
}
