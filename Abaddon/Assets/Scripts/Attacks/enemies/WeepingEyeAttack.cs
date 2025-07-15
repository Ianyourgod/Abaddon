using Unity.VisualScripting;
using UnityEngine;

public class WeepingEyeAttack : BaseAttack
{
    public enum AttackStage
    {
        WindUp,
        Attack,
    }

    [SerializeField]
    public AttackStage attackStage = AttackStage.Attack;

    public override bool WillAttack(Vector2 position, Vector2 direction)
    {
        // we check in a line for the weeping eye, but it's longer than the one provided
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, 3f);
        bool res = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                res = true;
            }
            else if (hit.collider.gameObject != this.gameObject)
            {
                res = false;
                return res;
            }
        }
        // mask only returns player
        return hits.Length > 0;
    }

    public override void Attack(Vector2 direction)
    {
        if (Controller.main == null)
            return;

        switch (attackStage)
        {
            case AttackStage.WindUp:
                enemyMovement.forceAttackNextTurn = true;
                attackStage = AttackStage.Attack;
                break;
            case AttackStage.Attack:
                if (WillAttack(transform.position, direction))
                {
                    if (WillAttack(transform.position, direction))
                    {
                        Controller.main.DamagePlayer(damage);
                    }
                }
                enemyMovement.forceAttackNextTurn = false;
                attackStage = AttackStage.WindUp;
                break;
        }

        if (sfxPlayer.playAttackOnDamagePlayer)
        {
            sfxPlayer.PlayAttackSound();
        }
    }

    public override string GetAttackAnimationName()
    {
        switch (attackStage)
        {
            case AttackStage.WindUp:
                return "windup";
            case AttackStage.Attack:
                return "attack";
            default:
                return "idle";
        }
    }
}
