using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : MonoBehaviour
{
    [SerializeField] uint damage = 2;

    void Awake() {
        Controller.OnTick += CustomUpdate;
    }

    void CustomUpdate() {
        if (!this) return;
        if (transform.position == Controller.main.transform.position) {
            Controller.main.DamagePlayer(damage, false);
        }
    }
}