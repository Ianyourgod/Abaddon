using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Pathfinding2D))]
[RequireComponent(typeof(EnemySfx))]
[RequireComponent(typeof(ItemDropper))]
public class EnemyMovement : MonoBehaviour, CanFight
{
    [Header("References")]
    [SerializeField]
    LayerMask collideLayers;

    [SerializeField]
    Animator animator;

    [SerializeField]
    Pathfinding2D pathfinding;

    [SerializeField]
    string animation_prefix = "Goblin";

    [SerializeField]
    BaseAttack attack;

    [SerializeField]
    SfxPlayer walkingSfxPlayer;

    [SerializeField]
    SfxPlayer hurtSfxPlayer;

    [Header("Attributes")]
    [SerializeField]
    int detectionDistance = 4;

    [SerializeField]
    float followDistance = 3f;

    [SerializeField]
    float enemyDecisionDelay;

    [SerializeField]
    EnemyType enemyType;

    public int health = 10;
    private Vector2 direction = Vector2.zero;

    private Vector2 movementTarget;
    private bool followingPlayer = false;

    private Vector3 StartPosition;

    private EnemySfx sfxPlayer;

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    private void Awake()
    {
        sfxPlayer = GetComponent<EnemySfx>();
        StartPosition = transform.position;
        int gridSize = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeX = gridSize;
        pathfinding.grid.gridSizeY = gridSize;
    }

    private void Start()
    {
        StartPosition = transform.position;
        int gridSize = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeX = gridSize;
        pathfinding.grid.gridSizeY = gridSize;
    }

    bool CheckPlayerIsInDetectionRange()
    {
        if (Controller.main == null)
            return false;

        return UnityEngine.Vector2.Distance(Controller.main.transform.position, transform.position)
            <= detectionDistance;
    }

    bool CheckPlayerIsInFollowRange()
    {
        if (Controller.main == null)
            return false;

        float distance = UnityEngine.Vector2.Distance(
            Controller.main.transform.position,
            StartPosition
        );
        return distance <= followDistance;
    }

    public void MakeDecision()
    {
        bool inFollowRange = CheckPlayerIsInFollowRange();
        bool inDetectionRange = CheckPlayerIsInDetectionRange();
        bool atHome = transform.position == StartPosition;

        if (inDetectionRange && inFollowRange)
        {
            Invoke(nameof(MoveToPlayer), enemyDecisionDelay);
        }
        else if (!inFollowRange && !atHome)
        {
            Vector2 direction = ToHome();

            if (direction == Vector2.zero)
            {
                Invoke(nameof(callNextEnemy), 0f);
                return;
            }

            Move(direction);
        }
        else
        {
            callNextEnemy();
        }
    }

    private void callNextEnemy()
    {
        if (Controller.main == null)
            return;

        Controller.main.NextEnemy();
    }

    void MoveToPlayer()
    {
        if (CheckPlayerIsInDetectionRange())
        {
            if (Controller.main == null)
                return;

            followingPlayer = true;
            movementTarget = Controller.main.transform.position;
        }
        else if (followingPlayer && !CheckPlayerIsInFollowRange())
        {
            followingPlayer = false;
            Invoke(nameof(callNextEnemy), 0f);
            return;
        }

        direction = ToPlayer();

        if (direction == Vector2.zero)
        {
            Invoke(nameof(callNextEnemy), 0f);
            return;
        }

        // get the direction we are moving

        Move(direction);
    }

