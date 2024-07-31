using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [Header("Attributes")]
    public AudioClip[] sfxList;
    [SerializeField] bool randomSoundFromList = true;

    private int randomGenerator;

    public void PlaySfx()
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
