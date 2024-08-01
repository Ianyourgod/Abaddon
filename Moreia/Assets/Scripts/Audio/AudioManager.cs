using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    [Header("Attributes")]
    public float musicVolume = 0.02f;
    public float sfxVolume = 0.02f;
    [SerializeField] float maxVolume = 1f;
    [SerializeField] KeyCode increaseKey;
    [SerializeField] KeyCode decreaseKey;

    void Awake(){
        main =  this;
    }

    void Update(){
        if (Input.GetKeyDown(increaseKey) && (musicVolume + 0.01f) <= maxVolume)
        {
            IncreaseVolume(0.01f);
        }
        if (Input.GetKeyDown(decreaseKey) && (musicVolume - 0.01f) >= 0f)
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
}
