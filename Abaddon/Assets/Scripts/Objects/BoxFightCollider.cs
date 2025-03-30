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
            print("Focusing on boss");
            cam.ChangeTarget(boss.transform, 1f, () => print("Boss fight started"));
            boss.StartFight();
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
