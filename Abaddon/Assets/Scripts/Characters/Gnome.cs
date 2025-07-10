// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// //using UnityEditor;

// [RequireComponent(typeof(BoxCollider2D))]
// [RequireComponent(typeof(Pathfinding2D))]
// [RequireComponent(typeof(EnemySfx))]
// [RequireComponent(typeof(ItemDropper))]

// public class Gnome : MonoBehaviour, CanFight, HasHealth, CanMove
// {

//     [Header("References")]
//     [SerializeField] LayerMask collideLayers;
//     [SerializeField] Animator animator;
//     [SerializeField] Pathfinding2D pathfinding;
//     [SerializeField] string animation_prefix = "Goblin";
//     [SerializeField] BaseAttack attack;
//     [SerializeField] SfxPlayer walkingSfxPlayer;
//     [SerializeField] SfxPlayer hurtSfxPlayer;

//     [Header("Attributes")]
//     [SerializeField] int detectionDistance = 4;
//     [SerializeField] float followDistance = 3f;
//     [SerializeField] float enemyDecisionDelay;

//     [SerializeField] uint health = 100;
//     [SerializeField] uint maxHealth = 100;

//     private Vector2 direction = Vector2.zero;

//     private Vector2 movementTarget;
//     private bool followingPlayer = false;

//     private Vector3 StartPosition;

//     private EnemySfx sfxPlayer;

//     private void Awake()
//     {
//         sfxPlayer = GetComponent<EnemySfx>();
//         StartPosition = transform.position;
//         int gridSize = (int)(detectionDistance * 2 + 1);
//         pathfinding.grid.gridSizeX = gridSize;
//         pathfinding.grid.gridSizeY = gridSize;
//     }

//     private void Start()
//     {
//         StartPosition = transform.position;
//         int gridSize = (int)(detectionDistance * 2 + 1);
//         pathfinding.grid.gridSizeX = gridSize;
//         pathfinding.grid.gridSizeY = gridSize;
//     }

//     bool CheckPlayerIsInDetectionRange()
//     {
//         return UnityEngine.Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
//     }

//     bool CheckPlayerIsInFollowRange()
//     {
//         float distance = UnityEngine.Vector2.Distance(Controller.main.transform.position, StartPosition);
//         return distance <= followDistance;
//     }

//     public void MakeDecision()
//     {
//         bool inFollowRange = CheckPlayerIsInFollowRange();
//         bool inDetectionRange = CheckPlayerIsInDetectionRange();
//         bool atHome = transform.position == StartPosition;

//         if (inDetectionRange && inFollowRange)
//         {
//             Invoke(nameof(MoveToPlayer), enemyDecisionDelay);
//         }
//         else if (!inFollowRange && !atHome)
//         {
//             Vector2 direction = ToHome();

//             if (direction == Vector2.zero)
//             {
//                 Invoke(nameof(callNextEnemy), 0f);
//                 return;
//             }

//             Move(direction);
//         }
//         else
//         {
//             Invoke(nameof(callNextEnemy), 0f);
//         }
//     }

//     private void callNextEnemy()
//     {
//         Controller.main.NextEnemy();
//     }

//     void MoveToPlayer()
//     {
//         if (CheckPlayerIsInDetectionRange())
//         {
//             followingPlayer = true;
//             movementTarget = Controller.main.transform.position;
//         }
//         else if (followingPlayer && !CheckPlayerIsInFollowRange())
//         {
//             followingPlayer = false;
//             Invoke(nameof(callNextEnemy), 0f);
//             return;
//         }

//         direction = ToPlayer();

//         if (direction == Vector2.zero)
//         {
//             Invoke(nameof(callNextEnemy), 0f);
//             return;
//         }

//         // get the direction we are moving

//         Move(direction);
//     }

//     public void Hurt(uint damage)
//     {
//         throw new System.NotImplementedException();
//     }

//     public void Heal(uint amount)
//     {
//         throw new System.NotImplementedException();
//     }

//     public void Move(Vector2 direction)
//     {
//         Collider2D hit = IsValidMove(direction);
//         PlayAnimation(direction, "idle");

//         bool will_attack = attack.WillAttack(hit, direction);

//         if (will_attack)
//         {
//             Attack(1);
//         }
//         else if (hit == null)
//         {
//             sfxPlayer.PlayWalkSound();
//             transform.Translate(direction);
//             Invoke(nameof(callNextEnemy), 0f);
//         }
//         else
//         {
//             Invoke(nameof(callNextEnemy), 0f);
//         }
//     }

//     public void Attack(uint damage)
//     {
//         GetComponent<CanFight>().Attack(damage);
//         Controller.main.enabled = false;
//         animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
//         PlayAnimation(direction, "attack");
//     }

//     private static float Clamp(float value, float min, float max)
//     {
//         return (value < min) ? min : (value > max) ? max : value;
//     }

//     private Vector2 ToPlayer()
//     {
//         List<Node2D> path = pathfinding.FindPath(transform.position, Controller.main.transform.position);

