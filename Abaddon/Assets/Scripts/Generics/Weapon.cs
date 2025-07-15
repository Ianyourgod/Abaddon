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
    public static int baseDamage = 2;
    public static Vector2 baseSize = new Vector2(1f, 1f); // length, width

    public abstract Vector2 GetSize();
    public abstract int GetDamage();

    void Awake()
    {
        hinderAttacksLayerMask = 1 << LayerMask.NameToLayer("Obstructions");
    }

    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static Weapon GetDefaultWeapon()
    {
        if (defaultWeapon == null)
        {
            if (Controller.main == null)
                return new Sword(); // unity yells at us for this but it's better than null reference exceptions

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

    private static float tempDebugOrientation = 0f;
    private static Vector2 tempDebugPosition = Vector2.zero;

    public CanBeDamaged[] GetFightablesInDamageArea(Vector2 position, float orientation)
    {
        // orientation is in radians
        tempDebugOrientation = orientation;
        tempDebugPosition = position;
        // Debug.Log(new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)));
        // Debug.Log(
        //     $"{new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)).magnitude} at {orientation} radians"
        // );
        Vector2 rotatedBox = rotate(GetSize() * 0.85f, orientation); // 0.85 is to stop the tiles from overflowing into neighboring tiles

        // if you're looking at this
        // please note that the center offset doesn't work completely but it does enough to hit the enemies intended
        // up to 2 enemies long, at least
        // the getsize() x is the length of the weapon, and the y is the width
        Vector2 centerOffset = new Vector2(
            Mathf.Cos(orientation) * (GetSize().x + 1) * 0.5f,
            Mathf.Sin(orientation) * (GetSize().x + 1) * 0.5f
        );
        Vector2 boxCenter = position + centerOffset;
        Collider2D[] colliders = Physics2D
            .OverlapBoxAll(boxCenter, rotatedBox, 0f)
            .ToList()
            // Loop over all hits and run a raycast on them to see if an obstructable object is in the way.
            .Where(collider =>
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    position,
                    new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)),
                    GetSize().x + 1,
                    hinderAttacksLayerMask
                );
                // Check if the collider either has nothing infront of it that can block the attack or if the object itself is a wall type
                return hit.collider == null || hit.collider == collider;
            })
            .ToArray();
        ;

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 centerOffset = new Vector2(
            Mathf.Cos(tempDebugOrientation) * (GetSize().x + 1) * 0.5f,
            Mathf.Sin(tempDebugOrientation) * (GetSize().x + 1) * 0.5f
        );
        Vector3 boxCenter = tempDebugPosition + centerOffset;

        // Draw rotated wire cube using matrix transformation
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            boxCenter,
            Quaternion.Euler(0, 0, tempDebugOrientation * Mathf.Rad2Deg),
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, GetSize() * 0.85f);
        Gizmos.matrix = oldMatrix;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            tempDebugPosition,
            tempDebugPosition
                + new Vector2(Mathf.Cos(tempDebugOrientation), Mathf.Sin(tempDebugOrientation))
        );
    }

    // Retrusn true if attack was successful, false otherwise
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
