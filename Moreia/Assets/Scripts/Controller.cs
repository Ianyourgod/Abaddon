using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller main;

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

    void Awake()
    {
        main = this;
    }

    void Update()
    {
        Move();
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

        if (IsValidMove(direction) && Time.time - lastMovement > movementDelay) {
            transform.Translate(horizontal, vertical, 0);
            lastMovement = Time.time;
            OnTick?.Invoke();
        }
    }

    sbyte BoolToSbyte(bool value)
    {
        return (sbyte) (value ? 1 : 0);
    }

    (sbyte, sbyte) GetAxis()
    {
        // todo: allow people to rebind movement keys
        sbyte up = BoolToSbyte(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow));
        sbyte down = BoolToSbyte(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow));
        sbyte left = BoolToSbyte(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow));
        sbyte right = BoolToSbyte(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow));

        sbyte horizontal = (sbyte) (right - left); // sbyte is int8
        sbyte vertical = (sbyte) (up - down); // sbyte is int8

        return (horizontal, vertical);
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