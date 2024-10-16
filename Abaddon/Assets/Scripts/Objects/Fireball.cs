using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] uint lastingTime = 2;
    [SerializeField] uint damage = 8;

    private uint timeAlive = 0;

    void Awake() {
        Controller.OnTick += CustomUpdate;
        if (transform.position == Controller.main.transform.position) {
            Controller.main.DamagePlayer(damage, false);
        }
    }

    void OnDestroy() {
        Controller.OnTick -= CustomUpdate;
    }

    void CustomUpdate() {
        timeAlive++;
        if (timeAlive > lastingTime) {
            Destroy(gameObject);
        }

        if (transform.position == Controller.main.transform.position) {
            Controller.main.DamagePlayer(damage, false);
        }
    }
}