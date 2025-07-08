using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoorSfx))]

public class Door : MonoBehaviour, CanBeInteractedWith
{
    [SerializeField] public bool NeedsKey;
    [SerializeField] GameObject lockPrefab;
    DoorSfx sfxPlayer;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        sfxPlayer = GetComponent<DoorSfx>();
    }

    // damage is unused
    public void Interact()
    {
        const int key_ID = 1;

        bool hasKey = Controller.main.inventory.CheckIfItemExists(key_ID);
        if ((NeedsKey && hasKey) || !NeedsKey)
        {
            if (NeedsKey)
            {
                Controller.main.inventory.RemoveByID(key_ID);
                sfxPlayer.PlayUnlockLockedSound();
            }
            else
            {
                sfxPlayer.PlayUnlockedSound();
            }

            Destroy(gameObject);
        }
        else
        {
            sfxPlayer.PlayLockedSound();
            Instantiate(lockPrefab, transform.position, Quaternion.identity);
        }
    }
}
