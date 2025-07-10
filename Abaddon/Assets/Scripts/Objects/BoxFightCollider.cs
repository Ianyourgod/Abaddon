using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFightCollider : MonoBehaviour
{
    [HideInInspector]
    public bool playerInside = false;

    [SerializeField]
    Gate gate;

    [SerializeField]
    Boss1 boss;

    [SerializeField]
    CameraScript cam;

    private void Start()
    {
        if (Controller.main == null)
            return;

        Controller.main.onDie += onDie;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerInside)
            return;

        if (collision.gameObject.tag == "Player")
        {
            playerInside = true;
            gate.Close();
            cam.ChangeTarget(
                boss.transform,
                2f,
                onComplete: () =>
                {
                    cam.UpdateFOV(
                        7f,
                        2f,
                        onComplete: () =>
                        {
                            boss.StartFight();
                        }
                    );
                }
            );
        }
    }

    private void onDie()
    {
        if (playerInside)
        {
            gate.Open();
        }
    }
}
