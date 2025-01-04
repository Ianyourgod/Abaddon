using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFightCollider : MonoBehaviour
{
    [HideInInspector] public bool playerInside = false;
    [SerializeField] Gate gate;

    private void Start()
    {
        Controller.main.onDie += onDie;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInside = true;
            gate.Close();
        }
    }

    private void onDie() {
        if (playerInside) {
            gate.Open();
        }
    }
}
