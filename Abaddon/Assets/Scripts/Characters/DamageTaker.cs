using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour {
    [SerializeField] GameObject textFadePrefab;
    public virtual bool TakeDamage(uint damage) {
        // ahhh im taking damage!!

        GameObject damageAmount = Instantiate(textFadePrefab, transform.position + new Vector3(Random.Range(1, 5) / 10, Random.Range(1, 5) / 10, 0), Quaternion.identity);
        damageAmount.GetComponent<RealTextFadeUp>().SetText(damage.ToString(), Color.red, Color.white, 0.4f);

        return true;
    }
}