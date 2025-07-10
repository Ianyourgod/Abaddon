using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoorSfx))]
public class Door : MonoBehaviour, CanBeInteractedWith
{
    [SerializeField]
    public bool NeedsKey;

    [Tooltip(
        "Optional ID for the key to unlock the door. If it's less than 0, it doesn't need a special key."
    )]
    public int specialKeyID = -1;

    [SerializeField]
    GameObject keyPrefab;

    [SerializeField]
    GameObject lockPrefab;
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
                if (specialKeyID >= 0)
                {
                    Item[] keysInInventory = Controller.main.inventory.GetItemsByID(key_ID);
                    bool hasSpecialKey = false;
                    foreach (Item key in keysInInventory)
                    {
                        if (
                            key.TryGetComponent(out Key keyComponent)
                            && keyComponent.isSpecialKey()
                            && keyComponent.GetKeyID() == specialKeyID
                        )
                        {
                            hasSpecialKey = true;
                            break;
                        }
                    }
                    if (!hasSpecialKey)
                    {
                        sfxPlayer.PlayLockedSound();
                        Instantiate(lockPrefab, transform.position, Quaternion.identity);
                        return;
                    }
                }
                Controller.main.inventory.RemoveItemAmount(key_ID, 1);
                Instantiate(keyPrefab, transform.position, Quaternion.identity);
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
