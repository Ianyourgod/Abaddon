using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFightCollider : MonoBehaviour
{
    [HideInInspector] public bool playerInside = false;
    [SerializeField] Gate gate;
    [SerializeField] Boss1 boss;
    [SerializeField] CameraScript cam;

    private void Start()
    {
        Controller.main.onDie += onDie;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerInside) return;

        if (collision.gameObject.tag == "Player")
        {
            playerInside = true;
            gate.Close();
            // focus on boss
            cam.PanToSmooth(boss.transform.position, 1f, 1f);
            boss.StartFight();
        }
    }

    private void onDie() {
        if (playerInside) {
            gate.Open();
        }
    }
}
