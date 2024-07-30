using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePixieAttack : BaseAttack {
    [SerializeField] GameObject fireball_prefab;
    [SerializeField] LayerMask playerLayer;

    public override bool WillAttack(Collider2D collider, EnemyMovement.Direction direction) {
        // base stuff
        if (collider != null && collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            return true;
        }

        // now check if the player is at most 1 unit away
        return IsPlayerThere(direction);
    }

    private bool IsPlayerThere(EnemyMovement.Direction direction) {
        switch (direction) {
            case EnemyMovement.Direction.Up:
                return Physics2D.Raycast(transform.position, transform.up, 2f, playerLayer).collider != null;
            case EnemyMovement.Direction.Down:
                return Physics2D.Raycast(transform.position, -transform.up, 2f, playerLayer).collider != null;
            case EnemyMovement.Direction.Left:
                return Physics2D.Raycast(transform.position, -transform.right, 2f, playerLayer).collider != null;
            case EnemyMovement.Direction.Right:
                return Physics2D.Raycast(transform.position, transform.right, 2f, playerLayer).collider != null;
        }
        return false;
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
        Instantiate(fireball_prefab, new_position, Quaternion.identity);
    }
}