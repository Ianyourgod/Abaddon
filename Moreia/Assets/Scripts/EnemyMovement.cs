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
    [SerializeField] Animator animator;

    [Header("Attributes")]
    [SerializeField] float detectionDistance = 1f;
    [SerializeField] float enemyDecisionDelay;
    [SerializeField] uint attackDamage = 1;

    public static uint Attacking = 0;
    public static uint health = 10;
    private Direction direction = Direction.Down;

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
        direction =
            horizontal == 0 ?
            (vertical == 1 ? Direction.Up : Direction.Down) :
            (horizontal == 1 ? Direction.Right : Direction.Left);

        Collider2D hit = IsValidMove(direction);
        PlayAnimation(direction, 1);

        if (hit == null && Attacking == 0) {
            transform.Translate(horizontal, vertical, 0);
        } else {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player") && Attacking == 0)
            {
                animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
                Controller.main.DamagePlayer(attackDamage);
                Attacking = 1;
                StartCoroutine(ExecuteAfterTime(1f, direction, 0));
                PlayAnimation(direction, 3);
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

    // 1 is idle, 2 is hurt, 3 is attack
    private void PlayAnimation(Direction direction, uint action)
    {
        switch (direction)
        {
            case Direction.Up:
                switch (action)
                {
                    case 1:
                        animator.Play("Goblin_animation_back_idle");
                        break;
                    case 2:
                        animator.Play("Goblin_animation_back_hurt");
                        break;
                    case 3:
                        transform.Translate(0, 0.5f, 0);
                        animator.Play("Goblin_animation_back_attack");
                        break;
                }
                break;
            case Direction.Down:
                switch (action)
                {
                    case 1:
                        animator.Play("Goblin_animation_front_idle");
                        break;
                    case 2:
                        animator.Play("Goblin_animation_front_hurt");
                        break;
                    case 3:
                        transform.Translate(0, -0.5f, 0);
                        animator.Play("Goblin_animation_front_attack");
                        break;
                }
                break;
            case Direction.Left:
                switch (action)
                {
                    case 1:
                        animator.Play("Goblin_animation_left_idle");
                        break;
                    case 2:
                        animator.Play("Goblin_animation_left_hurt");
                        break;
                    case 3:
                        animator.Play("Goblin_animation_left_attack");
                        break;
                }
                break;
            case Direction.Right:
                switch (action)
                {
                    case 1:
                        animator.Play("Goblin_animation_right_idle");
                        break;
                    case 2:
                        animator.Play("Goblin_animation_right_hurt");
                        break;
                    case 3:
                        animator.Play("Goblin_animation_right_attack");
                        break;
                }
                break;
        }
    }

    private void OnDrawGizmosSelected() {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, detectionDistance);
    }

    public void DamageEnemy(uint damage, string targetTag) {
    if (targetTag == gameObject.tag) {
        if (damage >= health)
        {
            health = 0;
            // prevent the player from locking up after death
            Attacking = 0;
            Destroy(gameObject);
            return;
            }
            PlayAnimation(direction, 2);
            StartCoroutine(ExecuteAfterTime(0.25f, direction, 1));
            Debug.Log("ouch");
        health -= damage;
    }
    }

    // intent 0 is attack, 1 is hurt
    IEnumerator ExecuteAfterTime(float time, Direction direction, uint intent)
    {
        yield return new WaitForSeconds(time);

        switch (intent)
        {
            case 0:
                animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
                Attacking = 0;
                PlayAnimation(direction, 1);
                switch (direction) {
                    case Direction.Up:
                        transform.Translate(0, -0.5f, 0);
                        break;
                    case Direction.Down:
                        transform.Translate(0, 0.5f, 0);
                        break;
                    default:
                        break;
                }
                break;
            case 1:
                PlayAnimation(direction, 1);
                break;
            default:
                break;
        }
    }
}
