using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Tooltip("Reference the scene's AudioManager to see each song's number.")]
    [SerializeField] int songNumber;
    [SerializeField] bool playOnStart;
    [SerializeField] bool playOnCollision;
    private GameObject player;
    bool activated = false;

    void Start()
    {
        if (playOnStart)
        {
            MusicManager.main.PlaySong(songNumber);
        }
        if (playOnCollision)
        {
            player = MusicManager.main.player;
        }
    }

    void Update()
    {
        if (playOnCollision)
        {
            bool touching = player.transform.position == transform.position;

            if (touching && !activated)
            {
                MusicManager.main.PlaySong(songNumber);
                activated = true;
            }
        }
    }
}
