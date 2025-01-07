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
        Debug.Log("i am 1ssoB, and i spawn statue");

        // generate statue position
        Vector2 bossPosition = transform.position;
        // man i hate unity
        Vector2 random_position = Random.insideUnitCircle.normalized * 3;
        // normalize to (integer + .5)
        Vector2 statuePosition = new Vector2(Mathf.Round(random_position.x), Mathf.Round(random_position.y)) + bossPosition;

        GameObject statue = Instantiate(statuePrefab, statuePosition, Quaternion.identity);

        statue.GetComponent<Statue>().boss = this;
    }
}
