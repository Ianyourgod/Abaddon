using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager main;

    [Header("References")]
    public AudioClip[] songs;
    AudioSource musicAudSource;
    public GameObject player;

    private void Awake()
    {
        main = this;
        musicAudSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        musicAudSource.volume = AudioManager.main.musicVolume;
    }

    public void PlaySong(int song)
    {
        musicAudSource.clip = songs[song];
        musicAudSource.Play();
    }
}
