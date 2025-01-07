using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    [SerializeField] GameObject statuePrefab;

    [HideInInspector] public bool inFight = false;
    [HideInInspector] public int stage = 0;

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

    public void Die() {
        inFight = false;
        stage = 0;
        Debug.Log("i am 1ssoB, and i hate (but im also dead so)");
    }
}
