using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class Boss1 : MonoBehaviour, CanFight
{
    [SerializeField] GameObject statuePrefab;
    [SerializeField] GameObject baseEnemy;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] int AllowedAttackTicks = 7;
    [SerializeField] int Stages = 3;
    [SerializeField] int enemiesPerStage = 2;
    [SerializeField] Vector2Int roomSize = new Vector2Int(21, 11);
    [SerializeField] uint maxHealth = 15;

    [HideInInspector] public bool inFight = false;
    [HideInInspector] public int stage = 0;
    private uint _health = 15;
    public uint health
    {
        get { return _health; }
        set
        {
            _health = (uint)Mathf.Clamp(value, 0, maxHealth);
        }
    }

    private int ticks_till_move_back = 0;

    void Awake()
    {
        Controller.OnTick += CustomUpdate;
    }

    public void StartFight()
    {
        health = maxHealth;
        inFight = true;
        stage = 1;
        Debug.Log("i am 1ssoB, and i hate");
        SpawnStatue();
    }

    private Vector2 GenerateRandomPosition()
    {
        Vector2 bossPosition = transform.position;
        // man i hate unity
        Vector2 random_position = new Vector2(UnityEngine.Random.Range(-roomSize.x / 2, roomSize.x / 2), UnityEngine.Random.Range(-roomSize.y / 2, roomSize.y / 2));
        // also check that theres no enemies or statues already there
        while (Mathf.Abs(random_position.x) < 2 || Mathf.Abs(random_position.y) < 2 || Physics2D.OverlapCircle(new Vector2(Mathf.Round(random_position.x), Mathf.Round(random_position.y)) + bossPosition, 0.5f, LayerMask.GetMask("Enemy")))
        {
            random_position = new Vector2(UnityEngine.Random.Range(-roomSize.x / 2, roomSize.x / 2), UnityEngine.Random.Range(-roomSize.y / 2, roomSize.y / 2));
        }
        return new Vector2(Mathf.Round(random_position.x), Mathf.Round(random_position.y)) + bossPosition;
    }

    public void Attack() => SpawnStatue();

    private void SpawnStatue()
    {
        // generate statue position
        Vector2 statuePosition = GenerateRandomPosition();

        GameObject statue = Instantiate(statuePrefab, statuePosition, Quaternion.identity);

        statue.GetComponent<Statue>().boss = this;

        // also spawn enemies
        for (int i = 0; i < (enemiesPerStage + (stage / 2)); i++)
        {
            Vector2 enemyPosition = GenerateRandomPosition();
            // make a copy of the base enemy
            GameObject enemy = Instantiate(baseEnemy, enemyPosition, Quaternion.identity);
        }
    }

    public void StatueDestroyed()
    {
        stage++;
        Debug.Log("i am 1ssoB, and i hate (but i also love 👅) and im on stage " + stage);
        if (stage > Stages * 2)
        {
            Die();
        }

        ticks_till_move_back = AllowedAttackTicks;
    }

    public void Die()
    {
        inFight = false;
        stage = 0;
        Debug.Log("i am 1ssoB, and i hate (but im also dead so)");
        PlayAnimation("die");
        // set color to dark red
        spriteRenderer.color = new Color(0.5f, 0, 0);

        StartCoroutine(FullyDie(2));
    }

    IEnumerator FullyDie(float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        Destroy(gameObject);
        UIStateManager.singleton.OpenUIPage(UIState.Win);
    }

    private void CustomUpdate()
    {
        if (stage % 2 == 0 && stage != 0)
        {
            ticks_till_move_back--;
            Debug.Log("uh oh im running out of ticks! " + ticks_till_move_back);
            if (ticks_till_move_back <= 0)
            {
                stage--;
                SpawnStatue();
            }
        }
    }

    private void PlayAnimation(string action)
    {
        string animation = $"BOSS1_animation_{action}";

        animator.Play(animation);
    }

    public void Hurt(uint damage)
    {
        if (inFight && stage % 2 == 0)
        {
            health -= damage;
            Debug.Log("i am 1ssoB, and i hate (but i also love 👅) and im taking damage (" + damage + ", " + health + ")");
            PlayAnimation("damage");
            if (health <= 0)
            {
                health = maxHealth;
                stage++;
                Debug.Log("i am 1ssoB, and i am dead but maybe not potentially stage is now " + stage);
                if (stage > Stages * 2)
                {
                    Die();
                }
                else
                {
                    PlayAnimation("stunned");
                    SpawnStatue();
                }
            }
        }
    }

    public uint Heal(uint amount)
    {
        health += amount;
        return health;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 0));
    }
}
