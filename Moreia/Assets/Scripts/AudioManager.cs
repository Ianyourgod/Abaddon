using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    [Header("References")]
    public AudioClip[] songs;
    [SerializeField] AudioSource musicAudSource;
    [SerializeField] AudioSource sfxAudSource;
    public GameObject player;

    [Header("Attributes")]
    [SerializeField] float volume = 0.05f;
    [SerializeField] float maxVolume = 1f;
    [SerializeField] KeyCode increaseKey;
    [SerializeField] KeyCode decreaseKey;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        musicAudSource.volume = volume;

        if (Input.GetKeyDown(increaseKey) && (volume + 0.01f) <= maxVolume)
        {
            IncreaseVolume(0.01f);
        }
        if (Input.GetKeyDown(decreaseKey) && (volume - 0.01f) >= 0f)
        {
            DecreaseVolume(0.01f);
        }
    }

    public void IncreaseVolume(float amount)
    {
        volume += amount;
    }

    public void DecreaseVolume(float amount)
    {
        volume -= amount;
    }

    public void PlaySong(int song)
    {
        musicAudSource.clip = songs[song];
        musicAudSource.Play();
    }

    public void PlaySFX(AudioClip sfxSound)
    {
        sfxAudSource.PlayOneShot(sfxSound, volume);
    }
}
