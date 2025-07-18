using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    private LayerMask hinderAttacksLayerMask;
    public static Weapon defaultWeapon;
    public abstract string AnimationName { get; }
    public static Vector2 baseSize = new Vector2(1f, 1f); // length, width

    public abstract Vector2 GetSize();
    public abstract int GetDamage();

    void Awake()
    {
        hinderAttacksLayerMask = 1 << LayerMask.NameToLayer("Obstructions");
    }

    static Vector2 FlipVector(Vector2 v)
    {
        return new Vector2(v.y, v.x);
    }

    public static (Vector2, Vector2) Rotate(Vector2 v, Vector2 dir)
    {
        // Default dimensions
        Vector2 dimensions = v;

        bool horizontal = Mathf.Abs(dir.x) > 0.5f;
        float d = horizontal ? dir.x : dir.y;
        Vector2 offset = new Vector2(0, (0.5f + dimensions.y / 2) * d);

        if (horizontal)
        {
            dimensions = FlipVector(dimensions);
            offset = FlipVector(offset);
        }

        return (dimensions, offset);
    }

    public static Weapon GetDefaultWeapon()
    {
        if (defaultWeapon == null)
        {
            // don't instantiate like this
            // better to just take the hit and get a null reference exception
            // if (Controller.main == null)
            //     return new Sword(); // unity yells at us for this but it's better than null reference exceptions
            // // alternatively you could just do it correctly..........

            // Debug.Log("Default weapon is not set, setting to Sword.");
            defaultWeapon = Controller.main.GetComponent<Sword>();
        }
        return defaultWeapon;
    }

    public static Weapon GetCurrentWeapon()
    {
        if (Controller.main == null)
            return GetDefaultWeapon(); // Return default weapon if Controller is not available

        Slot slot = Controller.main.inventory.equipSlots[3];
        if (slot == null)
        {
            // Debug.LogWarning("No weapon slot found, returning default.");
            return GetDefaultWeapon(); // Default to Sword if no slot is found
        }
        if (slot.TryGetComponent(out EquipmentSlot equipmentSlot))
        {
            if (!equipmentSlot.isEquipped())
            {
                // Debug.Log("Weapon slot is not equipped, returning default.");
                return GetDefaultWeapon();
            }
            Item currentItem = slot.GetComponent<Slot>().slotsItem;
            if (currentItem.TryGetComponent(out Weapon weapon))
            {
                return weapon; // Return the weapon associated with the current item
            }
            // Debug.LogWarning("Current item does not have a weapon association, returning default.");
            return GetDefaultWeapon();
        }
        return GetDefaultWeapon();
    }

    public CanBeDamaged[] GetFightablesInDamageArea(Vector2 position, Vector2 direction)
    {
        print("hiii");

        (Vector2 _rotatedBox, Vector2 centerOffset) = Rotate(GetSize(), direction); // 0.85 is to stop the tiles from overflowing into neighboring tiles
        Vector2 rotatedBox = _rotatedBox * 0.85f;

        // Debug.Log(
        //     $"GetFightablesInDamageArea: position={position}, direction={direction}, rotatedBox={rotatedBox}, centerOffset={centerOffset}"
        // );

        // print(GetSize());
        // print(position);
        // print(direction);
        // print("NEXT");
        // print(rotatedBox);
        // print(centerOffset);

        Vector2 boxCenter = position + centerOffset;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, rotatedBox, 0f);

        List<CanBeDamaged> fightables = new List<CanBeDamaged>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out CanBeDamaged fightable))
            {
                fightables.Add(fightable);
            }
        }
        return fightables.ToArray();
    }

    // Returns true if attack was successful, false otherwise
    public bool AttackEnemies(CanBeDamaged[] enemies, Vector2 direction)
    {
        if (Controller.main == null)
            return false; // if controller is null we can assume that no enemies were attacked

        int calculatedDamage = GetDamage() + Controller.main.GetDamageModifier();

        // Debug.Log($"Attacking with damage: {calculatedDamage}");
        if (enemies.Length == 0)
            return false; // No enemies to attack

        Controller.main.animator.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID(
            "AttackerLayer"
        );
        foreach (CanBeDamaged enemy in enemies)
            enemy.Hurt(calculatedDamage);

        return true; // Attack was successful
    }
}
