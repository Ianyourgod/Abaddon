using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyMovement : MonoBehaviour
{
    int movementPriority;

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
    [SerializeField] float detectionDistance = 1f;
    [SerializeField] float enemyDecisionDelay;
    [SerializeField] uint attackDamage = 1;

    void Start() {
        Controller.OnTick += MakeDecision;
    }

    public bool CheckPlayerIsInRange() {
        return UnityEngine.Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
    }

    void MakeDecision() {
        if (CheckPlayerIsInRange()){
            Invoke(nameof(Move), enemyDecisionDelay);
        }
    }

    void Move() {
        sbyte horizontal, vertical;

        (horizontal, vertical) = ToPlayer();

        if ((horizontal != 0 && vertical != 0) || (horizontal == 0 && vertical == 0))
            return;

        // get the direction we are moving
        Direction direction =
            horizontal == 0 ?
            (vertical == 1 ? Direction.Up : Direction.Down) :
            (horizontal == 1 ? Direction.Right : Direction.Left);

        Collider2D hit = IsValidMove(direction);

        if (hit == null) {
            transform.Translate(horizontal, vertical, 0);
        } else {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player")) {
                Controller.main.DamagePlayer(attackDamage);
            }
        }
    }

    private static float Clamp(float value, float min, float max) {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private (sbyte, sbyte) ToPlayer() {
        float raw_horizontal = Clamp(Controller.main.transform.position.x - transform.position.x, -1.0f, 1f);
        float raw_vertical = Clamp(Controller.main.transform.position.y - transform.position.y, -1.0f, 1f);

        if (raw_horizontal != 0 && raw_vertical != 0) {
            if (Random.Range(0,2) == 0) { // ints are exclusive on the second input, when they're floats they aint, unity sucks balls 
                raw_horizontal = 0f;
            }
            else {
                raw_vertical = 0f;
            }
        }

        sbyte horizontal = (sbyte)Mathf.Round(raw_horizontal); // sbyte is int8
        sbyte vertical = (sbyte)Mathf.Round(raw_vertical); // sbyte is int8

        return (horizontal, vertical);
    }

    private Collider2D IsValidMove(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return Physics2D.Raycast(transform.position, transform.up, 1f, collideLayers).collider;
            case Direction.Down:
                return Physics2D.Raycast(transform.position, -transform.up, 1f, collideLayers).collider;
            case Direction.Left:
                return Physics2D.Raycast(transform.position, -transform.right, 1f, collideLayers).collider;
            case Direction.Right:
                return Physics2D.Raycast(transform.position, transform.right, 1f, collideLayers).collider;
        }
        return null;
    }

    private void OnDrawGizmosSelected() {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, detectionDistance);
    }
}
