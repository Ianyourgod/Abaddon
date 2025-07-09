using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class Sword : Weapon
{
    public new Vector2 size = new Vector2(1f, 1f);

    public override float GetAttackSpeed()
    {
        return Weapon.baseAttackSpeed;
    }

    public override uint GetDamage()
    {
        return Weapon.baseDamage + Controller.main.GetDamageModifier();
    }
}