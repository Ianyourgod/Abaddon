using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SpriteRenderer spriteRend;

    [Header("Attributes")]
    public AudioClip[] sfxList;
    [SerializeField] bool randomSoundFromList = true;
    public bool playableOffScreen = false;

    private int randomGenerator;

    public void PlaySfx()
    {
        if (playableOffScreen || (!playableOffScreen && spriteRend.isVisible))
        {
            if (randomSoundFromList)
            {
                randomGenerator = Random.Range(0, sfxList.Length);

                AudioManager.main.PlaySFX(sfxList[randomGenerator]);
            }
            else
            {
                AudioManager.main.PlaySFX(sfxList[0]);
            }
        }
    }
}
