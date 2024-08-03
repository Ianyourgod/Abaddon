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

    //needed for a git pr
    public enum Direction {
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
    public int health;
    public int max_health;
    public int attackDamage;

    public System.Random rnd = new System.Random();

    public delegate void OnDie();
    public OnDie onDie;

    [HideInInspector] public PlayerSfx sfxPlayer;

    [HideInInspector]
    public GameObject textFadePrefab;
    [HideInInspector]
    public GameObject lockPrefab;

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
        inventory = FindObjectOfType<Inventory>();

        // stat randomization
        constitution += rnd.Next(1, maximum_stat_roll);
        dexterity += rnd.Next(1, maximum_stat_roll);
        strength += rnd.Next(1, maximum_stat_roll);
        wisdom += rnd.Next(1, maximum_stat_roll);

        health = constitution * 2; // current health
        max_health = health;
        ChangeHealthBar();

        textFadePrefab = (UnityEngine.GameObject)Resources.Load($"Prefabs/TextFadeCreator");
        lockPrefab = (UnityEngine.GameObject)Resources.Load($"Prefabs/AnimatedLock");
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
        PlayAnimation(direction, "idle");

        const int KeyID = 1;
        
        if (Time.time - lastMovement > movementDelay) {
            if (validMove || hit.gameObject.layer == LayerMask.NameToLayer("floorTrap")) {
                transform.Translate(horizontal, vertical, 0);
                sfxPlayer.PlayWalkSound();
                lastMovement = Time.time;
                FinishTick();
            } else {
                //print($"layer number: {LayerMask.NameToLayer("breakable")}");
                // if we hit an enemy, attack it
                if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                    Attack(hit, direction, true); // calls next enemy
                // if we hit a door, attempt to open it
                } else if (hit.gameObject.layer == LayerMask.NameToLayer("door")) {
                    // if the door needs a key, check if we have it
                    bool needsKey = hit.gameObject.GetComponent<Door>().NeedsKey;
                    bool hasKey = inventory.CheckIfItemExists(KeyID);
                    if ((needsKey && hasKey) || !needsKey) {
                        if (needsKey) {
                            inventory.RemoveByID(KeyID);
                            hit.GetComponent<DoorSfx>().PlayUnlockLockedSound();
                        } else {
                            hit.GetComponent<DoorSfx>().PlayUnlockedSound();
                        }

                        Destroy(hit.gameObject);
                    } else {
                        hit.GetComponent<DoorSfx>().PlayLockedSound();
                        Instantiate(lockPrefab, transform.position, Quaternion.identity);
                    }
                    FinishTick();
                }
                // if we hit a breakable, destroy it
                else if (hit.gameObject.layer == LayerMask.NameToLayer("breakable")) {
                    hit.GetComponent<BreakableSfx>().PlayBreakSound();
                    hit.gameObject.GetComponent<Breakable>().TakeHit(strength);
                    Attack(hit, direction, false);
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
    private void Attack(Collider2D hit, Direction direction, bool real)
    {
        if (real) {
            hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(Convert.ToUInt32(attackDamage), hit.gameObject.tag);
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        }
        sfxPlayer.PlayAttackSound();
        PlayAnimation(direction, "attack");
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

    string DirectionToString(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return "back";
            case Direction.Down:
                return "front";
            case Direction.Left:
                return "left";
            case Direction.Right:
                return "right";
        }
        return "forward";
    }

    // 1 is idle, 2 is hurt, 3 is attack
    private void PlayAnimation(Direction direction, string action) {
        string animation = $"Player_animation_{DirectionToString(direction)}_level_0_{action}";
        animator.Play(animation);
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

    public void AttackAnimationFinishHandler(PlayerAnimationPlayer.Direction direction)
    {
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        FinishTick();
    }
}
