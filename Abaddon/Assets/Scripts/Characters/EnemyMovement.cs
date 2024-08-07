using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
//using UnityEditor;7

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Pathfinding2D))]
[RequireComponent(typeof(EnemySfx))]
[RequireComponent(typeof(ItemDropper))]

public class EnemyMovement : MonoBehaviour, Damageable
{

    [Header("References")]
    [SerializeField] LayerMask collideLayers;
    [SerializeField] Animator animator;
    [SerializeField] Pathfinding2D pathfinding;
    [SerializeField] string animation_prefix = "Goblin";
    [SerializeField] BaseAttack attack;
    [SerializeField] SfxPlayer walkingSfxPlayer;
    [SerializeField] SfxPlayer hurtSfxPlayer;

    [Header("Attributes")]
    [SerializeField] float detectionDistance = 1;
    [SerializeField] float followDistance = 3f;
    [SerializeField] float enemyDecisionDelay;


    public GnomeAnimationEventHandler gnomeAnimationEventHandler;


    public uint health = 10;
    private Vector2 direction = Vector2.zero;

    private Vector3 StartPosition;
    
    private EnemySfx sfxPlayer;

    [SerializeField] GameObject textFadePrefab;


    private void Awake(){
        sfxPlayer = GetComponent<EnemySfx>(); 
    }

    private void Start() {
        Controller.main.enemies.Add(this);
        StartPosition = transform.position;
        int gridSize = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeX = gridSize;
        pathfinding.grid.gridSizeY = gridSize;
    }

    bool PlayerIsInDetectionRange() {
        return Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
    }

    bool PlayerIsInFollowRange() {
        return Vector2.Distance(Controller.main.transform.position, StartPosition) <= followDistance;
    }

    public void MakeDecision() {
        if (PlayerIsInDetectionRange()) {
            if (PlayerIsInFollowRange()) {
                MoveToPlayer();
            } 
        } 
        else  {
            MoveToHome();
        }
        
        CallNextEnemy();
    }

    private void CallNextEnemy() {
        Controller.main.NextEnemy();
    }

    void MoveToPlayer() {
        direction = ToPlayer();
        if (direction == Vector2.zero) return;

        print("direction: " + direction);
        Move(direction);
    }

    private void MoveToHome() {
        direction = ToHome();
        if (direction == Vector2.zero) return;

        Move(direction);
    }
    
    private void Move(Vector2 direction) {
        PlayAnimation(direction, "idle");

        Collider2D hit = IsValidMove(direction);
        if (hit != null && hit.gameObject  != null) {
            print("object name: " + hit.gameObject.name);
        }

        if (attack.WillAttack(hit, direction)) {
            print("trying to attack");
            Controller.main.enabled = false;
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
            PlayAnimation(direction, "attack");
        } else if (hit == null) {
            sfxPlayer.PlayWalkSound();
            transform.Translate(direction);
        }
    }

    private Vector2 ToPlayer() {
        List<Node2D> path = pathfinding.FindPath(transform.position, Controller.main.transform.position);

        if (path == null || path.Count == 0) {
            return Vector2.zero;
        }

        // get the relative position of the next node
        Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

        nextNode.y -= detectionDistance;
        nextNode.x -= detectionDistance;

        float raw_horizontal = Mathf.Clamp(nextNode.x, -1.0f, 1f);
        float raw_vertical = Mathf.Clamp(nextNode.y, -1.0f, 1f);

        float horizontal = Mathf.Round(raw_horizontal); // sbyte is int8
        float vertical = Mathf.Round(raw_vertical); // sbyte is int8

        return new Vector2(horizontal, vertical);
    }
    private Vector2 ToHome() {
        pathfinding.grid.gridSizeX = (int) (followDistance * 2 + 1);
        pathfinding.grid.gridSizeY = (int) (followDistance * 2 + 1);
        List<Node2D> path = pathfinding.FindPath(transform.position, StartPosition);

        pathfinding.grid.gridSizeX = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeY = (int)(detectionDistance * 2 + 1);

        if (path == null || path.Count == 0) {
            return Vector2.zero;
        }

        // get the relative position of the next node
        Vector2 nextNode = path[0].worldPosition - pathfinding.grid.worldBottomLeft;

        nextNode.y -= followDistance;
        nextNode.x -= followDistance;

        float raw_horizontal = Mathf.Clamp(nextNode.x, -1.0f, 1f);
        float raw_vertical = Mathf.Clamp(nextNode.y, -1.0f, 1f);

        float horizontal = Mathf.Round(raw_horizontal);
        float vertical = Mathf.Round(raw_vertical);

        return new Vector2(horizontal, vertical);
    }

    private Collider2D IsValidMove(Vector2 direction) {
        return Physics2D.OverlapCircle(transform.position+new Vector3(direction.x, direction.y, 0), 0.1f, collideLayers);
    }

    string DirectionToString(Vector2 direction) {
        /*
        back: (0, 1)
        front: (0, -1)
        left: (-1, 0)
        right: (1, 0)
        */
        
        if (direction == Vector2.up) {
            return "back";
        } else if (direction == Vector2.down) {
            return "front";
        } else if (direction == Vector2.left) {
            return "left";
        } else if (direction == Vector2.right) {
            return "right";
        }
        return "front";
    }

    // 1 is idle, 2 is hurt, 3 is attack
    private void PlayAnimation(Vector2 direction, string action)
    {
        if (action == "death") {
            animator.Play($"{animation_prefix}_animation_death");
            return;
        }

        string animation = $"{animation_prefix}_animation_{DirectionToString(direction)}_{action}";

        animator.Play(animation);
        print("done!");
    }

    /*
    sadly disabled because it causes errors when building
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
    */

    public void Hurt(uint damage, bool dodgeable = false) {
        if (damage >= health) {
            health = 0;
            sfxPlayer.audSource = AudioManager.main.deathSfxPlayer; //the object is destroyed so it has to play the sound through a non-destroyed audio source
            sfxPlayer.PlayDeathSound();
            GetComponent<ItemDropper>().Die();
            // run death animation
            PlayAnimation(direction, "death");
            return;
        }
        PlayAnimation(direction, "hurt");
        // TODO: GET RID OF THE COROUTINE!!!!!!!!!!!!
        
        sfxPlayer.PlayHurtSound();
        health -= damage;
        // GameObject damageAmount = Instantiate(textFadePrefab, transform.position + new Vector3(Random.Range(1, 5) / 10, Random.Range(1, 5) / 10, 0), Quaternion.identity);
        // damageAmount.GetComponent<RealTextFadeUp>().SetText(damage.ToString(), Color.red, Color.white, 0.4f);
        Helpers.singleton.SpawnHurtText(damage.ToString(), transform.position);
    }

    public void Die()
    {
        CallNextEnemy();
        Controller.main.enemies.Remove(this);
        Destroy(gameObject);
    }

    // this is called by the animation
    public void AttackTiming() {
        attack.Attack();
    }

    public void AttackEnd(Vector2 direction) {
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        Controller.main.enabled = true;
        PlayAnimation(direction, "idle");
    }
}
