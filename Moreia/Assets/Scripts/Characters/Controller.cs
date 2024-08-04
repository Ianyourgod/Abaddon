using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSfx))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(BoxCollider2D))]

public class Controller : MonoBehaviour {
    // used for non-enemy things where having stuff in order doesnt really matter
    public delegate void TickAction();
    public static event TickAction OnTick;

    public static Controller main;

    private float lastMovement = 0f;
    private Vector2 current_player_direction = new Vector2(0, -1);

    private float original_anchor_position;
    [HideInInspector]
    public Inventory inventory;
    public EnemyMovement[] enemies;
    private int current_enemy = 0;
    public bool done_with_tick = true;
    public int health;
    public int max_health;
    public int attackDamage;

    public System.Random rnd = new System.Random();

    public delegate void OnDie();
    public OnDie onDie;

    [HideInInspector] public PlayerSfx sfxPlayer;

    [SerializeField] GameObject textFadePrefab;

    [Header("Misc")]
    [SerializeField] LayerMask collideLayers;
    [SerializeField] float movementDelay = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform healthBar;
    [SerializeField] Transform respawnPoint;

    // stats
    [Header("Base Stats")]
    [Tooltip("Constitution (maximum health)")]
    [SerializeField] public int constitution = 9;
    [Tooltip("Dexterity (dodge chance)")]
    [SerializeField] public int dexterity = 9;
    [Tooltip("Strength (attack damage)")]
    [SerializeField] public int strength = 9;
    [Tooltip("Wisdom (ability damage)")]
    [SerializeField] public int wisdom = 9;
    [Tooltip("High end of range to add")]
    [SerializeField] public int maximum_stat_roll = 7;

    void Awake() {
        main = this;

        sfxPlayer = GetComponent<PlayerSfx>();

        if (healthBar == null) {
            healthBar = new GameObject().AddComponent<RectTransform>();
        } else {
            original_anchor_position = healthBar.anchoredPosition.x - healthBar.sizeDelta.x / 2;
        }
        inventory = GetComponent<Inventory>();

        // stat randomization
        constitution += rnd.Next(1, maximum_stat_roll);
        dexterity += rnd.Next(1, maximum_stat_roll);
        strength += rnd.Next(1, maximum_stat_roll);
        wisdom += rnd.Next(1, maximum_stat_roll);

        health = constitution * 2; // current health
        max_health = health;
        ChangeHealthBar();
    }

