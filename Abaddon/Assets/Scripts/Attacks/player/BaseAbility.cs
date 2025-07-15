using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour
{
    public bool enbaled = false;

    public virtual bool CanUse(CanFight fightable, Vector2 direction)
    {
        if (!enbaled)
            return false;

        if (fightable == null)
            return false;

        return true;
    }

    public virtual void Attack(
        CanFight fightable,
        Vector2 direction,
        Animator animator,
        PlayerSfx sfxPlayer
    )
    {
        // rahh im attacking!!
        print($"attacking BASE {fightable.GetType().Name} with {GetType().Name}");
    }
}
