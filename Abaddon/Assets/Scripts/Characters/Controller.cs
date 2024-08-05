using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerSfx)), RequireComponent(typeof(Inventory)), RequireComponent(typeof(BoxCollider2D))]

public class Controller : MonoBehaviour {
    #region Variables
        public static Controller main;
        
        #region Stats
            [Header("Base Stats")]
            [SerializeField, Tooltip("Constitution (maximum health)")] public int constitution = 9;
            
            [SerializeField, Tooltip("Dexterity (dodge chance)")] public int dexterity = 9;
            
            [SerializeField, Tooltip("Strength (attack damage)")] public int strength = 9;
            [HideInInspector] public int attackDamage;

            [SerializeField, Tooltip("Wisdom (ability damage)")] public int wisdom = 9;
            
            [SerializeField, Tooltip("High end of range to add")] public int maximum_stat_roll = 7;
            [Space]
        #endregion

        #region Health
            [Header("Health")]
            [SerializeField] Slider healthBarVisual;
            [SerializeField] Transform respawnPoint;
            [Space]
            [HideInInspector] public Action onDie;
            [HideInInspector] public int max_health;
            public int health
            {
                get => _health;
                private set {
                    _health = value;
                    _health = Math.Clamp(_health, 0, max_health);
                    if (healthBarVisual) healthBarVisual.value = health;
                    if (_health <= 0) {
                        onDie?.Invoke();
                    }
                }
            }    
            int _health;
        #endregion

        #region Movement
            [Header("Movement")]
            [SerializeField] LayerMask collideLayers;
            [SerializeField] float movementDelay = 0.1f;
            [Space]
            Vector2 current_player_direction = new Vector2(0, -1);
            float lastMovement = 0f;
        #endregion

        #region Player Update System 
            [HideInInspector] public static Action OnTick;
            [HideInInspector] public EnemyMovement[] enemies;
            [HideInInspector] public bool done_with_tick = true;
            int current_enemy = 0;
        #endregion

        #region Other 
            [Header("Other")]
            [SerializeField] Animator animator;
            [HideInInspector] public PlayerSfx sfxPlayer;
            [HideInInspector] public Inventory inventory;
        #endregion
        
        #region Constants 
            const int KeyID = 1;
        #endregion
    #endregion

    void Awake() {
        main = this;

        sfxPlayer = GetComponent<PlayerSfx>();
        inventory = FindObjectOfType<Inventory>();

        // stat randomization
        constitution += UnityEngine.Random.Range(1, maximum_stat_roll);
        dexterity += UnityEngine.Random.Range(1, maximum_stat_roll);
        strength += UnityEngine.Random.Range(1, maximum_stat_roll);
        wisdom += UnityEngine.Random.Range(1, maximum_stat_roll);

        max_health = constitution * 2;
        health = max_health;

        if (healthBarVisual) {
            healthBarVisual.maxValue = max_health;
            healthBarVisual.minValue = 0;
        }

        if (respawnPoint == null) {
            respawnPoint = Instantiate(transform, transform.position, Quaternion.identity);
        }
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

        if (Input.GetKeyDown(KeyCode.Equals)) {
            health += 1;
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            DamagePlayer(1, false);
        }
    }

    void Move() {
        Vector2 direction = GetAxis();
        // if we are not moving, do nothing. if we are going diagonally, do nothing
        if (direction == Vector2.zero || (direction.x != 0 && direction.y != 0)) {
            return;
        }

        done_with_tick = false;
        current_player_direction = direction;
        Collider2D hit = SendRaycast(direction);
        bool validMove = hit == null;

        current_enemy = 0;
        PlayAnimation("idle", direction);
        
        if (Time.time - lastMovement <= movementDelay) {
            return;
        }

        lastMovement = Time.time;

        // you hit a wall
        if (!validMove && hit == null) {
            FinishTick();
            return;
        }

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
                hit.GetComponent<FountainSfx>().PlayFountainSound();
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
        PlayAnimation("attack", direction);
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
    
    public Vector2 V3_2_V2(Vector3 vec) {
        return new Vector2(vec.x, vec.y);
    }

    // (if it hit something, what it hit)
    Collider2D SendRaycast(Vector2 direction) {
        Vector2 world_position = V3_2_V2(transform.position) + (direction * 1.02f  / 2f);

        Collider2D hit = Physics2D.Raycast(world_position, direction, .5f, collideLayers).collider;

        return hit;
    }

    string DirectionToAnimationLabel(Vector2 direction) {
        if (direction == Vector2.down) {
            return "front";
        } else if (direction == Vector2.up) {
            return "back";
        } else if (direction == Vector2.left) {
            return "left";
        } else if (direction == Vector2.right) {
            return "right";
        }

        throw new Exception("Invalid direction");
    }

    private void PlayAnimation(string action, Vector2? facingDirection = null) {
        if (facingDirection == null) facingDirection = current_player_direction;
        string animation = $"Player_animation_{DirectionToAnimationLabel((Vector2)facingDirection)}_level_0_{action}";
        animator.Play(animation);
    }

    public void DamagePlayer(uint damage, bool dodgeable = true) {
        if (dodgeable && DodgedAttack()) {
            sfxPlayer.PlayDodgeSound();
            Helpers.singleton.SpawnHurtText("dodged", transform.position);
        }
        else {
            health -= (int)damage;
            sfxPlayer.PlayHurtSound();
            PlayAnimation("hurt");
            Helpers.singleton.SpawnHurtText(damage.ToString(), transform.position);
        } 
    }

    bool DodgedAttack() {
        return UnityEngine.Random.value < DexterityToPercent();
    }

    float DexterityToPercent() {
        return dexterity / 20f;
    }

    public void Respawn() {
        health = max_health;
        transform.position = respawnPoint.position;
    }

    // returns overflow health
    public int HealPlayer(int heal)
    {
        int overflowHealth = health + heal - max_health;
        health += heal;
        return overflowHealth;
    }

    public void AttackAnimationFinishHandler()
    {
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        FinishTick();
    }
}