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

    public uint constitution = 10; // aka max health 
    public uint health = 20; // current health
    public uint attackDamage = 3;

    private float lastMovement = 0f;
    public static uint Attacking = 0;
    private Direction current_player_direction = Direction.Down;

    [SerializeField] LayerMask collideLayers;
    [SerializeField] float movementDelay = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform healthBar;
    private float original_anchor_position;
    private Inventory inventory;
    public EnemyMovement[] enemies;
    private int current_enemy = 0;
    public bool done_with_enemies = true;

    void Awake() {
        main = this;
        original_anchor_position = healthBar.anchoredPosition.x - healthBar.sizeDelta.x / 2;
        inventory = FindObjectOfType<Inventory>();
    }

    void Update() {
        enemies = FindObjectsOfType<EnemyMovement>();
        if (!done_with_enemies) {
            if (current_enemy >= enemies.Length) {
                done_with_enemies = true;
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

        // get the direction we are moving
        Direction direction =
            horizontal == 0 ?
            (vertical == 1 ? Direction.Up : Direction.Down) :
            (horizontal == 1 ? Direction.Right : Direction.Left);

        if (Attacking != 1) {
            Collider2D hit = SendRaycast(direction);
            current_enemy = 0;
            done_with_enemies = false;
            PlayAnimation(direction, 1);
            
            if (Time.time - lastMovement > movementDelay) {
                if (hit == null) {
                    transform.Translate(horizontal, vertical, 0);
                    lastMovement = Time.time;
                    NextEnemy();
                } else {
                    // if we hit an enemy, attack it
                    if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                        Attack(hit, direction);
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
                        Debug.Log(hit.gameObject);
                        hit.gameObject.GetComponent<Portal>().PortalTravel();
                    } else {
                        NextEnemy();
                    }
                }
            }
        }
    }

    public void NextEnemy() {
        if (current_enemy >= enemies.Length) {
            done_with_enemies = true;
            return;
        }
        enemies[current_enemy].MakeDecision();
        current_enemy++;
    }

    private void Attack(Collider2D hit, Direction direction)
    {
        hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(attackDamage, hit.gameObject.tag);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        PlayAnimation(direction, 3);
        StartCoroutine(ExecuteAfterTime(1f, direction, 2));
        Attacking = 1;
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

    bool IsValidMove(Direction direction) {
        current_player_direction = direction;
        switch (direction) {
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

    public void DamagePlayer(uint damage) {
        if (damage >= health) {
            health = 0;
            ChangeHealthBar();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
            // todo: death animation
        }
        health -= damage;

        PlayAnimation(current_player_direction, 2);
        StartCoroutine(ExecuteAfterTime(0.25f, current_player_direction, 1));

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
                Attacking = 0;
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
