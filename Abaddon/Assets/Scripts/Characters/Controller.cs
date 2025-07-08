using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Linq;

[RequireComponent(typeof(PlayerSfx)), RequireComponent(typeof(Inventory)), RequireComponent(typeof(BoxCollider2D))]

public class Controller : MonoBehaviour
{
    #region Variables
    public static Controller main;

    #region Stats
    [Header("Base Stats")]
    [SerializeField, Tooltip("Constitution (maximum health)")] public int constitution = 9;

    [SerializeField, Tooltip("Dexterity (dodge chance)")] public int dexterity = 9;

    [SerializeField, Tooltip("Strength (attack damage)")] public int strength = 9;
    [HideInInspector] public int attackDamage;

    [SerializeField, Tooltip("Wisdom (ability damage)")] public int wisdom = 9;

    [SerializeField, Tooltip("Minimum stat roll")] public int minimum_stat_roll = 1;
    [SerializeField, Tooltip("Maximum stat roll")] public int maximum_stat_roll = 7;
    [SerializeField, Tooltip("Sum of starting stats")] public int sum_of_starting_stats = 40;

    public int exp = 0;

    [SerializeField] Slider expBarVisual;

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
        private set
        {
            _health = value;
            _health = Math.Clamp(_health, 0, max_health);
            if (healthBarVisual) healthBarVisual.value = health;
            if (_health <= 0)
            {
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
    [HideInInspector] public static Action OnMoved;
    [HideInInspector] public EnemyMovement[] enemies;
    [HideInInspector] public bool done_with_tick = true;
    int current_enemy = 0;
    #endregion

    #region Other 
    [Header("Other")]
    [SerializeField] Animator animator;
    [SerializeField] CameraScript mainCamera;
    [HideInInspector] public PlayerSfx sfxPlayer;
    [HideInInspector] public Inventory inventory;
    #endregion

    #region Constants 
    const int KeyID = 1;
    #endregion

    #region Local Things
    private bool god_mode = false;
    private (bool, bool) god_mode_keys = (false, false);
    #endregion

    #region Movement Controls
    private class MoveState
    {
        public float start_hold_time = 0f;
        public bool held_last_frame = false;
        public float next_repeat_run = 0f;
    }

    private enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private Dictionary<MovementDirection, MoveState> movementStates = new Dictionary<MovementDirection, MoveState>();

    [SerializeField] float initialMovementDelay = 0.3f;
    [SerializeField] float holdDelay = 0.03f;

    #endregion

    #endregion

    void Awake()
    {
        main = this;
        onDie += () => UIStateManager.singleton.OpenUIPage(UIState.Death);

        sfxPlayer = GetComponent<PlayerSfx>();
        inventory = FindObjectOfType<Inventory>();

        #region Generate Stats
        float minPercentage = (float)minimum_stat_roll / sum_of_starting_stats;
        float maxPercentage = (float)maximum_stat_roll / sum_of_starting_stats;

        float[] newStats = new float[4];
        for (int i = 0; i < newStats.Length; i++)
        {
            newStats[i] = UnityEngine.Random.Range(minPercentage, maxPercentage);
        }

        float total = newStats.Sum();
        for (int i = 0; i < newStats.Length; i++)
        {
            newStats[i] = newStats[i] / total;
        }

        var results = newStats.Select(stat => Mathf.RoundToInt(stat * sum_of_starting_stats)).ToArray();

        int diff = results.Sum() - sum_of_starting_stats;
        if (diff != 0) results[Array.IndexOf(results, diff < 0 ? results.Min() : results.Max())] -= diff; // If the rounding messed up, adjust the highest or lowest stat accordingly

        strength = results[0];
        dexterity = results[1];
        constitution = results[2];
        wisdom = results[3];
        #endregion

        max_health = constitution * 2;
        health = max_health;

        if (healthBarVisual)
        {
            healthBarVisual.maxValue = max_health;
            healthBarVisual.minValue = 0;
        }

        if (expBarVisual)
        {
            expBarVisual.maxValue = 100;
            expBarVisual.minValue = 0;
            expBarVisual.value = exp;
        }

        if (respawnPoint == null)
        {
            respawnPoint = Instantiate(transform, transform.position, Quaternion.identity);
        }

        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        Transform targetPosition = boss.transform;

        mainCamera.ResetTarget(null, false, null);
        // disable player movement until the camera has panned
        // done_with_tick = false;
        // StartCoroutine(AfterDelay(1f, () =>
        //     mainCamera.ChangeTarget(targetPosition, 0.5f, onComplete: () =>
        //     {
        //         mainCamera.ResetTarget(0.5f, onComplete: () =>
        //         {
        //             print("Camera reset complete");
        //             done_with_tick = true;
        //         });
        //     })
        // ));
    }

    void Update()
    {
        UpdateStats();

        bool god_mode_keys_prev = god_mode_keys.Item1;
        bool god_mode_keys_pressed = Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.J);
        god_mode_keys = (god_mode_keys_pressed, god_mode_keys_prev);

        if (god_mode_keys.Item1 && !god_mode_keys.Item2 && god_mode)
        {
            print("God mode deactivated");
            god_mode = false;
            god_mode_keys = (true, true);
            // round to nearest tile
            transform.position = new Vector3(
                Mathf.Round(transform.position.x - 0.5f) + 0.5f,
                Mathf.Round(transform.position.y - 0.5f) + 0.5f,
                transform.position.z
            );
        }

        enemies = FindObjectsOfType<EnemyMovement>();
        if (!done_with_tick) return;

        if (god_mode_keys.Item1 && !god_mode_keys.Item2 && !god_mode)
        {
            god_mode = true;
            print("God mode activated");
        }

        if (god_mode)
        {
            GodModeMove();
            return;
        }

        Move();

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            health += 1;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            DamagePlayer(1, false);
        }
    }

