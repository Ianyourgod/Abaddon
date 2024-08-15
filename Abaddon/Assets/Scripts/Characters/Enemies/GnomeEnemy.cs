using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;


public class GnomeEnemy : Enemy {
    public override void Move() {
        // Implement the Move method here.
    }

    public override void Attack(uint damage) {
        // Implement the Attack method here.
    }

    public override void Attack() {
        // Implement the Attack method here.
    }

    public override void Interact() {
        // Implement the Interact method here.
    }

    private void Awake() {
        sfxPlayer = GetComponent<EnemySfx>(); 
    }

    private void Start() {
        Controller.main.enemies.Add(this);
        StartPosition = transform.position;
        int gridSize = (int)(detectionDistance * 2 + 1);
        pathfinding.grid.gridSizeX = gridSize;
        pathfinding.grid.gridSizeY = gridSize;
        direction = Vector2.right;
    }

    bool PlayerIsInDetectionRange() {
        return Vector2.Distance(Controller.main.transform.position, transform.position) <= detectionDistance;
    }

    bool PlayerIsInFollowRange() {
        return Vector2.Distance(Controller.main.transform.position, StartPosition) <= followDistance;
    }

    public override void MakeDecision() {
        print("making decision");
        if (PlayerIsInDetectionRange()) {
            if (PlayerIsInFollowRange()) {
                Step(TowardsPlayer());
            }
            //else stay where you are
        } 
        else  {
            Step(TowardsHome());
        }
        
        CallNextEnemy();
    }

    private void CallNextEnemy() {
        Controller.main.NextEnemy();
    }
    
    private void Step(Vector2 direction) {
        if (direction == Vector2.zero) return;

        PlayAnimation("idle", direction);

        Collider2D[] hits = GetAllTilesInFront(direction);

        if (hits.Length == 0) {
            sfxPlayer.PlayWalkSound();
            transform.Translate(direction);
        }
        else if (currentAttack.WouldHit(hits[0], direction)) {
            Controller.main.enabled = false;
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
            PlayAnimation("attack", direction);
        } 
    }

    private Vector2 TowardsPlayer() {
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
    private Vector2 TowardsHome() {
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

    string DirectionToString(Vector2 direction) {
        direction = direction.normalized;

        print($"direction: {direction}");
        if (direction == Vector2.up) {
            return "back";
        } else if (direction == Vector2.down) {
            return "front";
        } else if (direction == Vector2.left) {
            return "left";
        } else if (direction == Vector2.right) {
            return "right";
        }

        throw new Exception("Invalid direction");
    }

    private void PlayAnimation(string action, Vector2? direction = null)
    {
        if (direction == null) direction = this.direction;
        if (action == "death") {
            animator.Play($"{animation_prefix}_animation_death");
            return;
        }

        string animation = $"{animation_prefix}_animation_{DirectionToString((Vector2)direction)}_{action}";

        animator.Play(animation);
        print($"done playing {action} animation");
    }

    public override void Hurt(uint damage, bool dodgeable = false) {
        if (damage >= health) {
            StartDie();
            return; 
        }
        PlayAnimation("hurt", direction);
        
        sfxPlayer.PlayHurtSound();
        health -= damage;
        Helpers.singleton.SpawnHurtText(damage.ToString(), transform.position);
        print("getting hurt");
    }

    public void StartDie() {
        print("dying");

        sfxPlayer.audSource = AudioManager.main.deathSfxPlayer; //the object is destroyed so it has to play the sound through a non-destroyed audio source
        sfxPlayer.PlayDeathSound();
        itemDropper.Drop();
        PlayAnimation("death", direction);
    }

    public override void Die() {
        CallNextEnemy();
        Controller.main.enemies.Remove(this);
        // AttackEnd();
        Destroy(gameObject);
    }
}