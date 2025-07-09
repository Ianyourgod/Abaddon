using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System;

public abstract class Weapon : MonoBehaviour
{
    public static Weapon defaultWeapon;
    public static uint baseDamage = 2;
    public static Vector2 baseSize = new Vector2(1f, 1f);


    public abstract Vector2 GetSize();
    public abstract uint GetDamage();

    public static Weapon GetDefaultWeapon()
    {
        if (defaultWeapon == null)
        {
            Debug.Log("Default weapon is not set, setting to Sword.");
            defaultWeapon = Controller.main.GetComponent<Sword>();
        }
        return defaultWeapon;
    }

    public static Weapon GetCurrentWeapon()
    {
        Slot slot = Controller.main.inventory.equipSlots[3];
        if (slot == null)
        {
            Debug.LogWarning("No weapon slot found, returning default.");
            return GetDefaultWeapon(); // Default to Sword if no slot is found
        }
        if (slot.TryGetComponent(out EquipmentSlot equipmentSlot))
        {
            if (!equipmentSlot.isEquipped())
            {
                Debug.Log("Weapon slot is not equipped, returning default.");
                return GetDefaultWeapon();
            }
            Item currentItem = slot.GetComponent<Slot>().slotsItem;
            if (currentItem.TryGetComponent(out Weapon weapon))
            {
                return weapon; // Return the weapon associated with the current item
            }
            Debug.LogWarning("Current item does not have a weapon association, returning default.");
            return GetDefaultWeapon();
        }
        return GetDefaultWeapon();
    }

    private static float tempDebugOrientation = 0f;
    private static Vector2 tempDebugPosition = Vector2.zero;


    public CanFight[] GetFightablesInDamageArea(Vector2 position, float orientation)
    {
        // orientation is in radians
        tempDebugOrientation = orientation;
        tempDebugPosition = position;
        Debug.Log(new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)));
        Debug.Log($"{new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)).magnitude} at {orientation} radians");
        Vector2 boxCenter = position + new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, GetSize() * 0.85f, orientation);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rotatedSize = Quaternion.AngleAxis(tempDebugOrientation, Vector3.forward) * (GetSize() * 0.85f);
        Gizmos.DrawWireCube(tempDebugPosition + new Vector2(Mathf.Cos(tempDebugOrientation), Mathf.Sin(tempDebugOrientation)), rotatedSize);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(tempDebugPosition, tempDebugPosition + new Vector2(Mathf.Cos(tempDebugOrientation), Mathf.Sin(tempDebugOrientation)));
    }

    public bool AttackEnemies(CanFight[] enemies, Vector2 direction)
    {
        uint calculatedDamage = GetDamage() + Controller.main.GetDamageModifier();
        Debug.Log($"Attacking with damage: {calculatedDamage}");
        if (enemies.Length == 0)
        {
            return false; // No enemies to attack
        }
        if (enemies.Length == 0)
        {
            return false; // No enemies to attack
        }
        Controller.main.animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("AttackerLayer");
        Controller.main.sfxPlayer.PlayAttackSound();
        Controller.main.PlayAnimation("attack", direction);
        foreach (CanFight enemy in enemies)
        {
            enemy.Hurt(calculatedDamage);
        }
        return true; // Attack was successful
    }
}