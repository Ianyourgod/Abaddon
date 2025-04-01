using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerBetter : MonoBehaviour
{
    public static AudioManagerBetter main;

    [Header("References")]
    [SerializeField] AudioSource musicSource;

    [Header("Attributes")]
    [SerializeField] float sfxVolume = 0.02f;
    [SerializeField] float musicVolume = 0.02f;
    [SerializeField] float maxVolume = 1f;
    [SerializeField] float volumeInterval = 0.01f;
    [SerializeField] KeyCode increaseVolumeKey;
    [SerializeField] KeyCode decreaseVolumeKey;

    [Header("Music Dictionary")]
    [Tooltip("Keys and values must be at same indexes, and keys and values lists must be the same length")]
    [SerializeField] string[] songKeys;
    [SerializeField] AudioClip[] songValues;

    Dictionary<string, AudioClip> songs;

    void Awake()
    {
        main = this;

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

    public AudioClip GetSong(string key){
        return songs[key];
    }

    public AudioSource GetMusicSource(){
        return musicSource;
    }

    public void IncreaseVolume(float interval){
        sfxVolume += interval;
        musicVolume += interval;

        UpdateAudioSourcesVolume();
    }

    public void DecreaseVolume(float interval){
        sfxVolume -= interval;
        musicVolume -= interval;

        UpdateAudioSourcesVolume();
    }

    void UpdateAudioSourcesVolume(){
        musicSource.volume = musicVolume;
    }

    public void PlaySong(string songName){
        musicSource.clip = songs[songName];
        musicSource.Play();
    }

    public void StopSong(AudioSource musicPlayer){
        musicPlayer.Stop();
    }
}