    void GodModeMove()
    {
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        transform.Translate(direction * Time.deltaTime * 60f * .75f);
    }

    bool CanMove(GameObject[] objectsAhead)
    {
        return !objectsAhead.Any(obj => (collideLayers.value & (1 << obj.layer)) != 0);
    }

    void Move()
    {
        Vector2 direction = GetAxis();
        if (direction.magnitude != 1) { return; } // if we are not moving, do nothing. if we are going diagonally, do nothing

        done_with_tick = false;
        current_player_direction = direction;
        GameObject[] objectsAhead = SendRaycast(direction);
        bool canMove = CanMove(objectsAhead);

        current_enemy = 0;
        PlayAnimation("idle", direction);

        if (Time.time - lastMovement <= movementDelay) return;
        if (canMove) print("doing move: " + (Time.time - lastMovement));
        lastMovement = Time.time;

        if (canMove)
        {

            transform.Translate(direction);
            sfxPlayer.PlayWalkSound();
            OnMoved?.Invoke();
            FinishTick();
        }

        bool did_something = false;
        foreach (GameObject obj in objectsAhead)
        {
            if (obj.TryGetComponent(out CanBeHurt hurtable))
            {
                hurtable.Hurt((uint)attackDamage);
                FinishTick();
                did_something = true;
            }
            if (obj.TryGetComponent(out CanBeInteractedWith interactable))
            {
                interactable.Interact();
                FinishTick();
                did_something = true;
            }
            if (obj.TryGetComponent(out CanFight enemy))
            {
                Attack(enemy, direction); // calls next enemy so no need for a finish tick
                did_something = true;
            }

            if (!did_something) FinishTick();
        }
    }

    public void UpdateStats()
    {
        double health_percentage = (double)health / (double)max_health;
        max_health = constitution * 2;
        if (healthBarVisual)
        {
            healthBarVisual.maxValue = max_health;
        }
        health = (int)((double)max_health * health_percentage);
        attackDamage = 2 + ((strength - 10) / 2); // attack damage
        HealPlayer(0);
    }

    public void FinishTick()
    {
        OnTick?.Invoke();
        NextEnemy();
    }

    public void NextEnemy()
    {
        if (current_enemy >= enemies.Length)
        {
            done_with_tick = true;
            return;
        }
        current_enemy++;
        enemies[current_enemy - 1].MakeDecision();
    }

