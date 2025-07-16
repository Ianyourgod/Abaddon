using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManagerBetter : MonoBehaviour
{
    public static AudioManagerBetter main;

    [Header("References")]
    [SerializeField]
    public AudioSource deathSfxPlayer;
    public AudioSource musicSource;

    [Header("Attributes")]
    [SerializeField]
    public float sfxVolume = 0.02f;

    [SerializeField]
    public float musicVolume = 0.005f;

    [Header("Music Dictionary")]
    [Tooltip(
        "Keys and values must be at same indexes, and keys and values lists must be the same length"
    )]
    [SerializeField]
    string[] songKeys;

    [SerializeField]
    AudioClip[] songValues;

    Dictionary<string, AudioClip> songs;

    [SerializeField]
    List<SfxSource> sfxSources;

    void Awake()
    {
        main = this;

        songs = new Dictionary<string, AudioClip>();
        for (int i = 0; i < songKeys.Length; i++)
        {
            songs[songKeys[i]] = songValues[i];
        }

        sfxSources = new List<SfxSource>();
    }

    void Update() { }

    public AudioClip GetSong(string key)
    {
        return songs[key];
    }

    public AudioSource GetMusicSource()
    {
        return musicSource;
    }

    public void IncreaseVolume(float interval)
    {
        sfxVolume += interval;
        musicVolume += interval;

        UpdateAudioSourcesVolume();
    }

    public void SetMusicVolume(float newVolume)
    {
        musicVolume = newVolume;
        UpdateAudioSourcesVolume();
    }

    public void SetSfxVolume(float newVolume)
    {
        sfxVolume = newVolume;
        UpdateAudioSourcesVolume();
    }

    public void DecreaseVolume(float interval)
    {
        sfxVolume -= interval;
        musicVolume -= interval;

        UpdateAudioSourcesVolume();
    }

    void UpdateAudioSourcesVolume()
    {
        musicSource.volume = musicVolume;
        foreach (SfxSource currentSource in sfxSources)
        {
            currentSource.SetVolume(sfxVolume);
        }
    }

    public void PlaySong(string songName)
    {
        musicSource.clip = songs[songName];
        // it plays the song from here (and in one other spot) search for this comment
        musicSource.Play();
    }

    public void StopSong(AudioSource musicPlayer)
    {
        musicPlayer.Stop();
    }

    public void PopSfxSource(int index)
    {
        Destroy(sfxSources[index].Source());
        Destroy(sfxSources[index]);
        sfxSources.RemoveAt(index);
        for (int i = 0; i < sfxSources.Count; i++)
        {
            sfxSources[i].SetIndex(i);
        }
    }

    public void PlaySfx(AudioClip sound)
    {
        sfxSources.Add(gameObject.AddComponent<SfxSource>());
        sfxSources[^1].Play(sound);
    }
}
