using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager main;

    [Header("References")]
    public AudioClip[] songs;
    public AudioSource musicAudSource;
    [HideInInspector] public GameObject player;

    private void Awake()
    {
        main = this;
        musicAudSource = GetComponent<AudioSource>();

        try
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
        }
        catch
        {
            player = null;
        }
    }

    private void Update()
    {
        if (musicAudSource == null || AudioManager.main == null) return;

        musicAudSource.volume = AudioManager.main.musicVolume;
    }

    public void PlaySong(int song)
    {
        musicAudSource.clip = songs[song];
        musicAudSource.Play();
    }
}