    void Update() {
        UpdateStats();

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
        Vector2 direction = GetAxis();
        // if we are not moving, do nothing. if we are going diagonally, do nothing
        if (direction == Vector2.zero || (direction.x != 0 && direction.y != 0)) {
            return;
        }

        done_with_tick = false;

        (bool validMove, Collider2D hit) = IsValidMove(direction);
        current_enemy = 0;
        PlayAnimation(direction, "idle");
        
        if (Time.time - lastMovement <= movementDelay) {
            return;
        }

        lastMovement = Time.time;

        if (validMove || hit.gameObject.layer == LayerMask.NameToLayer("floorTrap")) {
            transform.Translate(direction);
            sfxPlayer.PlayWalkSound();
            FinishTick();
        // if we hit an enemy, attack it
        } else if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Attack(hit, direction, true); // calls next enemy
        // if we hit a door, attempt to open it
        } else if (hit.gameObject.layer == LayerMask.NameToLayer("interactable")) {
            hit.GetComponent<Interactable>().Interact(attackDamage);
            FinishTick();
        }
        // if we hit a fountain, heal from it
        else if (hit.gameObject.layer == LayerMask.NameToLayer("fountain")) {
            if (health < max_health)
            {
                hit.GetComponent<FountainSfx>().PlayFountainSound();
            }
            hit.gameObject.GetComponent<Fountain>().Heal();
            FinishTick();
        }
        else {
            FinishTick();
        }
    }

    public void UpdateStats()
    {
        max_health = constitution * 2;
        attackDamage = 2 + ((strength - 10) / 2); // attack damage
        HealPlayer(0);
        ChangeHealthBar();
    }

    void FinishTick() {
        OnTick?.Invoke();
        NextEnemy();
    }

    public void NextEnemy() {
        if (current_enemy >= enemies.Length) {
            done_with_tick = true;
            return;
        }
        current_enemy++;
        enemies[current_enemy - 1].MakeDecision();
    }

    // real is whether or not to try to actually hit, set to false to just play the animation
    private void Attack(Collider2D hit, Vector2 direction, bool real)
    {
        if (real) {
            hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(Convert.ToUInt32(attackDamage), hit.gameObject.tag);
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        }
        sfxPlayer.PlayAttackSound();
        PlayAnimation(direction, "attack");
    }

    Vector2 GetAxis() {
        Func<bool, int> BoolToInt = boolValue => boolValue ? 1 : 0;

        // todo: allow people to rebind movement keys
        int up = BoolToInt(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow));
        int down = BoolToInt(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow));
        int left = BoolToInt(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow));
        int right = BoolToInt(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow));

        float horizontal = (float) (right - left);
        float vertical = (float) (up - down);

        return new Vector2(horizontal, vertical);
    }

    // (if it hit something, what it hit)
    (bool, Collider2D) IsValidMove(Vector2 direction) {
        current_player_direction = direction;
        Collider2D hit = SendRaycast(direction);
        return (hit == null, hit);
    }

    string DirectionToString(Vector2 direction) {
        /*
        front: 0, -1
        back: 0, 1
        left: -1, 0
        right: 1, 0
        */

        if (direction == new Vector2(0, -1)) {
            return "front";
        } else if (direction == new Vector2(0, 1)) {
            return "back";
        } else if (direction == new Vector2(-1, 0)) {
            return "left";
        } else if (direction == new Vector2(1, 0)) {
            return "right";
        } else {
            return "front";
        }
    }

    // 1 is idle, 2 is hurt, 3 is attack
    private void PlayAnimation(Vector2 direction, string action) {
        string animation = $"Player_animation_{DirectionToString(direction)}_level_0_{action}";
        animator.Play(animation);
    }

    private Collider2D SendRaycast(Vector2 direction)
    {
        return Physics2D.Raycast(transform.position, direction, 1f, collideLayers).collider;

    }

    private void ChangeHealthBar() {
        float new_bar_width = (health / (float) (constitution * 2)) * 200;
        healthBar.sizeDelta = new Vector2(new_bar_width, healthBar.sizeDelta.y);
        healthBar.anchoredPosition = new Vector2(healthBar.sizeDelta.x / 2 + original_anchor_position, healthBar.anchoredPosition.y);
    }

    public void DamagePlayer(uint damage, bool dodgeable = true) {
        GameObject damageAmount = Instantiate(textFadePrefab, transform.position + new Vector3(rnd.Next(1, 5) / 10, rnd.Next(1, 5) / 10, 0), Quaternion.identity);
        if ((rnd.Next(10, 25) > dexterity && dodgeable) || !dodgeable)
        {
            health -= Convert.ToInt32(damage);
            sfxPlayer.PlayHurtSound();
            PlayAnimation(current_player_direction, "hurt");
            damageAmount.GetComponent<RealTextFadeUp>().SetText(damage.ToString(), Color.red, Color.white, 0.4f);
        } else {
            sfxPlayer.PlayDodgeSound();
            damageAmount.GetComponent<RealTextFadeUp>().SetText("dodged", Color.red, Color.white, 0.4f);
            // Instantiate(dodgePrefab, transform.position, Quaternion.identity);
            // todo: dodge animation
        }

        ChangeHealthBar();

        if (health <= 0) {
            onDie();
        }
    }

    public void Respawn() {
        health = max_health;
        transform.position = respawnPoint.position;
        ChangeHealthBar();
    }

    public (int, int) PlayerHealthInfo()
    {
        return (health, max_health);
    }

    // returns overflow health
    public int HealPlayer(int heal)
    {
        int overflowHealth = (health + heal) - max_health;
        health += heal;
        health = Math.Clamp(health, 0, max_health);
        ChangeHealthBar();
        return overflowHealth;
    }

    public void AttackAnimationFinishHandler()
    {
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        FinishTick();
    }
}
