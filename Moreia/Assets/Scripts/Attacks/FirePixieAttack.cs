using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePixieAttack : BaseAttack {
    [SerializeField] GameObject fireball_prefab;
    [SerializeField] int detectionDistance = 2;

    public override bool WillAttack(Collider2D collider, EnemyMovement.Direction direction) {
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

    private bool IsFireballThere(EnemyMovement.Direction direction) {
        Vector3 dir = transform.up;

        switch (direction) {
            case EnemyMovement.Direction.Up:
                dir = transform.up;
                break;
            case EnemyMovement.Direction.Down:
                dir = -transform.up;
                break;
            case EnemyMovement.Direction.Left:
                dir = -transform.right;
                break;
            case EnemyMovement.Direction.Right:
                dir = transform.right;
                break;
        }

        Collider2D collider = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask("Fireball")).collider;
        return collider != null;
    }

    private bool IsPlayerThere() {
        return UnityEngine.Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
    }

    public override void Attack(EnemyMovement.Direction direction) {
        Vector3 new_position = transform.position;

        switch (direction) {
            case EnemyMovement.Direction.Up:
                new_position.y += 0.5f;
                break;
            case EnemyMovement.Direction.Down:
                new_position.y -= 0.5f;
                break;
            case EnemyMovement.Direction.Left:
                new_position.x -= 1.0f;
                break;
            case EnemyMovement.Direction.Right:
                new_position.x += 1.0f;
                break;
        }
        sfxPlayer.PlayAttackSound();
        
        Instantiate(fireball_prefab, new_position, Quaternion.identity);
    }
}