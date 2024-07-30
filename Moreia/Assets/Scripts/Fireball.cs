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
    }

    void CustomUpdate() {
        timeAlive++;
        if (timeAlive > lastingTime) {
            Destroy(gameObject);
        }

        // check if we're colliding with the player
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Player"));

        if (collider != null) {
            Controller.main.DamagePlayer(damage);
        }
    }
}