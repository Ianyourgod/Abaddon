using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerBetter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private string songName = "level1";

    void Start()
    {
        AudioManagerBetter.main.SetLooping(true);
        AudioManagerBetter.main.PlaySong(songName);
    }

    // Update is called once per frame
    void Update() { }
}