//         if (path == null)
//         {
//             return Vector2.zero;
//         }

//         // shouldnt happen but just in case
//         if (path.Count == 0)
//         {
//             return Vector2.zero;
//         }

//         // get the relative position of the next node
//         Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

//         nextNode.y -= detectionDistance;
//         nextNode.x -= detectionDistance;

//         float raw_horizontal = Clamp(nextNode.x, -1.0f, 1f);
//         float raw_vertical = Clamp(nextNode.y, -1.0f, 1f);

//         float horizontal = Mathf.Round(raw_horizontal); // sbyte is int8
//         float vertical = Mathf.Round(raw_vertical); // sbyte is int8

//         return new Vector2(horizontal, vertical);
//     }

//     private Vector2 ToHome()
//     {
//         pathfinding.grid.gridSizeX = (int)(followDistance * 2 + 1);
//         pathfinding.grid.gridSizeY = (int)(followDistance * 2 + 1);
//         List<Node2D> path = pathfinding.FindPath(transform.position, StartPosition);

//         pathfinding.grid.gridSizeX = (int)(detectionDistance * 2 + 1);
//         pathfinding.grid.gridSizeY = (int)(detectionDistance * 2 + 1);

//         if (path == null)
//         {
//             return Vector2.zero;
//         }

//         // shouldnt happen but just in case
//         if (path.Count == 0)
//         {
//             return Vector2.zero;
//         }

//         // get the relative position of the next node
//         Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

//         nextNode.y -= followDistance;
//         nextNode.x -= followDistance;

//         float raw_horizontal = Clamp(nextNode.x, -1.0f, 1f);
//         float raw_vertical = Clamp(nextNode.y, -1.0f, 1f);

//         float horizontal = Mathf.Round(raw_horizontal);
//         float vertical = Mathf.Round(raw_vertical);

//         return new Vector2(horizontal, vertical);
//     }

//     private Collider2D IsValidMove(Vector2 direction)
//     {
//         return Physics2D.OverlapCircle(transform.position + new Vector3(direction.x, direction.y, 0), 0.1f, collideLayers);
//     }

//     string DirectionToString(Vector2 direction)
//     {
//         /*
//         back: (0, 1)
//         front: (0, -1)
//         left: (-1, 0)
//         right: (1, 0)
//         */

//         if (direction == Vector2.up)
//         {
//             return "back";
//         }
//         else if (direction == Vector2.down)
//         {
//             return "front";
//         }
//         else if (direction == Vector2.left)
//         {
//             return "left";
//         }
//         else if (direction == Vector2.right)
//         {
//             return "right";
//         }
//         return "front";
//     }

//     // 1 is idle, 2 is hurt, 3 is attack
//     private void PlayAnimation(Vector2 direction, string action)
//     {
//         if (action == "death")
//         {
//             animator.Play($"{animation_prefix}_animation_death");
//             return;
//         }

//         string animation = $"{animation_prefix}_animation_{DirectionToString(direction)}_{action}";

//         animator.Play(animation);
//     }

//     //sadly disabled because it causes errors when building
//     void OnDrawGizmosSelected()
//     {
//         int gridSize = (int)(detectionDistance * 2 + 1);
//         pathfinding.grid.gridSizeX = gridSize;
//         pathfinding.grid.gridSizeY = gridSize;
//         pathfinding.grid.DrawGizmos();
//     }

//     public bool TakeDamage(uint damage)
//     {
//         if (damage >= health)
//         {
//             health = 0;
//             sfxPlayer.audSource = AudioManager.main.deathSfxPlayer; //the object is destroyed so it has to play the sound through a non-destroyed audio source
//             sfxPlayer.PlayDeathSound();
//             // random number between 1 and 3
//             Controller.main.add_exp(Random.Range(1, 4));
//             GetComponent<ItemDropper>().Die();
//             // run death animation
//             PlayAnimation(direction, "death");
//             return true;
//         }
//         PlayAnimation(direction, "hurt");
//         // TODO: GET RID OF THE COROUTINE!!!!!!!!!!!!
//         StartCoroutine(ExecuteAfterTime(0.25f, direction, 1));
//         sfxPlayer.PlayHurtSound();
//         health -= damage;

//         return true;
//     }

//     public void Die()
//     {
//         Invoke(nameof(callNextEnemy), 0f);
//         Destroy(gameObject);
//     }

//     // this is called by the animation
//     public void AttackTiming(Vector2 direction)
//     {
//         attack.Attack(direction);
//     }

//     public void AttackEnd(Vector2 direction)
//     {
//         animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
//         Controller.main.enabled = true;
//         PlayAnimation(direction, "idle");
//         Invoke(nameof(callNextEnemy), 0f);
//     }

//     // intent 1 is hurt
//     IEnumerator ExecuteAfterTime(float time, Vector2 direction, uint intent)
//     {
//         yield return new WaitForSeconds(time);

//         switch (intent)
//         {
//             case 1:
//                 PlayAnimation(direction, "idle");
//                 break;
//             default:
//                 break;
//         }
//     }
// }
