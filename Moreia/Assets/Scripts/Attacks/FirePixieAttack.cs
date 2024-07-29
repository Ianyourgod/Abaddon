using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePixieAttack : BaseAttack {
    [SerializeField] GameObject fireball_prefab;

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
                new_position.x += 0.5f;
                break;
            case EnemyMovement.Direction.Right:
                new_position.x -= 0.5f;
                break;
        }
        Instantiate(fireball_prefab, new_position, Quaternion.identity);
    }
}