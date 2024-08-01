using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoorSfx))]

public class Door : MonoBehaviour
{
    enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] public bool NeedsKey;
    [SerializeField] Direction direction = Direction.Left;
    [SerializeField] Inventory inventory;
    [HideInInspector] public DoorSfx sfxPlayer;

    private SpriteRenderer spriteRenderer;

    void Awake() {
        sfxPlayer = GetComponent<DoorSfx>();
    }
}
