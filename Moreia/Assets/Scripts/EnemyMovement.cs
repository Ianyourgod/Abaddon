using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Pathfinding2D))]
[RequireComponent(typeof(EnemySfx))]

public class EnemyMovement : MonoBehaviour
{
    public enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    [Header("References")]
    [SerializeField] LayerMask collideLayers;
    [SerializeField] Animator animator;
    [SerializeField] Pathfinding2D pathfinding;
    [SerializeField] string animation_prefix = "Goblin";
    [SerializeField] BaseAttack attack;
    [SerializeField] Breakable breakableLogic;
    EnemySfx sfxPlayer;

    [Header("Attributes")]
    [SerializeField] int detectionDistance = 1;
    [SerializeField] float followDistance = 3f;
    [SerializeField] float enemyDecisionDelay;

    public uint health = 10;
    private Direction direction = Direction.Down;

    private Vector2 movementTarget;
    private bool followingPlayer = false;
    private bool directionChange = false;

    private Vector3 StartPosition;

    private void Awake(){
        sfxPlayer = GetComponent<EnemySfx>();
    }

    private void Start() {
        StartPosition = transform.position;
        int gridSize = (int) (detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeX = gridSize;
        pathfinding.grid.gridSizeY = gridSize;
    }

    bool CheckPlayerIsInDetectionRange() {
        return UnityEngine.Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
    }

    bool CheckPlayerIsInFollowRange() {
        float distance = UnityEngine.Vector2.Distance(Controller.main.transform.position, StartPosition);
        return distance <= followDistance;
    }

    public void MakeDecision() {
        bool inFollowRange = CheckPlayerIsInFollowRange();
        bool inDetectionRange = CheckPlayerIsInDetectionRange();
        bool atHome = transform.position == StartPosition;

        if (inDetectionRange && inFollowRange) {
            Invoke(nameof(MoveToPlayer), enemyDecisionDelay);
        } else if (!inFollowRange && !atHome) {
            (sbyte horizontal, sbyte vertical) = ToHome();
            
            if ((horizontal != 0 && vertical != 0) || (horizontal == 0 && vertical == 0)) {
                Invoke(nameof(callNextEnemy), 0f);
                return;
            }
            direction =
                horizontal == 0 ?
                (vertical == 1 ? Direction.Up : Direction.Down) :
                (horizontal == 1 ? Direction.Right : Direction.Left);

            Move(direction);
        } else {
            Invoke(nameof(callNextEnemy), 0f);
        }
    }

    private void callNextEnemy() {
        Controller.main.NextEnemy();
    }

    void MoveToPlayer() {
        sbyte horizontal, vertical;

        if (CheckPlayerIsInDetectionRange()) {
            followingPlayer = true;
            movementTarget = Controller.main.transform.position;
        } else if (followingPlayer && !CheckPlayerIsInFollowRange()) {
            followingPlayer = false;
            Invoke(nameof(callNextEnemy), 0f);
            return;
        }

        (horizontal, vertical) = ToPlayer();

        // the diagonal case *probably* shouldn't happen, but just in case
        if ((horizontal != 0 && vertical != 0) || (horizontal == 0 && vertical == 0)) {
            Invoke(nameof(callNextEnemy), 0f);
            return;
        }

        // get the direction we are moving
        direction =
            horizontal == 0 ?
            (vertical == 1 ? Direction.Up : Direction.Down) :
            (horizontal == 1 ? Direction.Right : Direction.Left);

        Move(direction);
    }

    private Vector2 DirectionToVector(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return Vector2.up;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
        }
        return Vector2.zero;
    }

    private void Move(Direction direction) {
        Collider2D hit = IsValidMove(direction);
        PlayAnimation(direction, 1);

        bool willAttack = attack.WillAttack(hit, direction);

        if (willAttack) {
            Controller.main.enabled = false;
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
            PlayAnimation(direction, 3);
        } else if (hit == null) {
            Vector2 movement = DirectionToVector(direction);
            sfxPlayer.PlayWalkSound();
            transform.Translate(movement.x, movement.y, 0);
            Invoke(nameof(callNextEnemy), 0f);
        } else {
            Invoke(nameof(callNextEnemy), 0f);
        }
    }

