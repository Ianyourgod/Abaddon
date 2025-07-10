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
        Debug.Log($"I am {gameObject.name}");
        Debug.Log($"I have {soundKeys.Length} sound keys and {soundValues.Length} sound values");
        soundEffects = new Dictionary<string, AudioClip>();
        for (int i = 0; i < soundKeys.Length; i++)
        {
            soundEffects[soundKeys[i]] = soundValues[i];
        }
        Debug.Log($"I have {soundEffects.Count} sound effects");
    }

    public void PlaySound(string key)
    {
        Debug.Log($"I am {gameObject.name} and I am playing sound with key: {key}");
        Debug.Log(
            $"I think that soundEffects does contain key: {soundEffects.ContainsKey(key)} and that AudioManagerBetter.main is: {AudioManagerBetter.main != null}"
        );
        if (soundEffects.ContainsKey(key) && AudioManagerBetter.main != null)
        {
            AudioManagerBetter.main.PlaySfx(soundEffects[key]);
        }
        else
        {
            print("invalid sound byte key: " + key);
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
