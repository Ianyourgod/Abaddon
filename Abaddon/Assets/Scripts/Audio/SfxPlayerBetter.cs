using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayerBetter : MonoBehaviour
{
    [Header("References")]
    [Tooltip(
        "Keys and values must be at same indexes, and keys and values lists must be the same length"
    )]
    [SerializeField]
    string[] soundKeys;

    [SerializeField]
    AudioClip[] soundValues;

    Dictionary<string, AudioClip> soundEffects;

    void Awake()
    {
        soundEffects = new Dictionary<string, AudioClip>();
        for (int i = 0; i < soundKeys.Length; i++)
        {
            soundEffects[soundKeys[i]] = soundValues[i];
        }
    }

    public void PlaySound(string key)
    {
        if (soundEffects.ContainsKey(key) && AudioManagerBetter.main != null)
        {
            AudioManagerBetter.main.PlaySfx(soundEffects[key]);
        }
        else
        {
            print($"gameobject {gameObject.name} invalid sound byte key: {key}");
        }
    }

    public void PlayRandomSound(string baseKey)
    {
        int baseKeyAmount = 0;
        foreach ((string key, AudioClip value) in soundEffects)
        {
            if (key.Contains(baseKey))
            {
                baseKeyAmount++;
            }
        }
        // var rand = new Random();
    }
}