    private void Move(Vector2 direction)
    {
        RaycastHit2D[] hits = IsValidMove(direction);
        PlayAnimation(direction, "idle");

        bool will_attack = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (attack.WillAttack(hit, direction))
            {
                will_attack = true;
                break;
            }
        }
        bool can_move = true;
        bool hitting_only_player = will_attack;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.name != this.name)
            {
                can_move = false;
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    hitting_only_player = false;
                    break;
                }
            }
        }

        if (hitting_only_player)
        {
            Attack();
        }
        else if (can_move)
        {
            sfxPlayer.PlayWalkSound();
            transform.Translate(direction);
            // Debug.Log($"chika chika my name is {this.name} Moving in direction: {direction}");
            Invoke(nameof(callNextEnemy), 0f);
        }
        else
        {
            Invoke(nameof(callNextEnemy), 0f);
        }
    }

    public void Attack()
    {
        if (Controller.main == null)
            return;

        // GetComponent<CanFight>().Attack();
        Controller.main.enabled = false;
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        PlayAnimation(direction, "attack");
    }

    private static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min
            : (value > max) ? max
            : value;
    }

    private Vector2 ToPlayer()
    {
        if (Controller.main == null)
            return Vector2.zero;

        List<Node2D> path = pathfinding.FindPath(
            transform.position,
            Controller.main.transform.position
        );

        if (path == null)
        {
            return Vector2.zero;
        }

        // shouldnt happen but just in case
        if (path.Count == 0)
        {
            return Vector2.zero;
        }

        // get the relative position of the next node
        Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

        nextNode.y -= detectionDistance;
        nextNode.x -= detectionDistance;

        float raw_horizontal = Clamp(nextNode.x, -1.0f, 1f);
        float raw_vertical = Clamp(nextNode.y, -1.0f, 1f);

        float horizontal = Mathf.Round(raw_horizontal); // sbyte is int8
        float vertical = Mathf.Round(raw_vertical); // sbyte is int8

        return new Vector2(horizontal, vertical);
    }

    private Vector2 ToHome()
    {
        pathfinding.grid.gridSizeX = (int)(followDistance * 2 + 1);
        pathfinding.grid.gridSizeY = (int)(followDistance * 2 + 1);
        List<Node2D> path = pathfinding.FindPath(transform.position, StartPosition);

        pathfinding.grid.gridSizeX = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeY = (int)(detectionDistance * 2 + 1);

        if (path == null)
        {
            return Vector2.zero;
        }

        // shouldnt happen but just in case
        if (path.Count == 0)
        {
            return Vector2.zero;
        }

        // get the relative position of the next node
        Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

        nextNode.y -= followDistance;
        nextNode.x -= followDistance;

        float raw_horizontal = Clamp(nextNode.x, -1.0f, 1f);
        float raw_vertical = Clamp(nextNode.y, -1.0f, 1f);

        float horizontal = Mathf.Round(raw_horizontal);
        float vertical = Mathf.Round(raw_vertical);

        return new Vector2(horizontal, vertical);
    }

    private RaycastHit2D[] IsValidMove(Vector2 direction)
    {
        // return Physics2D.OverlapCircle(
        //     transform.position + new Vector3(direction.x, direction.y, 0),
        //     0.1f,
        //     collideLayers
        // );
        var hits = Physics2D.RaycastAll(transform.position, direction, 1f, collideLayers);

        if (hits.Length > 1)
        {
            Debug.DrawRay(transform.position, direction, Color.magenta, 0.2f, false);
        }
        return hits;
    }

    string DirectionToString(Vector2 direction)
    {
        direction = direction.normalized;

        if (direction == Vector2.up)
            return "back";
        else if (direction == Vector2.down)
            return "front";
        else if (direction == Vector2.left)
            return "left";
        else if (direction == Vector2.right)
            return "right";
        else
            throw new System.Exception("Invalid direction to be converted to string: " + direction);
    }

    private void PlayAnimation(Vector2 direction, string action)
    {
        if (action == "death")
        {
            //print($"playing animation {animation_prefix}_animation_death");
            animator.Play($"{animation_prefix}_animation_death");
            return;
        }

        if (direction == Vector2.zero)
        {
            direction = Vector2.down;
        }
        string animation = $"{animation_prefix}_animation_{DirectionToString(direction)}_{action}";
        animator.Play(animation);
    }

    void OnDrawGizmosSelected()
    {
        int gridSize = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeX = gridSize;
        pathfinding.grid.gridSizeY = gridSize;
        pathfinding.grid.DrawGizmos();
    }

    public int Hurt(int damage)
    {
        if (damage >= health)
        {
            Controller.main.KilledEnemy(enemyType);
            Controller.OnTick -= MakeDecision;
            health = 0;
            sfxPlayer.audSource = AudioManager.main.deathSfxPlayer; //the object is destroyed so it has to play the sound through a non-destroyed audio source
            sfxPlayer.PlayDeathSound();
            Controller.main.add_exp(Random.Range(1, 4));
            GetComponent<ItemDropper>().DropRandomItem();
            PlayAnimation(direction, "death");
        }
        else
        {
            Helpers.singleton.SpawnHurtText(damage.ToString(), transform.position);
            PlayAnimation(direction, "hurt");
            sfxPlayer.PlayHurtSound();
            health -= damage;
        }
        return health; // Return remaining health as int
    }

    public int Heal(int amount)
    {
        health += amount;
        return health;
    }

    public void Die()
    {
        Invoke(nameof(callNextEnemy), 0f);
        Destroy(gameObject);
    }

    // this is called by the animation
    public void AttackTiming(Vector2 direction)
    {
        attack.Attack(direction);
    }

    public void AttackEnd(Vector2 direction)
    {
        if (Controller.main == null)
            return;

        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        Controller.main.enabled = true;
        PlayAnimation(direction, "idle");
        Invoke(nameof(callNextEnemy), 0f);
    }
}
