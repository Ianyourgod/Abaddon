using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : DamageTaker
{
    [SerializeField] GameObject statuePrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] int AllowedAttackTicks = 7;
    [SerializeField] int Stages = 3;
    [SerializeField] int statueRange = 5;

    [HideInInspector] public bool inFight = false;
    [HideInInspector] public int stage = 0;

    private int health = 10;
    private int ticks_till_move_back = 0;

    void Awake() {
        Controller.OnTick += CustomUpdate;
    }

    public void StartFight() {
        inFight = true;
        stage = 1;
        Debug.Log("i am 1ssoB, and i hate");
        SpawnStatue();
    }

    private void SpawnStatue() {
        // generate statue position
        Vector2 bossPosition = transform.position;
        // man i hate unity
        float range = Random.Range(2, statueRange);
        Vector2 random_position = Random.insideUnitCircle.normalized * range;
        Vector2 statuePosition = new Vector2(Mathf.Round(random_position.x), Mathf.Round(random_position.y)) + bossPosition;

        GameObject statue = Instantiate(statuePrefab, statuePosition, Quaternion.identity);

        statue.GetComponent<Statue>().boss = this;
    }

    public void StatueDestroyed() {
        stage++;
        Debug.Log("i am 1ssoB, and i hate (but i also love 👅) and im on stage " + stage);
        if (stage > Stages*2) {
            Die();
        }

        if (stage % 2 == 1) {
            SpawnStatue();
        } else {
            // allow boss to be damaged
            health = 15;
            ticks_till_move_back = AllowedAttackTicks;
        }
    }

    public void Die() {
        inFight = false;
        stage = 0;
        Debug.Log("i am 1ssoB, and i hate (but im also dead so)");
        PlayAnimation("die");
        // set color to dark red
        spriteRenderer.color = new Color(0.5f, 0, 0);

        StartCoroutine(FullyDie(2));
    }

    IEnumerator FullyDie(float wait_time) {
        yield return new WaitForSeconds(wait_time);
        Destroy(gameObject);
    }

    private void CustomUpdate() {
        if (stage % 2 == 0 && stage != 0) {
            ticks_till_move_back--;
            Debug.Log("uh oh im running out of ticks! " + ticks_till_move_back);
            if (ticks_till_move_back <= 0) {
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

    public override bool TakeDamage(uint damage) {
        if (inFight && stage % 2 == 0) {
            health -= (int) damage;
            Debug.Log("i am 1ssoB, and i hate (but i also love 👅) and im taking damage (" + damage + ", " + health + ")");
            PlayAnimation("damage");
            if (health <= 0) {
                health = 0;
                stage++;
                Debug.Log("i am 1ssoB, and i am dead but maybe not potentially stage is now " + stage);
                if (stage > Stages*2) {
                    Die();
                } else {
                    SpawnStatue();
                }
            }
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, statueRange);
    }
}
