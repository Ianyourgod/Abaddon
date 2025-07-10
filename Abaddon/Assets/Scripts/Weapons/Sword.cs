using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class Sword : Weapon
{
    public static new int baseDamage = 3;
    public static new Vector2 baseSize = new Vector2(1f, 1f); // one wide, one long

    public override Vector2 GetSize()
    {
        return baseSize;
    }

    public override int GetDamage()
    {
        return baseDamage + Controller.main.GetDamageModifier();
    }
}