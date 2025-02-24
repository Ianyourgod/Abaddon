using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : DamageTaker
{
    [SerializeField] GameObject statuePrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int AllowedAttackTicks = 7;
    [SerializeField] int Stages = 3;

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
        Vector2 random_position = Random.insideUnitCircle.normalized * 3;
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
        // set color to dark red
        spriteRenderer.color = new Color(0.5f, 0, 0);
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

    public override bool TakeDamage(uint damage) {
        if (inFight && stage % 2 == 0) {
            health -= (int) damage;
            Debug.Log("i am 1ssoB, and i hate (but i also love 👅) and im taking damage (" + damage + ", " + health + ")");
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
}
