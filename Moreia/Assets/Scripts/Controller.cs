using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {
    public static Controller main;

    private enum Direction {
        Up,
        Down,
        Left,
        Right
    }


    private float lastMovement = 0f;
    private Direction current_player_direction = Direction.Down;

    private float original_anchor_position;
    private Inventory inventory;
    public EnemyMovement[] enemies;
    private int current_enemy = 0;
    public bool done_with_tick = true;
    public uint health;
    public uint attackDamage;

    public System.Random rnd = new System.Random();

    [Header("Misc")]
    [SerializeField] LayerMask collideLayers;
    [SerializeField] float movementDelay = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform healthBar;

    // stats
    [Header("Base Stats")]
    [Tooltip("Constitution (maximum health)")]
    [SerializeField] public uint constitution = 10;
    [Tooltip("Dexterity (dodge chance)")]
    [SerializeField] public uint dexterity = 10;
    [Tooltip("Strength (attack damage)")]
    [SerializeField] public uint strength = 10;
    [Tooltip("Wisdom (ability damage)")]
    [SerializeField] public uint wisdom = 10;

    void Awake() {
        main = this;
        original_anchor_position = healthBar.anchoredPosition.x - healthBar.sizeDelta.x / 2;
        inventory = FindObjectOfType<Inventory>();

        // stat randomization
        constitution += Convert.ToUInt32(rnd.Next(0, 5));
        dexterity += Convert.ToUInt32(rnd.Next(0, 5));
        strength += Convert.ToUInt32(rnd.Next(0, 5));
        wisdom += Convert.ToUInt32(rnd.Next(0, 5));

        health = constitution * 2; // current health
        attackDamage = 2 + ((strength - 10) / 2); // attack damage 
    }

    void Update() {
        enemies = FindObjectsOfType<EnemyMovement>();
        if (!done_with_tick) {
            return;
        }
        if (!done_with_tick) {
            if (current_enemy >= enemies.Length) {
                done_with_tick = true;
            } else {
                return;
            }
        }
        Move();
    }

    void Move()
    {
        sbyte horizontal, vertical;

        (horizontal, vertical) = GetAxis();

        if ((horizontal != 0 && vertical != 0) || (horizontal == 0 && vertical == 0))
            return;

        done_with_tick = false;

        // get the direction we are moving
        Direction direction =
            horizontal == 0 ?
            (vertical == 1 ? Direction.Up : Direction.Down) :
            (horizontal == 1 ? Direction.Right : Direction.Left);

        (bool validMove, Collider2D hit) = IsValidMove(direction);
        current_enemy = 0;
        PlayAnimation(direction, 1);
        
        if (Time.time - lastMovement > movementDelay) {
            if (validMove) {
                transform.Translate(horizontal, vertical, 0);
                lastMovement = Time.time;
                NextEnemy();
            } else {
                // if we hit an enemy, attack it
                if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                    Attack(hit, direction); // calls next enemy
                // if we hit a door, attempt to open it
                } else if (hit.gameObject.layer == LayerMask.NameToLayer("door")) {
                    // if the door needs a key, check if we have it
                    bool needsKey = hit.gameObject.GetComponent<Door>().NeedsKey;
                    bool hasKey = inventory.CheckIfItemExists(1);
                    if ((needsKey && hasKey) || !needsKey) {
                        hit.gameObject.GetComponent<Door>().DoorDestroy();
                    } else {
                        Debug.Log("need key");
                    }
                    NextEnemy();
                // if we hit a portal, travel through it
                } else if (hit.gameObject.layer == LayerMask.NameToLayer("portal")) {
                    hit.gameObject.GetComponent<Portal>().PortalTravel();
                } else {
                    NextEnemy();
                }
            }
        }
    }

    public void NextEnemy() {
        if (current_enemy >= enemies.Length) {
            done_with_tick = true;
            return;
        }
        current_enemy++;
        enemies[current_enemy - 1].MakeDecision();
    }

    private void Attack(Collider2D hit, Direction direction)
    {
        hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(attackDamage, hit.gameObject.tag);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        PlayAnimation(direction, 3);
        StartCoroutine(ExecuteAfterTime(1f, direction, 2));
    }

    sbyte BoolToSbyte(bool value) {
        return (sbyte) (value ? 1 : 0);
    }

    (sbyte, sbyte) GetAxis() {
        // todo: allow people to rebind movement keys
        sbyte up = BoolToSbyte(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow));
        sbyte down = BoolToSbyte(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow));
        sbyte left = BoolToSbyte(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow));
        sbyte right = BoolToSbyte(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow));

        sbyte horizontal = (sbyte) (right - left); // sbyte is int8
        sbyte vertical = (sbyte) (up - down); // sbyte is int8

        return (horizontal, vertical);
    }

    // (if it hit something, what it hit)
    (bool, Collider2D) IsValidMove(Direction direction) {
        current_player_direction = direction;
        Collider2D hit = SendRaycast(direction);
        return (hit == null, hit);
    }

    // 1 is idle, 2 is hurt, 3 is attack
    private void PlayAnimation(Direction direction, uint action) {
        switch (direction) {
            case Direction.Up:
                switch (action) {
                    case 1:
                        animator.Play("Player_animation_back_level_0_idle");
                        break;
                    case 2:
                        animator.Play("Player_animation_back_level_0_hurt");
                        break;
                    case 3:
                        transform.Translate(0, 0.5f, 0);
                        animator.Play("Player_animation_back_level_0_attack");
                        break;
                }
                break;  
            case Direction.Down:
                switch (action) {
                    case 1:
                        animator.Play("Player_animation_front_level_0_idle");
                        break;
                    case 2:
                        animator.Play("Player_animation_front_level_0_hurt");
                        break;
                    case 3:
                        transform.Translate(0, -0.5f, 0);
                        animator.Play("Player_animation_front_level_0_attack");
                        break;
                }
                break;
            case Direction.Left:
                switch (action) {
                    case 1:
                        animator.Play("Player_animation_left_level_0_idle");
                        break;
                    case 2:
                        animator.Play("Player_animation_left_level_0_hurt");
                        break;
                    case 3:
                        transform.Translate(-0.5f, 0, 0);
                        animator.Play("Player_animation_left_level_0_attack");
                        break;
                }
                break;
            case Direction.Right:
                switch (action) {
                    case 1:
                        animator.Play("Player_animation_right_level_0_idle");
                        break;
                    case 2:
                        animator.Play("Player_animation_right_level_0_hurt");
                        break;
                    case 3:
                        transform.Translate(0.5f, 0, 0);
                        animator.Play("Player_animation_right_level_0_attack");
                        break;
                }
                break;
        }
    }

    private Collider2D SendRaycast(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Physics2D.Raycast(transform.position, transform.up, 1f, collideLayers).collider;
            case Direction.Down:
                return Physics2D.Raycast(transform.position, -transform.up, 1f, collideLayers).collider;
            case Direction.Left:
                return Physics2D.Raycast(transform.position, -transform.right, 1f, collideLayers).collider;
            case Direction.Right:
                return Physics2D.Raycast(transform.position, transform.right, 1f, collideLayers).collider;
        }
        return Physics2D.Raycast(transform.position, transform.up, 1f, collideLayers).collider;

    }

    private void ChangeHealthBar() {
        float new_bar_width = (health / (float) (constitution * 2)) * 194;
        healthBar.sizeDelta = new Vector2(new_bar_width, healthBar.sizeDelta.y);
        healthBar.anchoredPosition = new Vector2(healthBar.sizeDelta.x / 2 + original_anchor_position, healthBar.anchoredPosition.y);
    }

    public void DamagePlayer(uint damage, bool dodgeable = true) {
        if (damage >= health) {
            health = 0;
            ChangeHealthBar();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
            // todo: death animation
        }

        if ((Convert.ToUInt32(rnd.Next(0, 100)) > 7.5 * ((Math.Max(dexterity - 1, 0) / 2)) && dodgeable) || !dodgeable) {
            health -= damage;
            PlayAnimation(current_player_direction, 2);
            StartCoroutine(ExecuteAfterTime(0.25f, current_player_direction, 1));
        } else {
            Debug.Log("dodged");
            // todo: dodge animation
        }

        ChangeHealthBar();
    }

    // additionalRoutine 1 is nothing, 2 is attack (moves back after animation plays)
    IEnumerator ExecuteAfterTime(float time, Direction direction, uint additionalRoutine)
    {
        yield return new WaitForSeconds(time);

        PlayAnimation(direction, 1);

        switch (additionalRoutine)
        {
            case 1:
                break;
            case 2:
                animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
                switch (direction)
                {
                    case Direction.Up:
                        transform.Translate(0, -0.5f, 0);
                        break;
                    case Direction.Down:
                        transform.Translate(0, 0.5f, 0);
                        break;
                    case Direction.Left:
                        transform.Translate(0.5f, 0, 0);
                        break;
                    case Direction.Right:
                        transform.Translate(-0.5f, 0, 0);
                        break;
                }
                NextEnemy();
                break;
            default:
                break;
        }
    }

}
