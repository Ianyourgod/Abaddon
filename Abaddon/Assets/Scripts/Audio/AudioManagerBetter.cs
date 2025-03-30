using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerBetter : MonoBehaviour
{
    public static AudioManagerBetter main;

    [Header("References")]
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] AudioSource musicPlayer;

    [Header("Attributes")]
    [SerializeField] float sfxVolume = 0.02f;
    [SerializeField] float musicVolume = 0.02f;
    [SerializeField] float maxVolume = 1f;
    [SerializeField] float volumeInterval = 0.01f;
    [SerializeField] KeyCode increaseVolumeKey;
    [SerializeField] KeyCode decreaseVolumeKey;

    void Awake()
    {
        main = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(increaseVolumeKey) && (musicVolume + volumeInterval) <= maxVolume)
        {
            IncreaseVolume(volumeInterval);
        }
        if (Input.GetKeyDown(decreaseVolumeKey) && (musicVolume - volumeInterval) >= 0f)
        {
            DecreaseVolume(volumeInterval);
        }
    }

    public AudioSource SfxPlayer(){
        return sfxPlayer;
    }

    public AudioSource MusicPlayer(){
        return musicPlayer;
    }

    public void IncreaseVolume(float interval){
        sfxVolume += interval;
        musicVolume += interval;
    }

    public void DecreaseVolume(float interval){
        sfxVolume -= interval;
        musicVolume -= interval;
    }

    public void PlayMusic(AudioClip music){
        musicPlayer.clip = music;
        musicPlayer.Play();
    }

    public void PlaySfx(AudioClip sfx){
        sfxPlayer.clip = sfx;
        sfxPlayer.Play();
    }
}
