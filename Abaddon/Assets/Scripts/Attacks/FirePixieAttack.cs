using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePixieAttack : BaseAttack {
    [SerializeField] GameObject fireball_prefab;
    [SerializeField] int detectionDistance = 2;

    public override bool WillAttack(Collider2D collider, Vector2 direction) {
        // base stuff
        if (IsFireballThere(direction)) {
            return false;
        }

        if (collider != null && collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            return true;
        }

        // now check if the player is at most 1 unit away
        return IsPlayerThere();
    }

    private bool IsFireballThere(Vector2 direction) {
        return Physics2D.OverlapCircle(transform.position + new Vector3(direction.x, direction.y, 0), 0.1f, LayerMask.GetMask("Fireball")) != null;
    }

    private bool IsPlayerThere() {
        return UnityEngine.Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
    }

    public void Attack(Vector2 direction) {
        Vector3 new_position = transform.position;

        new_position += new Vector3(direction.x, direction.y, 0);
        sfxPlayer.PlayAttackSound();
        
        Instantiate(fireball_prefab, new_position, Quaternion.identity);
    }
}