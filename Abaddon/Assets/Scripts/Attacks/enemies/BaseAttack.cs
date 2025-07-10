using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    [SerializeField]
    public uint damage = 1;

    [HideInInspector]
    public EnemySfx sfxPlayer;

    private void Awake()
    {
        sfxPlayer = GetComponent<EnemySfx>();
    }

    public virtual bool WillAttack(RaycastHit2D hit, Vector2 direction)
    {
        // we just check if the collider is the player, and if it is, we return true - direction is for if children of this need it
        if (hit.collider == null)
            return false;

        return hit.collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }

    public virtual void Attack(Vector2 direction)
    {
        if (sfxPlayer.playAttackOnDamagePlayer)
        {
            sfxPlayer.PlayAttackSound();
        }
        Controller.main.DamagePlayer(damage);
    }
}
