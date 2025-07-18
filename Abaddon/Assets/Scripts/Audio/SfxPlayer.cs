using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxPlayer : MonoBehaviour
{
    [Header("SfxPlayer References")]
    [Tooltip("The audio source on the object that will play the sound effects")]
    [HideInInspector]
    public AudioSource audSource;

    private SpriteRenderer sprRend;

    [Header("SfxPlayer Attributes")]
    [Tooltip(
        "(If the object is a player, ignore this) Determines whether the sound effects can be heard when the object is off screen"
    )]
    public bool playableOffScreen = false;

    private int randomGenerator;

    void Awake()
    {
        audSource = GetComponent<AudioSource>();

        if (GetComponent<SpriteRenderer>())
        {
            sprRend = GetComponent<SpriteRenderer>();
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<SpriteRenderer>())
                {
                    sprRend = transform.GetChild(i).GetComponent<SpriteRenderer>();
                    break;
                }
            }
        }
    }

    public void PlaySfx(AudioClip soundFx, float addedSound = 0f)
    {
        if (soundFx == null)
            return;

        if (playableOffScreen || (sprRend != null && sprRend.isVisible))
        {
            if (AudioManagerBetter.main == null)
                return;

            if (AudioManagerBetter.main.sfxVolume == 0)
                addedSound = 0;

            audSource.PlayOneShot(soundFx, AudioManagerBetter.main.sfxVolume + addedSound);
        }
    }

    public void PlayRandomSound(AudioClip[] audioClips, float addedSound = 0f)
    {
        randomGenerator = Random.Range(0, audioClips.Length);

        PlaySfx(audioClips[randomGenerator], addedSound);
    }
}
