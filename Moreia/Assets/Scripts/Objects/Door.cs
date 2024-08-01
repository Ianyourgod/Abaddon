using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoorSfx))]

public class Door : MonoBehaviour
{
    [SerializeField] public bool NeedsKey;
    [SerializeField] Inventory inventory;
    [HideInInspector] public DoorSfx sfxPlayer;

    private SpriteRenderer spriteRenderer;

    void Awake() {
        sfxPlayer = GetComponent<DoorSfx>();
    }
}
