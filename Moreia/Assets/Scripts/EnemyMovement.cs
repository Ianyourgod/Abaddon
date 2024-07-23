using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    [Header("References")]
    [SerializeField] LayerMask collideLayers;

    [Header("Attributes")]
    public float detectionDistance = 1f;

    void Start()
    {
        Controller.OnTick += MakeDecision;
    }

    public bool CheckPlayerIsInRange()
    {
        return UnityEngine.Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
    }

    void MakeDecision()
    {
        if (CheckPlayerIsInRange()){
            Move();
            return;
        }
    }

    void Move()
    {

        // todo: deadzone if we add controller support?
        sbyte horizontal, vertical;

        (horizontal, vertical) = ToPlayer();

        if ((horizontal != 0 && vertical != 0) || (horizontal == 0 && vertical == 0))
            return;

        // get the direction we are moving
        Direction direction =
            horizontal == 0 ?
            (vertical == 1 ? Direction.Up : Direction.Down) :
            (horizontal == 1 ? Direction.Right : Direction.Left);

        if (IsValidMove(direction))
        {
            transform.Translate(horizontal, vertical, 0);
            OnTick?.Invoke();
        }
    }

    public static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private (sbyte, sbyte) ToPlayer()
    {
        float raw_horizontal = Clamp(Controller.main.transform.position.x - transform.position.x, -1.0f, 1f);
        float raw_vertical = Clamp(Controller.main.transform.position.y - transform.position.y, -1.0f, 1f);

        if (raw_horizontal != 0 && raw_vertical != 0) {
            raw_vertical = 0f;
        }

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

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, detectionDistance);
    }
}
