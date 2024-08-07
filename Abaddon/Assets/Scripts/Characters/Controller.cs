using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class Controller : MonoBehaviour {
    #region Variables
        public static Controller main;
        
        #region Stats
            [Header("Base Stats")]
            [SerializeField, Tooltip("Constitution (maximum health)")] private int constitution = 9;
            
            [SerializeField, Tooltip("Dexterity (dodge chance)")] private int dexterity = 9;
            
            [SerializeField, Tooltip("Strength (attack damage)")] private int strength = 9;

            [SerializeField, Tooltip("Wisdom (ability damage)")] private int wisdom = 9;
            
            [SerializeField, Tooltip("High end of range to add")] private int statVariance = 7;
            [Space]
            
            public Stats stats = new Stats();
        #endregion

        #region Health
            [Header("Health")]
            [SerializeField] Slider healthBarVisual;
            [SerializeField] Transform respawnPoint;
            Vector3 respawnPointPosition;
            [Space]
            [HideInInspector] public Action onDie;
            public int health
            {
                get => _health;
                private set {
                    _health = value;
                    _health = Math.Clamp(_health, 0, stats.GetMaxHealth());
                    if (healthBarVisual) healthBarVisual.value = health;
                    if (_health <= 0) {
                        onDie?.Invoke();
                    }
                    onHealthChanged?.Invoke(_health);
                }
            }
            public Action<int> onHealthChanged;
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

    [SerializeField] Animator gnome;

    #region Life cycle functions
    void Awake() {
        main = this;

        sfxPlayer = GetComponent<PlayerSfx>();
        inventory = FindObjectOfType<Inventory>();

        // stat randomization
        stats = new Stats(constitution, dexterity, strength, wisdom, statVariance);

        if (healthBarVisual) {
            healthBarVisual.maxValue = stats.GetMaxHealth();
            stats.onChangeConstitution += (_) => healthBarVisual.maxValue = stats.GetMaxHealth();
            stats.onChangeConstitution += (_) => health = stats.GetMaxHealth();
        }
        
        constitution += UnityEngine.Random.Range(1, statVariance);
        dexterity += UnityEngine.Random.Range(1, statVariance);
        strength += UnityEngine.Random.Range(1, statVariance);
        wisdom += UnityEngine.Random.Range(1, statVariance);

        health = stats.GetMaxHealth();

        if (respawnPoint == null) respawnPointPosition = transform.position;
        else respawnPointPosition = respawnPoint.position;
    }

    void Update() {
        if (!IsDoneWithTickCycle()) return;

        Move();

        BugTesting();
    }
    #endregion

    bool IsDoneWithTickCycle() {
        enemies = FindObjectsOfType<EnemyMovement>();
        if (!done_with_tick) {
            if (current_enemy >= enemies.Length) done_with_tick = true;
            
            else return false;
        }

        return true;
    }

    void BugTesting() {
        if (Input.GetKeyDown(KeyCode.Equals)) health += 1;
        if (Input.GetKeyDown(KeyCode.Minus)) DamagePlayer(1, false);
        
        if (Input.GetKeyDown(KeyCode.Comma)) stats.constitution += 1;
        if (Input.GetKeyDown(KeyCode.Period)) stats.constitution -= 1;

        if (Input.GetKeyDown(KeyCode.T)) gnome.Play("Goblin_animation_back_attack");
    }

    void Move() {
        Vector2 direction = GetCardinalInputs();

        // if we are not moving, do nothing. if we are going diagonally, do nothing
        if (direction == Vector2.zero || (direction.x != 0 && direction.y != 0)) {
            return;
        }

        done_with_tick = false;
        current_player_direction = direction;
        Collider2D hit = GetMovingToTile(direction);
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
            sfxPlayer?.PlayWalkSound();
            FinishTick();
        } 
        // if we hit an enemy, attack it
        else if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Attack(hit, direction, true); // calls next enemy
        } 
        // if we hit a door, attempt to open it
        else if (hit.gameObject.layer == LayerMask.NameToLayer("interactable")) {
            hit.GetComponent<Interactable>().Interact(stats.GetAttackDamage());
            FinishTick();
        }
        // if we hit a fountain, heal from it
        else if (hit.gameObject.layer == LayerMask.NameToLayer("fountain")) {
            if (health < stats.GetMaxHealth())
                hit.GetComponent<FountainSfx>().PlayFountainSound();
            hit.gameObject.GetComponent<Fountain>().Heal();
            FinishTick();
        }
        else {
            FinishTick();
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
        enemies[current_enemy++].MakeDecision();
    }

    // real is whether or not to try to actually hit, set to false to just play the animation
    private void Attack(Collider2D hit, Vector2 direction, bool real)
    {
        if (real) {
            hit.gameObject.GetComponent<EnemyMovement>().DamageEnemy(Convert.ToUInt32(stats.GetAttackDamage()), hit.gameObject.tag);
            animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        }
        sfxPlayer?.PlayAttackSound();
        PlayAnimation("attack", direction);
    }

    Vector2 GetCardinalInputs() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    // (if it hit something, what it hit)
    Collider2D GetMovingToTile(Vector2 direction) {
        Vector2 centerOfBox = (Vector2)transform.position + direction;
        return Physics2D.OverlapBox(centerOfBox, new Vector3(0.9f, 0.9f, 0), 0, collideLayers);
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
            sfxPlayer?.PlayDodgeSound();
            Helpers.singleton.SpawnHurtText("dodged", transform.position);
        }
        else {
            health -= (int)damage;
            sfxPlayer?.PlayHurtSound();
            PlayAnimation("hurt");
            Helpers.singleton?.SpawnHurtText(damage.ToString(), transform.position);
        } 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + current_player_direction, new Vector3(0.9f, 0.9f, 0));   
    }

    bool DodgedAttack() {
        return UnityEngine.Random.value < stats.DexterityToPercent();
    }

    float DexterityToPercent() {
        return dexterity / 20f;
    }

    public void Respawn() {
        health = stats.GetMaxHealth();
        transform.position = respawnPointPosition;
    }

    // returns overflow health
    public int HealPlayer(int heal)
    {
        int overflowHealth = health + heal - stats.GetMaxHealth();
        health += heal;
        return overflowHealth;
    }

    public void AttackAnimationFinishHandler()
    {
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Characters");
        FinishTick();
    }
}