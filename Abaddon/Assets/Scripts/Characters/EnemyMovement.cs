using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.U2D.Path.GUIFramework;
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
    [SerializeField] Attack attack;
    [SerializeField] SfxPlayer walkingSfxPlayer;
    [SerializeField] SfxPlayer hurtSfxPlayer;

    [Header("Attributes")]
    [SerializeField] float detectionDistance = 1;
    [SerializeField] float followDistance = 3f;
    [SerializeField] float enemyDecisionDelay;
    [SerializeField] AnimationEventHandler animationEventHandler;

    public uint health = 10;
    private Vector2 facingDirection = Vector2.down;

    private Vector3 StartPosition;
    
    private EnemySfx sfxPlayer;


    private void Awake(){
        sfxPlayer = GetComponent<EnemySfx>();
    }

    private void Start() {
        // Controller.main.enemies.Add(this);
        StartPosition = transform.position;
        int gridSize = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeX = gridSize;
        pathfinding.grid.gridSizeY = gridSize;
        ((GnomeAnimationHandlerData)animationEventHandler.data).onAttackEnd += AttackEnd;
        ((GnomeAnimationHandlerData)animationEventHandler.data).onHurtEnd += HurtEnd;
        // animationEventHandler.QueueAnimation(
        //     new AnimationEventHandler.QueuedAnimation(
        //         animation: "idle", 
        //         priority: 0, 
        //         shouldInterupt: false, 
        //         shouldLoop: true
        //     ), 
        //     facingDirection
        // );
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
        
        if (gameObject.name == "GnomeEnemy") {
            print("call next enemy");
        }
        CallNextEnemy();
    }

    private void CallNextEnemy() {
        Controller.main.NextEnemy();
    }

    void MoveToPlayer() {
        var movingDirection = ToPlayer();
        if (movingDirection == Vector2.zero) return;

        Move(movingDirection);
    }

    private void MoveToHome() {
        var movingDirection = ToHome();
        if (movingDirection == Vector2.zero) return;

        Move(movingDirection);
    }
    
    private void Move(Vector2 direction) {
        facingDirection = direction;
        // animationEventHandler.QueueAnimation(new AnimationEventHandler.QueuedAnimation(
        //         animation: "idle", 
        //         priority: 0, 
        //         shouldInterupt: false, 
        //         shouldLoop: true
        //     ), direction);

        Collider2D[] hits = GetAllTilesInFront(direction);

        if (hits.Length == 0) {
            sfxPlayer.PlayWalkSound();
            transform.Translate(direction);
        }
        else if (attack.WouldHit(hits[0], direction)) {
            Controller.main.enabled = false;
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
            animationEventHandler.QueueAnimation(new AnimationEventHandler.Animation(
                animation: "attack", 
                priority: 1, 
                shouldInterupt: true, 
                shouldLoop: false
            ), direction);
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

    Collider2D[] GetAllTilesInFront(Vector2 direction) {
        Vector2 centerOfBox = (Vector2)transform.position + direction;
        return Physics2D.OverlapBoxAll(centerOfBox, new Vector3(0.9f, 0.9f, 0), 0, collideLayers);
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
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
            GetComponent<ItemDropper>().Drop();
            // run death animation
            animationEventHandler.QueueAnimation(new AnimationEventHandler.Animation(
                animation: "death", 
                priority: 2, 
                shouldInterupt: true, 
                shouldLoop: false
            ), facingDirection);
            print("dying");
            return;
        }
        animationEventHandler.QueueAnimation(new AnimationEventHandler.Animation(
            animation: "hurt", 
            priority: 1, 
            shouldInterupt: true, 
            shouldLoop: false
        ), facingDirection);
        
        sfxPlayer.PlayHurtSound();
        health -= damage;
        Helpers.singleton.SpawnHurtText(damage.ToString(), transform.position);
        print("getting hurt");
    }

    public void Die()
    {
        CallNextEnemy();
        AttackEnd();
        Destroy(gameObject);
    }

    // this is called by the animation
    public void AttackTiming() {
        attack.Hit();
    }

    public void AttackEnd() {
        print("ENDING ATTACK");
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        Controller.main.enabled = true;
        // animationEventHandler.QueueAnimation(new AnimationEventHandler.QueuedAnimation(
        //     animation: "idle", 
        //     priority: 0, 
        //     shouldInterupt: false, 
        //     shouldLoop: true
        // ), facingDirection);
    }

    public void HurtEnd() {
        print("ENDING Hurt Animation");
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        Controller.main.enabled = true;
        // animationEventHandler.QueueAnimation(new AnimationEventHandler.QueuedAnimation(
        //     animation: "idle", 
        //     priority: 0, 
        //     shouldInterupt: false, 
        //     shouldLoop: true
        // ), facingDirection);
    }
}
