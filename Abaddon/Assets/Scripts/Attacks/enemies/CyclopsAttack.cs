using Unity.VisualScripting;
using UnityEngine;

public class CyclopsAttack : BaseAttack
{
    public enum AttackStage
    {
        WindUp,
        Attack,
    }

    [SerializeField]
    public AttackStage attackStage = AttackStage.Attack;

    public override bool WillAttack(Vector2 position, RaycastHit2D[] hits, Vector2 direction)
    {
        // we check in a radius for the cyclops, don't care about raycast hits
        Collider2D[] h = Physics2D.OverlapBoxAll(
            position,
            new Vector2(1f, 1f),
            0f,
            LayerMask.GetMask("Player")
        );
        // mask only returns player
        return h.Length > 0;
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
                if (WillAttack(transform.position, null, direction))
                {
                    base.Attack(direction);
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
