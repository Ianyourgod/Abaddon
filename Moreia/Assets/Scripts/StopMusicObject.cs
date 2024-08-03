using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMusicObject : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("stopped");
        var audManager = AudioManager.main;
        Debug.Log("stopped 2");
        var musicManager = audManager.musicManager;
        Debug.Log("stopped 3");
        var musicSource = musicManager.musicAudSource;
        Debug.Log("stopped 4");
        Debug.Log(musicSource);
        musicSource.Stop();
    }
}
