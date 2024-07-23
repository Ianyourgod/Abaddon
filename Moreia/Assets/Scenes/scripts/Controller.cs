using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] LayerMask collideLayers;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.W) && IsValidMove(Direction.Up))
        {
            transform.Translate(0, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.A) && IsValidMove(Direction.Left))
        {
            transform.Translate(-1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.S) && IsValidMove(Direction.Down))
        {
            transform.Translate(0, -1, 0);
        }
        if (Input.GetKeyDown(KeyCode.D) && IsValidMove(Direction.Right))
        {
            transform.Translate(1, 0, 0);
        }
    }

    bool IsValidMove(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Physics2D.Raycast(transform.position, transform.up, 1f, collideLayers).collider == null;
            case Direction.Down:
                return Physics2D.Raycast(transform.position, -transform.up, 1f, collideLayers).collider == null;
            case Direction.Left:
                return Physics2D.Raycast(transform.position, -transform.right, 1f, collideLayers).collider == null;
            case Direction.Right:
                return Physics2D.Raycast(transform.position, transform.right, 1f, collideLayers).collider == null;
        }
        return false;
    }
}