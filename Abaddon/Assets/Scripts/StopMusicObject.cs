using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMusicObject : MonoBehaviour
{
    void Awake()
    {
        var audManager = AudioManager.main;
        var musicManager = audManager.musicManager;
        var musicSource = musicManager.musicAudSource;
        musicSource.Stop();
    }
}
