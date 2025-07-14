using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    [Header("References")]
    public AudioSource deathSfxPlayer;

    [Header("Attributes")]
    public float musicVolume = 0.02f;
    public float sfxVolume = 0.02f;

    [SerializeField]
    float maxVolume = 1f;

    [HideInInspector]
    public MusicManager musicManager;

    void Awake()
    {
        main = this;
        musicManager = GetComponent<MusicManager>();

        //GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioManager");

        //if (objs.Length > 1)
        //{
        //Destroy(this.gameObject);
        //}

        //DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (
            Input.GetKeyDown(SettingsMenu.singleton.increaseVolumeKeybind.key)
            && (musicVolume + 0.01f) <= maxVolume
        )
        {
            IncreaseVolume(0.01f);
        }
        if (
            Input.GetKeyDown(SettingsMenu.singleton.decreaseVolumeKeybind.key)
            && (musicVolume - 0.01f) >= 0f
        )
        {
            DecreaseVolume(0.01f);
        }
    }

    public void IncreaseVolume(float amount)
    {
        musicVolume += amount;
        sfxVolume += amount;
    }

    public void DecreaseVolume(float amount)
    {
        musicVolume -= amount;
        sfxVolume -= amount;
    }

    public void StopTheMusic()
    {
        musicManager.musicAudSource.Stop();
    }
}
