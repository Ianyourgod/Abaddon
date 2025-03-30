using System;
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

    [Header("Music Dictionary")]
    [Tooltip("Keys and values must be at same indexes, and keys and values must be the same length")]
    [SerializeField] string[] songKeys;
    [SerializeField] AudioClip[] songValues;

    Dictionary<string, AudioClip> songs;

    void Awake()
    {
        main = this;
        sfxPlayer.volume = sfxVolume;
        musicPlayer.volume = musicVolume;

        songs = new Dictionary<string, AudioClip>();
        for(int i = 0; i < songKeys.Length; i++){
            songs[songKeys[i]] = songValues[i];
        }
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

        sfxPlayer.volume = sfxVolume;
        musicPlayer.volume = musicVolume;
    }

    public void DecreaseVolume(float interval){
        sfxVolume -= interval;
        musicVolume -= interval;

        sfxPlayer.volume = sfxVolume;
        musicPlayer.volume = musicVolume;
    }

    public void PlayMusic(AudioClip music){
        musicPlayer.clip = music;
        musicPlayer.Play();
    }

    public void StopMusic(){
        musicPlayer.Stop();
    }

    public void PlaySfx(AudioClip sfx){
        sfxPlayer.clip = sfx;
        sfxPlayer.Play();
    }
}
