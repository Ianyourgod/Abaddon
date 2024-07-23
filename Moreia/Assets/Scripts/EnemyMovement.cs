using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public delegate void TickAction();
    public static event TickAction OnTick;

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private float lastMovement = 0f;

    [SerializeField] LayerMask collideLayers;
    [SerializeField] float movementDelay = 0.1f;

    void Start()
    {
        Controller.OnTick += MakeDecision;
    }

    void MakeDecision()
    {
        return;
    }

    void Move()
    {

        // todo: deadzone if we add controller support?
        sbyte horizontal, vertical;

        (horizontal, vertical) = GetAxis();

        if ((horizontal != 0 && vertical != 0) || (horizontal == 0 && vertical == 0))
            return;

        // get the direction we are moving
        Direction direction =
            horizontal == 0 ?
            (vertical == 1 ? Direction.Up : Direction.Down) :
            (horizontal == 1 ? Direction.Right : Direction.Left);

        if (IsValidMove(direction) && Time.time - lastMovement > movementDelay)
        {
            transform.Translate(horizontal, vertical, 0);
            lastMovement = Time.time;
            OnTick?.Invoke();
        }
    }

    private (sbyte, sbyte) GetAxis()
    {
        float raw_horizontal = Input.GetAxisRaw("Horizontal");
        float raw_vertical = Input.GetAxisRaw("Vertical");

        // todo: deadzone if we add controller support?
        sbyte horizontal = (sbyte)Mathf.Round(raw_horizontal); // sbyte is int8
        sbyte vertical = (sbyte)Mathf.Round(raw_vertical); // sbyte is int8

        return (horizontal, vertical);
    }

    private bool IsValidMove(Direction direction)
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
