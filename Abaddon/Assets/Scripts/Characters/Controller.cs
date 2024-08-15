using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.VisualScripting;


public class Controller : MonoBehaviour, Damageable, Attackable {
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
            [HideInInspector] public static Action OnPlayerTick;
            [HideInInspector] public List<Enemy> enemies;
            int current_enemy = -1;
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
        print("cycling tick: " + IsDoneWithTickCycle());
        if (!IsDoneWithTickCycle()) return;

        Move();

        BugTesting();
    }

    bool IsDoneWithTickCycle() {
        return current_enemy < 0 || current_enemy >= enemies.Count;
    }

    void BugTesting() {
        if (Input.GetKeyDown(KeyCode.Equals)) health += 1;
        if (Input.GetKeyDown(KeyCode.Minus)) Hurt(1, false);
        
        if (Input.GetKeyDown(KeyCode.Comma)) stats.constitution += 1;
        if (Input.GetKeyDown(KeyCode.Period)) stats.constitution -= 1;
    }

    void Move() {
        Vector2 direction = GetCardinalInputs();

        // if we are not moving, do nothing. if we are going diagonally, do nothing
        if (direction == Vector2.zero || (direction.x != 0 && direction.y != 0)) return;

        current_player_direction = direction;

        PlayAnimation("idle", direction);
        
        if (Time.time - lastMovement <= movementDelay) return;
        lastMovement = Time.time;


        GameObject[] hits = GetAllTilesInFront(direction);
        if (hits.Length == 0) {
            transform.Translate(direction);
            sfxPlayer?.PlayWalkSound();
            print("moved");
        }
        foreach (GameObject hit in hits) {
            if (hit.TryGetComponent(out Interactable interactable)) {
                interactable.Interact();
            }
            if (hit.TryGetComponent(out Damageable enemy)) {
                Attack(enemy);
            }
        }

        StartPlayerTick();
    }
    
    void QueuePlayerTick() {
        current_enemy = -2;
    }

    private void FixedUpdate()
    {
        if (current_enemy == -2) StartPlayerTick();
    }

    void StartPlayerTick() {
        OnPlayerTick?.Invoke();
        current_enemy = 0;
        Invoke("NextEnemy", 0.3f);
    }

    public void NextEnemy() {
        if (IsDoneWithTickCycle()) return;
        
        print("calling enemy");
        enemies[current_enemy++]?.MakeDecision();

        print($"count: {enemies.Count} | current: {current_enemy}");
        if (current_enemy >= enemies.Count) {
            current_enemy = -1;
            return;
        }
    }

    public void Attack(uint damage)
    {
        throw new NotImplementedException();
    }
    
    public void Attack(Damageable enemy)
    {
        enemy.Hurt((uint) stats.GetAttackDamage(), false);
        animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        sfxPlayer?.PlayAttackSound();
        PlayAnimation("attack", current_player_direction);
    }

    Vector2 GetCardinalInputs() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    GameObject[] GetAllTilesInFront(Vector2 direction) {
        Vector2 centerOfBox = (Vector2)transform.position + direction;
        return Physics2D.OverlapBoxAll(centerOfBox, new Vector3(0.9f, 0.9f, 0), 0, collideLayers).Select(hit => hit.gameObject).ToArray();
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

    public void Hurt(uint damage, bool dodgeable = true) {
        if (dodgeable && DodgedAttack()) {
            sfxPlayer?.PlayDodgeSound();
            PlayAnimation("idle");
            Helpers.singleton.SpawnHurtText("dodged", transform.position);
        }
        else {
            health -= (int)damage;
            sfxPlayer?.PlayHurtSound();
            PlayAnimation("hurt");
            Helpers.singleton?.SpawnHurtText(damage.ToString(), transform.position);
        } 
    }

    bool DodgedAttack() {
        return UnityEngine.Random.value < stats.DexterityToPercent();
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
        StartPlayerTick();
    }
}