    private void Attack(CanFight enemy, Vector2 direction)
    {
        BaseAbility attack = AbilitySwapper.getAbility(main); // Get current attack. Useful if we add more abilities later.

        if (attack.CanUse(enemy, direction))
        {
            print($"attacking {enemy.GetType().Name} with {attack.GetType().Name}");
            attack.Attack(enemy, direction, animator, sfxPlayer);
        }
        else
        {
            FinishTick();
        }
    }

    private bool ShouldMove(MovementDirection dir, bool isHeld)
    {
        if (!movementStates.ContainsKey(dir))
            movementStates[dir] = new MoveState();

        MoveState state = movementStates[dir];

        if (isHeld)
        {
            if (!state.held_last_frame)
            {
                state.held_last_frame = true;
                state.start_hold_time = Time.time + initialMovementDelay;
                return true;
            }
            else if (state.start_hold_time <= Time.time)
            {
                //print(state.next_repeat_run + " " + holdDelay);
                if (state.next_repeat_run <= Time.time)
                {
                    state.next_repeat_run = Time.time + holdDelay;
                    return true;
                }
            }
        }
        else
        {
            state.held_last_frame = false;
            state.start_hold_time = float.PositiveInfinity;
        }

        return false;
    }

    Vector2 GetAxis()
    {
        Func<bool, int> BoolToInt = boolValue => boolValue ? 1 : 0;

        // todo: allow people to rebind movement keys


        bool up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        up = ShouldMove(MovementDirection.Up, up);
        down = ShouldMove(MovementDirection.Down, down);
        left = ShouldMove(MovementDirection.Left, left);
        right = ShouldMove(MovementDirection.Right, right);

        int i_up = BoolToInt(up);
        int i_down = BoolToInt(down);
        int i_left = BoolToInt(left);
        int i_right = BoolToInt(right);

        float horizontal = i_right - i_left;
        float vertical = i_up - i_down;

        return new Vector2(horizontal, vertical);
    }

    // (if it hit something, what it hit)
    GameObject[] SendRaycast(Vector2 direction)
    {
        Vector2 world_position = (Vector2)transform.position + (direction * 1.02f / 2f); // convert direction to relative position (ex: up = (0, 1)), then add it to the current position

        return Physics2D.RaycastAll(world_position, direction, .5f).Select(hit => hit.collider.gameObject).ToArray();
    }

    string DirectionToAnimationLabel(Vector2 direction)
    {
        if (direction == Vector2.down)
        {
            return "front";
        }
        else if (direction == Vector2.up)
        {
            return "back";
        }
        else if (direction == Vector2.left)
        {
            return "left";
        }
        else if (direction == Vector2.right)
        {
            return "right";
        }

        throw new Exception("Invalid direction");
    }

    public void PlayAnimation(string action, Vector2? facingDirection = null)
    {
        if (facingDirection == null) facingDirection = current_player_direction;
        string animation = $"Player_animation_{DirectionToAnimationLabel((Vector2)facingDirection)}_level_0_{action}";
        animator.Play(animation);
    }

    public void DamagePlayer(uint damage, bool dodgeable = true)
    {
        if (dodgeable && DodgedAttack())
        {
            sfxPlayer.PlayDodgeSound();
            Helpers.singleton.SpawnHurtText("dodged", transform.position);
        }
        else
        {
            health -= (int)damage;
            sfxPlayer.PlayHurtSound();
            PlayAnimation("hurt");
            Helpers.singleton.SpawnHurtText(damage.ToString(), transform.position);
        }
    }

    bool DodgedAttack()
    {
        return UnityEngine.Random.value < DexterityToPercent();
    }

    float DexterityToPercent()
    {
        return dexterity / 20f;
    }

    public void Respawn()
    {
        health = max_health;
        transform.position = respawnPoint.position;
        UIStateManager.singleton.CloseUIPage(UIState.Death);
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

    public void add_exp(int exp)
    {
        this.exp += exp;
    }
}