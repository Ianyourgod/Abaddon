using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public abstract class Weapon
    {
    public static float baseAttackSpeed = 1f;
    public static uint baseDamage = 15;

    private float lastAttackTime;
    public Vector2 damageBoxHalfExtents;

    public abstract float GetAttackSpeed();
    public abstract uint GetDamage();

    public CanFight[] GetFightablesInDamageArea(Vector2 position, float orientation)
    {
        Vector2 boxCenter = position + new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)) * 0.5f;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, damageBoxHalfExtents * 2, 0f);
        List<CanFight> fightables = new List<CanFight>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out CanFight fightable))
            {
                fightables.Add(fightable);
            }
        }
        return fightables.ToArray();
    }
    public bool AttackEnemies(CanFight[] enemies)
    {
        if (Time.time - lastAttackTime < GetAttackSpeed())
        {
            return false; // Not enough time has passed to attack again
        }
        lastAttackTime = Time.time; // Update the last attack time
        foreach (CanFight enemy in enemies)
        {
            enemy.Hurt(GetDamage());
        }
        return true; // Attack was successful
    }
}