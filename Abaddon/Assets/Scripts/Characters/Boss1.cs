using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShortcutManagement;
using UnityEngine;

public class Boss1 : MonoBehaviour, CanFight
{
    [SerializeField]
    List<Statue> statues = new List<Statue>();

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public bool vulnerable = false;

    [SerializeField]
    GameObject[] enemiesToSpawn;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Animator animator;

    [SerializeField]
    int enemiesPerStage = 5;

    [SerializeField]
    int increasePerStage = 2;

    [SerializeField]
    Vector2Int roomSize = new Vector2Int(21, 11);

    [SerializeField]
    int damagePerRound = 15;

    [HideInInspector]
    public bool inFight = false;

    private int damageTakenThisRound = 0;
    private int maxStages = 0;

    void Awake()
    {
        maxStages = statues.Count;
        Controller.main.onDie += () =>
        {
            if (inFight)
            {
                foreach (GameObject enemy in spawnedEnemies)
                {
                    Destroy(enemy);
                }
            }
        };
    }

    public EnemyType GetEnemyType()
    {
        return EnemyType.Boss1;
    }

    public void StartFight()
    {
        inFight = true;
        Debug.Log("i am 1ssoB, and i hate");
        SpawnStatue();
    }

    private Vector2 GenerateRandomPosition()
    {
        Vector2 bossPosition = transform.position;
        // man i hate unity
        Vector2 random_position = new Vector2(
            UnityEngine.Random.Range(-roomSize.x / 2, roomSize.x / 2),
            UnityEngine.Random.Range(-roomSize.y / 2, roomSize.y / 2)
        );
        // also check that theres no enemies or statues already there
        while (
            Mathf.Abs(random_position.x) < 2
            || Mathf.Abs(random_position.y) < 2
            || Physics2D.OverlapCircle(
                new Vector2(Mathf.Round(random_position.x), Mathf.Round(random_position.y))
                    + bossPosition,
                0.5f,
                LayerMask.GetMask("Enemy")
            )
        )
        {
            random_position = new Vector2(
                UnityEngine.Random.Range(-roomSize.x / 2, roomSize.x / 2),
                UnityEngine.Random.Range(-roomSize.y / 2, roomSize.y / 2)
            );
        }
        return new Vector2(Mathf.Round(random_position.x), Mathf.Round(random_position.y))
            + bossPosition;
    }

    private void SpawnStatue()
    {
        // generate statue position
        vulnerable = false;

        print("Activating the next statue at stage " + (maxStages - statues.Count));
        statues.Pop().Activate();
        print("done activating the statue");

        // also spawn enemies
        for (int i = 0; i < (enemiesPerStage + increasePerStage * (maxStages - statues.Count)); i++)
        {
            Vector2 enemyPosition = GenerateRandomPosition();
            // make a copy of the base enemy
            var randomEnemy = enemiesToSpawn[UnityEngine.Random.Range(0, enemiesToSpawn.Length)];

            spawnedEnemies.Add(Instantiate(randomEnemy, enemyPosition, Quaternion.identity));
        }

        PlayAnimation("boss idle");
    }

    public void Die()
    {
        if (Controller.main == null)
            return;

        Controller.main.KilledEnemy(GetEnemyType());
        inFight = false;
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

    public void OnStatueDestroyed()
    {
        print("Statue destroyed");
        PlayAnimation("boss exposed");
        vulnerable = true;
    }

    private void PlayAnimation(string action)
    {
        string animation = $"BOSS1_animation_{action}";

        animator.Play(animation);
    }

    public int Hurt(int damage)
    {
        if (vulnerable)
        {
            damageTakenThisRound += damage;
            PlayAnimation("damage");
            if (damageTakenThisRound >= damagePerRound)
            {
                damageTakenThisRound = 0;
                print("Took enough daamge to end round");
                if (statues.Count == 0)
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
        return damageTakenThisRound;
    }

    public void Attack() => SpawnStatue();

    public int Heal(int amount)
    {
        return -1;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 0));
    }
}
