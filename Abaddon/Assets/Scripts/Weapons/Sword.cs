using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class Sword : Weapon
{
    public override Vector2 GetSize()
    {
        return baseSize;
    }

    // public override float GetAttackSpeed()
    // {
    //     return baseAttackSpeed;
    // }

    public override uint GetDamage()
    {
        return baseDamage + Controller.main.GetDamageModifier();
    }
}