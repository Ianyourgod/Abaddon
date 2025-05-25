using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void Interact();
}

public interface Hurtable
{
    bool Hurt(float damage);
    void Die();
}

public interface Fightable
{
    void Attack(uint damage);
    bool TakeDamage(uint damage);
    void Die();
}

// public class DamageTaker : MonoBehaviour
// {
//     [SerializeField] GameObject textFadePrefab;
//     public virtual bool TakeDamage(uint damage)
//     {
//         // ahhh im taking damage!!

//         GameObject damageAmount = Instantiate(textFadePrefab, transform.position + new Vector3(Random.Range(1, 5) / 10, Random.Range(1, 5) / 10, 0), Quaternion.identity);
//         damageAmount

//         return true;
//     }
// }