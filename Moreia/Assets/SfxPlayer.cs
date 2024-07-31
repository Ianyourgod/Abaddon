using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] string name;

    [Header("References")]
    [SerializeField] SpriteRenderer spriteRend;

    [Header("Attributes")]
    public AudioClip[] sfxList;
    public bool playableOffScreen = false;
    public bool playFromRandom = true;
    [SerializeField] float playLouderAmount = 0f;
    [SerializeField] bool playerSound = false;

    private int randomGenerator;

    public void PlaySfx()
    {
        if (playableOffScreen || (!playableOffScreen && spriteRend.isVisible))
        {
            if (playFromRandom)
            {
                randomGenerator = Random.Range(0, sfxList.Length);
            }
            else
            {
                randomGenerator = 0;
            }

            ChooseAudioSource();
        }
    }

    private void ChooseAudioSource()
    {
        if (playerSound)
        {
            AudioManager.main.PlayPlayerSFX(sfxList[randomGenerator], playLouderAmount);
        }
        else
        {
            AudioManager.main.PlayOtherSFX(sfxList[randomGenerator], playLouderAmount);
        }
    }
}
