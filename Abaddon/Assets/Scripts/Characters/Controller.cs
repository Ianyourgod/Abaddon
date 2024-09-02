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
            [HideInInspector] public List<EnemyMovement> enemies;
            int current_enemy = -1;
        #endregion

        #region Other 
            [Header("Other")]
            [SerializeField] Animator animator;
            [SerializeField] AnimationEventHandler animationEventHandler;
            [HideInInspector] public PlayerSfx sfxPlayer;
            [HideInInspector] public Inventory inventory;
        #endregion
        
        #region Constants 
            const int KeyID = 1;
        #endregion
    #endregion

    AnimationEventHandler.Animation hurtAnimation;
    AnimationEventHandler.Animation attackAnimation;
    AnimationEventHandler.Animation deathAnimation;

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
        InitializeAnimationHandler();
    }

    void OnAttackEnd() {
        print("does nothing yet");
    }

    void OnHurtEnd() {
        print("does nothing yet");
    }

    void OnDieEnd() {
        print("does nothing yet");
    }

    void InitializeAnimationHandler() {
        animationEventHandler.data = new PlayerAnimationHandlerData();
        ((PlayerAnimationHandlerData)animationEventHandler.data).onAttackEnd += OnAttackEnd;
        ((PlayerAnimationHandlerData)animationEventHandler.data).onHurtEnd += OnHurtEnd;
        ((PlayerAnimationHandlerData)animationEventHandler.data).onDeathEnd += OnDieEnd;
        
        
        hurtAnimation = new AnimationEventHandler.Animation(
            action: () => $"{animationEventHandler.data.animationPrefix}_animation_{current_player_direction.ToStringDirection()}_level_0_hurt", 
            priority: 1, 
            shouldLoop: false,
            persistUntilPlayed: false
        );
        attackAnimation = new AnimationEventHandler.Animation(
            action: () => $"{animationEventHandler.data.animationPrefix}_animation_{current_player_direction.ToStringDirection()}_level_0_attack", 
            priority: 1, 
            shouldLoop: false,
            persistUntilPlayed: true
        );

        deathAnimation = new AnimationEventHandler.Animation(
            action: () => $"{animationEventHandler.data.animationPrefix}_level_0_animation_death", 
            priority: 2, 
            shouldLoop: false,
            persistUntilPlayed: true
        );

        animationEventHandler.data.defaultAnimation = new AnimationEventHandler.Animation(
            action: () => $"{animationEventHandler.data.animationPrefix}_animation_{current_player_direction.ToStringDirection()}_level_0_idle",
            priority: 0, 
            shouldLoop: true,
            persistUntilPlayed: false
        );
    }

    void Update() {
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
        print("direction: " + direction);

        // if we are not moving, do nothing. if we are going diagonally, do nothing
        if (direction == Vector2.zero || (direction.x != 0 && direction.y != 0)) return;

        current_player_direction = direction;
        
        if (Time.time - lastMovement <= movementDelay) return;
        lastMovement = Time.time;


        GameObject[] hits = GetAllTilesInFront(direction);
        if (hits.Length == 0) {
            transform.Translate(direction);
            sfxPlayer?.PlayWalkSound();
            Physics2D.SyncTransforms();
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

    void StartPlayerTick() {
        OnPlayerTick?.Invoke();
        current_enemy = 0;
        enemies = FindObjectsOfType<EnemyMovement>().ToList();
        NextEnemy();
    }

    public void NextEnemy() {
        if (IsDoneWithTickCycle()) return;
        
        enemies[current_enemy++]?.MakeDecision();

        // This is the last enemy
        if (current_enemy >= enemies.Count) current_enemy = -1; 
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
        animationEventHandler.QueueAnimation(attackAnimation);
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

    public void Hurt(uint damage, bool dodgeable = true) {
        if (dodgeable && DodgedAttack()) {
            sfxPlayer?.PlayDodgeSound();
            Helpers.singleton.SpawnHurtText("dodged", transform.position);
        }
        else {
            health -= (int)damage;
            sfxPlayer?.PlayHurtSound();
            animationEventHandler.QueueAnimation(hurtAnimation);
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
    }
}