    private static float Clamp(float value, float min, float max) {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private (sbyte, sbyte) ToPlayer() {
        List<Node2D> path = pathfinding.FindPath(transform.position, Controller.main.transform.position);

        if (path == null) {
            return (0, 0);
        }

        // shouldnt happen but just in case
        if (path.Count == 0) {
            return (0, 0);
        }

        // get the relative position of the next node
        Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

        nextNode.y -= detectionDistance;
        nextNode.x -= detectionDistance;

        float raw_horizontal = Clamp(nextNode.x, -1.0f, 1f);
        float raw_vertical = Clamp(nextNode.y, -1.0f, 1f);

        if (raw_horizontal != 0 && raw_vertical != 0) {
            if (directionChange) {
                directionChange = false;
                raw_horizontal = 0f;
            } else {
                directionChange = true;
                raw_vertical = 0f;
            }
        }

        sbyte horizontal = (sbyte)Mathf.Round(raw_horizontal); // sbyte is int8
        sbyte vertical = (sbyte)Mathf.Round(raw_vertical); // sbyte is int8

        return (horizontal, vertical);
    }

    private (sbyte, sbyte) ToHome() {
        pathfinding.grid.gridSizeX = (int) (followDistance * 2 + 1);
        pathfinding.grid.gridSizeY = (int) (followDistance * 2 + 1);
        List<Node2D> path = pathfinding.FindPath(transform.position, StartPosition);

        pathfinding.grid.gridSizeX = (int) (detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeY = (int) (detectionDistance * 2 + 1);

        if (path == null) {
            return (0, 0);
        }

        // get the relative position of the next node
        Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

        nextNode.y -= followDistance;
        nextNode.x -= followDistance;

        float raw_horizontal = Clamp(nextNode.x, -1.0f, 1f);
        float raw_vertical = Clamp(nextNode.y, -1.0f, 1f);

        if (raw_horizontal != 0 && raw_vertical != 0) {
            if (directionChange) {
                directionChange = false;
                raw_horizontal = 0f;
            } else {
                directionChange = true;
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
                return Physics2D.Raycast(transform.position + new Vector3(0, 0.51f), transform.up, 0.4f, collideLayers).collider;
            case Direction.Down:
                return Physics2D.Raycast(transform.position + new Vector3(0, -0.51f), -transform.up, 0.4f, collideLayers).collider;
            case Direction.Left:
                return Physics2D.Raycast(transform.position + new Vector3(-0.51f, 0), -transform.right, 0.4f, collideLayers).collider;
            case Direction.Right:
                return Physics2D.Raycast(transform.position + new Vector3(0.51f, 0), transform.right, 0.4f, collideLayers).collider;
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
                        animator.Play($"{animation_prefix}_animation_back_idle");
                        break;
                    case 2:
                        animator.Play($"{animation_prefix}_animation_back_hurt");
                        break;
                    case 3:
                        transform.Translate(0, 0.5f, 0);
                        animator.Play($"{animation_prefix}_animation_back_attack");
                        break;
                }
                break;
            case Direction.Down:
                switch (action)
                {
                    case 1:
                        animator.Play($"{animation_prefix}_animation_front_idle");
                        break;
                    case 2:
                        animator.Play($"{animation_prefix}_animation_front_hurt");
                        break;
                    case 3:
                        transform.Translate(0, -0.5f, 0);
                        animator.Play($"{animation_prefix}_animation_front_attack");
                        break;
                }
                break;
            case Direction.Left:
                switch (action)
                {
                    case 1:
                        animator.Play($"{animation_prefix}_animation_left_idle");
                        break;
                    case 2:
                        animator.Play($"{animation_prefix}_animation_left_hurt");
                        break;
                    case 3:
                        animator.Play($"{animation_prefix}_animation_left_attack");
                        break;
                }
                break;
            case Direction.Right:
                switch (action)
                {
                    case 1:
                        animator.Play($"{animation_prefix}_animation_right_idle");
                        break;
                    case 2:
                        animator.Play($"{animation_prefix}_animation_right_hurt");
                        break;
                    case 3:
                        animator.Play($"{animation_prefix}_animation_right_attack");
                        break;
                }
                break;
        }
    }

    private void OnDrawGizmosSelected() {
        pathfinding.grid.gridSizeX = (int) (detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeY = (int) (detectionDistance * 2 + 1);
        pathfinding.grid.CreateGrid();
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, detectionDistance);
        Handles.color = Color.red;
        // draw a square around the player
        Handles.DrawWireDisc(transform.position, transform.forward, followDistance);
    }

    public void DamageEnemy(uint damage, string targetTag) {
        if (damage >= health) {
            breakableLogic?.TakeHit(999);
            health = 0;
            Destroy(gameObject);
            return;
        }
        PlayAnimation(direction, 2);
        StartCoroutine(ExecuteAfterTime(0.25f, direction, 1));
        sfxPlayer.PlayHurtSound();
        health -= damage;
    }

    // this is called by the animation
    public void AttackTiming(Direction direction) {
        attack.Attack(direction);
    }

    public void AttackEnd(Direction direction) {
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        Controller.main.enabled = true;
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
        Invoke(nameof(callNextEnemy), 0f);
    }

    // intent 1 is hurt
    IEnumerator ExecuteAfterTime(float time, Direction direction, uint intent)
    {
        yield return new WaitForSeconds(time);

        switch (intent)
        {
            case 1:
                PlayAnimation(direction, 1);
                break;
            default:
                break;
        }
    }
}
