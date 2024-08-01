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

    private PlayerSfx sfxPlayer;

    [Header("Misc")]
    [SerializeField] LayerMask collideLayers;
    [SerializeField] float movementDelay = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform healthBar;
    [SerializeField] GameObject dodgePrefab;
    [SerializeField] GameObject lockPrefab;
    [SerializeField] GameObject textFadePrefab;
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
    [SerializeField] public int maximum_stat_roll = 6;

    void Awake() {
        main = this;

        sfxPlayer = GetComponent<PlayerSfx>();

        if (healthBar == null) {
            healthBar = new GameObject().AddComponent<RectTransform>();
        }else {
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
                    Attack(hit, direction); // calls next enemy
                // if we hit a door, attempt to open it
                } else if (hit.gameObject.layer == LayerMask.NameToLayer("door")) {
                    // if the door needs a key, check if we have it
                    bool needsKey = hit.gameObject.GetComponent<Door>().NeedsKey;
                    bool hasKey = inventory.CheckIfItemExists(KeyID);
                    if ((needsKey && hasKey) || !needsKey) {
                        if (needsKey){
                            hit.GetComponent<Door>().sfxPlayer.PlayUnlockLockedSound();
                        }else{
                            hit.GetComponent<Door>().sfxPlayer.PlayUnlockedSound();
                        }

                        Destroy(hit.gameObject);
                        inventory.RemoveByID(KeyID);
                    } else {
                        hit.GetComponent<Door>().sfxPlayer.PlayLockedSound();

                        Debug.Log("need key");
                        Instantiate(lockPrefab, transform.position, Quaternion.identity);
                    }
                    FinishTick();
                // if we hit a fountain, heal from it
                } else if (hit.gameObject.layer == LayerMask.NameToLayer("breakable")) {
                    hit.gameObject.GetComponent<Breakable>().TakeHit(strength);
                    FinishTick();
                } else if (hit.gameObject.layer == LayerMask.NameToLayer("fountain")) {
                    hit.gameObject.GetComponent<Fountain>().Heal();
                    FinishTick();
                }
                else {
                    FinishTick();
                }
            }
        }
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

    private void Attack(Collider2D hit, Direction direction)
    {
        hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(Convert.ToUInt32(attackDamage), hit.gameObject.tag);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        sfxPlayer.PlayAttackSound();
        PlayAnimation(direction, 3);
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
        Debug.Log("what the fuck");
        if ((rnd.Next(10, 25) > dexterity && dodgeable) || !dodgeable)
        {
            health -= Convert.ToInt32(damage);
            sfxPlayer.PlayHurtSound();
            PlayAnimation(current_player_direction, 2);
            GameObject damageAmount = Instantiate(textFadePrefab, transform.position, Quaternion.identity);
            damageAmount.GetComponent<RealTextFadeUp>().SetText(damage.ToString());
        } else {
            Debug.Log("dodged");
            //sfxPlayer.PlayDodgeSound(); once we have a dodge sound effect
            GameObject damageAmount = Instantiate(textFadePrefab, transform.position, Quaternion.identity);
            damageAmount.GetComponent<RealTextFadeUp>().SetText("dodged");
            // Instantiate(dodgePrefab, transform.position, Quaternion.identity);
            // todo: dodge animation
        }

        ChangeHealthBar();

        if (health <= 0) {
            onDie();
        }
    }

    void Respawn() {
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
        switch (direction)
        {
            case PlayerAnimationPlayer.Direction.Up:
                transform.Translate(0, -0.5f, 0);
                break;
            case PlayerAnimationPlayer.Direction.Down:
                transform.Translate(0, 0.5f, 0);
                break;
            case PlayerAnimationPlayer.Direction.Left:
                transform.Translate(0.5f, 0, 0);
                break;
            case PlayerAnimationPlayer.Direction.Right:
                transform.Translate(-0.5f, 0, 0);
                break;
        }
        FinishTick();
    }

    // first int is stat, second int is modifier
    // stat 1 is constitution
    // stat 2 is dexterity
    // stat 3 is strength
    // stat 4 is wisdom
    public (int, int) ReturnItemStatModifier(int id)
    {
        int stat;
        int modifier;
        switch (id)
        {
            case 2:
                stat = 2;
                modifier = 1;
                break;
            case 3:
                stat = 1;
                modifier = 1;
                break;
            default:
                return (0, 0);
        }
        return (stat, modifier);
    }

    // bool is true to unequip and false to equip
    public void EquipItem(int id, bool unequip)
    {
        int negative = 1;
        if (unequip) { negative = -1; }
        (int, int) stat_tuple = ReturnItemStatModifier(id);
        switch (stat_tuple.Item1)
        {
            case 1:
                Console.WriteLine("{0} added to constitution", stat_tuple.Item2);
                constitution += stat_tuple.Item2 * negative;
                break;
            case 2:
                Console.WriteLine("{0} added to dexterity", stat_tuple.Item2);
                dexterity += stat_tuple.Item2 * negative;
                break;
            case 3:
                Console.WriteLine("{0} added to strength", stat_tuple.Item2);
                strength += stat_tuple.Item2 * negative;
                break;
            case 4:
                Console.WriteLine("{0} added to wisdom", stat_tuple.Item2);
                wisdom += stat_tuple.Item2 * negative;
                break;
            default:
                break;
        }
    }
}
