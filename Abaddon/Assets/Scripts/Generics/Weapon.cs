using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System;

[System.Serializable]
public abstract class Weapon
{
    public static float baseAttackSpeed = 1f;
    public static uint baseDamage = 2;

    private float lastAttackTime;
    public Vector2 size;

    public abstract float GetAttackSpeed();
    public abstract uint GetDamage();

    public static Weapon GetCurrentWeapon()
    {
        Slot slot = Controller.main.inventory.equipSlots[3];
        if (slot == null)
        {
            Debug.LogWarning("No weapon slot found, returning default Sword.");
            return new Sword(); // Default to Sword if no slot is found
        }
        if (slot.TryGetComponent(out EquipmentSlot equipmentSlot))
        {
            if (!equipmentSlot.isEquipped())
            {
                Debug.LogWarning("Weapon slot is not equipped, returning default Sword.");
                return new Sword(); // Default to Sword if not equipped
            }
            Item currentItem = slot.GetComponent<Slot>().slotsItem;
            if (currentItem.TryGetComponent(out ItemWeaponAssociation weaponAssociation))
            {
                return weaponAssociation.GetWeaponType();
            }
            Debug.LogWarning("Current item does not have a weapon association, returning default Sword.");
            return new Sword(); // Default to Sword if no weapon association is found
        }
        return new Sword(); // Default to Sword if no weapon is equipped
    }

    public CanFight[] GetFightablesInDamageArea(Vector2 position, float orientation)
    {
        Vector2 boxCenter = position + new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)) * 0.5f;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, size * 2, 0f);
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
    public bool AttackEnemies(CanFight[] enemies, Vector2 direction)
    {
        uint calculatedDamage = GetDamage() + Controller.main.GetDamageModifier();
        Debug.Log($"Attacking with damage: {calculatedDamage}");
        if (Time.time - lastAttackTime < GetAttackSpeed())
        {
            return false; // Not enough time has passed to attack again
        }
        if (enemies.Length == 0)
        {
            return false; // No enemies to attack
        }
        lastAttackTime = Time.time; // Update the last attack time
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