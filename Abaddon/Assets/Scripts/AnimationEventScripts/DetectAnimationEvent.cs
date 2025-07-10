using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAnimationEvent : MonoBehaviour
{
    [SerializeField]
    EnemyMovement enemyMovement;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    private Vector2 GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    public void AttackTiming(Direction direction)
    {
        enemyMovement.AttackTiming(GetDirectionVector((direction)));
    }

    public void AttackEnd(Direction direction)
    {
        enemyMovement.AttackEnd(GetDirectionVector((direction)));
    }

    public void Die()
    {
        enemyMovement.Die();
    }
}
