using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public static Controller main;

    public delegate void TickAction();
    public static event TickAction OnTick;

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
    public string current_player_direction = "Down";

    [SerializeField] LayerMask collideLayers;
    [SerializeField] float movementDelay = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform healthBar;
    private float original_anchor_position;

    void Awake() {
        main = this;
        original_anchor_position = healthBar.anchoredPosition.x - healthBar.sizeDelta.x / 2;
    } 

    void Update() {
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

        Collider2D hit = SendRaycast(direction);

        if (EnemyMovement.Attacking != 1) {
            if (IsValidMove(direction) && Time.time - lastMovement > movementDelay) {
                transform.Translate(horizontal, vertical, 0);
                lastMovement = Time.time;
            } else if (hit != null && Time.time - lastMovement > movementDelay) {
                hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(attackDamage, hit.gameObject.tag);
            }
            OnTick?.Invoke();
        }
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
        switch (direction) {
            case Direction.Up:
                animator.Play("Player_animation_back_level_0_idle");
                current_player_direction = "Up";
                return Physics2D.Raycast(transform.position, transform.up, 1f, collideLayers).collider == null;
            case Direction.Down:
                animator.Play("Player_animation_front_level_0_idle");
                current_player_direction = "Down";
                return Physics2D.Raycast(transform.position, -transform.up, 1f, collideLayers).collider == null;
            case Direction.Left:
                animator.Play("Player_animation_left_level_0_idle");
                current_player_direction = "Left";
                return Physics2D.Raycast(transform.position, -transform.right, 1f, collideLayers).collider == null;
            case Direction.Right:
                animator.Play("Player_animation_right_level_0_idle");
                current_player_direction = "Right";
                return Physics2D.Raycast(transform.position, transform.right, 1f, collideLayers).collider == null;
        }
        return false;
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
            Destroy(gameObject);
            return;
            // todo: death
        }
        health -= damage;

        switch (current_player_direction) {
            case "Up":
                animator.Play("Player_animation_back_level_0_hurt");
                StartCoroutine(ExecuteAfterTime(0.25f));
                break;
            case "Down":
                animator.Play("Player_animation_front_level_0_hurt");
                StartCoroutine(ExecuteAfterTime(0.25f));
                break;
            case "Left":
                animator.Play("Player_animation_left_level_0_hurt");
                StartCoroutine(ExecuteAfterTime(0.25f));
                break;
            case "Right":
                animator.Play("Player_animation_right_level_0_hurt");
                StartCoroutine(ExecuteAfterTime(0.25f));
                break;
        };

        ChangeHealthBar();
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        switch (current_player_direction)
        {
            case "Up":
                animator.Play("Player_animation_back_level_0_idle");
                break;
            case "Down":
                animator.Play("Player_animation_front_level_0_idle");
                break;
            case "Left":
                animator.Play("Player_animation_left_level_0_idle");
                break;
            case "Right":
                animator.Play("Player_animation_right_level_0_idle");
                break;
        };
    }

}