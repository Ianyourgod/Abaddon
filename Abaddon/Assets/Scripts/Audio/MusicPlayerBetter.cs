using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerBetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManagerBetter.main.PlaySong("level1");
    }

    // Update is called once per frame
    void Update() { }
}
