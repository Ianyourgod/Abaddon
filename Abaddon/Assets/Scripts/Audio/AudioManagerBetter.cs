using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerBetter : MonoBehaviour
{
    public static AudioManagerBetter main;

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
    AudioSource[] audioSources; //first index is always the music audio source, others are for sfx

    void Awake()
    {
        main = this;

        songs = new Dictionary<string, AudioClip>();
        for(int i = 0; i < songKeys.Length; i++){
            songs[songKeys[i]] = songValues[i];
        }

        audioSources[0] = new AudioSource();
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
        audioSources[0].volume = musicVolume;
        for(int i = 1; i < audioSources.Length; i++){
            audioSources[i].volume = sfxVolume;
        }
    }

    public void PlaySong(string songName){
        audioSources[0].clip = songs[songName];
        audioSources[0].Play();
    }

    public void StopSong(AudioSource musicPlayer){
        musicPlayer.Stop();
    }
}
