// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public abstract class Living : MonoBehaviour, CanFight, HasHealth, CanMove
// {
//     [SerializeField] protected uint health = 100;
//     [SerializeField] protected uint maxHealth = 100;
//     [SerializeField] GameObject textFadePrefab;

//     protected abstract void _Hurt(uint damage, Vector2 direction);
//     public void Hurt(uint damage)
//     {
//         _Hurt(damage, Vector2.zero);
//         if (health <= 0) Die();
//         GameObject damageAmount = Instantiate(textFadePrefab, transform.position + new Vector3(Random.Range(1, 5) / 10, Random.Range(1, 5) / 10, 0), Quaternion.identity);
//         damageAmount.GetComponent<RealTextFadeUp>().SetText(damage.ToString(), Color.red, Color.white, 0.4f);
//     }

//     protected abstract void _Attack(uint damage);
//     public void Attack(uint damage)
//     {
//         _Attack(damage);

//         GameObject damageAmount = Instantiate(textFadePrefab, transform.position + new Vector3(Random.Range(1, 5) / 10, Random.Range(1, 5) / 10, 0), Quaternion.identity);
//         damageAmount.GetComponent<RealTextFadeUp>().SetText(damage.ToString(), Color.red, Color.white, 0.4f);
//     }

//     public virtual void Die()
//     {
//         Debug.Log("Die");
//     }

//     protected abstract void _Heal(uint amount);
//     public void Heal(uint amount)
//     {
//         _Heal(amount);
//     }

//     protected abstract void _Move(Vector2 direction);
//     public void Move(Vector2 direction)
//     {
//         _Move(direction);
//     }
// }
