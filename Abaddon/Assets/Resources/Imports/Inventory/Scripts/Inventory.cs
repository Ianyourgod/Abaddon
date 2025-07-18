///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SaveAndLoad))]
public class Inventory : MonoBehaviour
{
    AudioClip openingSfx;
    AudioClip closingSfx;

    [Header("Inventory Settings")]
    [Tooltip("Put your whole inventory object inside here.")]
    public GameObject inventoryObject;

    [Tooltip("Assign your inventory object inside here.")]
    public GameObject equipmentObject;

    [Tooltip("equiptment object.")]
    public GameObject statObject;

    [Tooltip("stat object.")]
    public GameObject craftObject;

    [Tooltip("crafting :scream:")]
    public GameObject levelUpObject;

    [Tooltip("level up object.")]
    public GameObject shadow;

    [Tooltip(
        "Assign the game object thas is a parent for each of your hotbar slots. NOTE: It has to have the \"HotbarParent\" script attached to it."
    )]
    public HotbarParent hotbarParent;

    [Tooltip("Select a font for the amount displays.")]
    public TMP_FontAsset font;

    [Header("Forces")]
    [Tooltip(
        "Assign an empty game object it's position will be used as the point from where items will be dropped when thrown out of the inventory."
    )]
    public Transform throwPosition;

    [Tooltip(
        "This is distance from which the player can pick items. The red wire sphere displays the maximum distance in the editor window."
    )]
    public float pickupRange;

    [Tooltip("This is the upward force that will be applied to the item when throwing it.")]
    public float throwForceUp;

    [Tooltip("This is the forward force that will be applied to the item when throwing it.")]
    public float throwForceForward;

    [Tooltip(
        "Select your movement script of choice. It could be our built-in FPS controller or any other."
    )]
    public Controller playerMovement;

    [Header("Slots")]
    [Tooltip(
        "Drag all of your slots in here. NOTE: They HAVE to be in ascending order or else the items will not go to the first empty slot but in a random one!"
    )]
    public Slot[] slots;

    [Tooltip(
        "Drag all of your equipment slots in here. NOTE: They HAVE to be in ascending order or else the items will not go to the first empty slot but in a random one!"
    )]
    public Slot[] equipSlots;

    [Tooltip("no tooltip for you")]
    public StatInfoUpdater[] statValues;

    [HideInInspector]
    public List<Slot> emptySlots;

    [HideInInspector]
    public bool readyToAdd = true;

    private UISfx invSfxPlayer;

    private enum Tab
    {
        Inventory,
        LevelUp,
    }

    private Tab current_tab = Tab.Inventory;
    public bool isInventoryOpen = false;

    private void Awake()
    {
        invSfxPlayer = inventoryObject.transform.parent.GetComponent<UISfx>();
    }

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].slotsItem && !slots[i].gameObject.GetComponent<EquipmentSlot>())
                emptySlots.Add(slots[i]);
        }

        foreach (Slot slot in slots)
        {
            if (slot)
                slot.CustomStart();
        }

        foreach (Slot slot in equipSlots)
        {
            if (slot)
                slot.CustomStart();
        }

        CloseAllTabs();
    }

    private void Update()
    {
        if (UIStateManager.singleton == null)
            return;
        if (UIStateManager.singleton.mostRecentState == UIState.Pause)
            return;
        if (Controller.main != null)
            Controller.main.UIFloatText();

        if (Input.GetKeyDown(KeyCode.Tab))
        { // TODO: make this a rebindable key
            UIStateManager.singleton.ToggleUIPage(UIState.Inventory);
            Controller.main.hasOpenedInventory = true;
        }

        foreach (Slot i in slots)
        {
            if (i)
                i.CheckForItem();
        }

        foreach (Slot i in equipSlots)
        {
            i.CheckForItem();
        }
    }

    public bool AddItemAtIndex(Item itemToAdd, int index, bool isEquipmentSlot = false)
    {
        // Pick which slot array to use
        Slot[] slotArray = isEquipmentSlot ? equipSlots : slots;

        if (index < 0 || index >= slotArray.Length)
        {
            Debug.LogWarning(
                $"AddItemAtIndex: Index {index} is out of range for {(isEquipmentSlot ? "equipSlots" : "slots")}"
            );
            return false;
        }

        Slot targetSlot = slotArray[index];

        bool wasEmpty = (targetSlot.slotsItem == null);

        // If there's already an item there, destroy or drop it if you want — up to you
        if (!wasEmpty)
        {
            Debug.LogWarning(
                $"AddItemAtIndex: Slot {index} already has an item. It will be replaced."
            );
            Destroy(targetSlot.slotsItem.gameObject);
        }

        // Actually parent the item to this slot
        itemToAdd.transform.SetParent(targetSlot.transform, false);
        itemToAdd.transform.localPosition = Vector3.zero;
        itemToAdd.gameObject.SetActive(true);

        // Update the slot's reference
        targetSlot.slotsItem = itemToAdd;

        // If you want to remove it from emptySlots list:
        emptySlots.Remove(targetSlot);

        return wasEmpty;
    }

    const float FADE_IN_TIME = 10f; // the higher, the faster. multed by dt so doesn't need to be an exact science.
    const float DARK_VAL = 0.75f; // how dark you want it.

    public void OpenInventory()
    {
        invSfxPlayer.PlayOpenSound(); //play the open sound effect on the parent of the inventory object (the inventory ui)

        isInventoryOpen = true;

        ReloadInventory();

        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;

        UIStateManager.singleton.FadeInDarkener(FADE_IN_TIME, DARK_VAL);

        playerMovement.enabled = false;
        if (shadow)
            shadow.SetActive(true);
    }

    public void CloseInventory()
    {
        invSfxPlayer.PlayCloseSound(); //play the close sound on the parent of the inventory object (the inventory ui)

        CloseAllTabs();

        UIStateManager.singleton.FadeOutDarkener(FADE_IN_TIME, DARK_VAL);

        isInventoryOpen = false;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        playerMovement.enabled = true;
        if (shadow)
            shadow.SetActive(false);
    }

    public bool CheckIfItemExists(int id)
    {
        // more efficient !!!!!!
        foreach (Slot slot in slots)
        {
            if (slot.slotsItem)
            {
                Item z = slot.slotsItem;
                if (z.ItemID == id)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Item[] GetItemsByID(int id)
    {
        List<Item> items = new List<Item>();
        foreach (Slot slot in slots)
        {
            if (slot.slotsItem)
            {
                Item z = slot.slotsItem;
                if (z.ItemID == id)
                {
                    items.Add(z);
                }
            }
        }
        return items.ToArray();
    }

    public void SwapTuahTheStats()
    {
        current_tab = Tab.LevelUp;
        ReloadInventory();
    }

    public void ClearInventory()
    {
        foreach (Slot i in slots.Union(equipSlots))
        {
            if (!i)
                continue;

            Destroy(i.slotsItem?.gameObject);
            i.slotsItem = null;
        }
    }

    public void SwapToInventory()
    {
        current_tab = Tab.Inventory;
        ReloadInventory();
    }

    public void CloseAllTabs()
    {
        inventoryObject.SetActive(false);
        equipmentObject.SetActive(false);
        statObject.SetActive(false);
        craftObject.SetActive(false);
    }

    public void ReloadInventory()
    {
        CloseAllTabs();

        switch (current_tab)
        {
            case Tab.Inventory:
                inventoryObject.SetActive(true);
                // print(equipmentObject);
                equipmentObject.SetActive(true);
                statObject.SetActive(true);
                craftObject.SetActive(true);
                break;
            case Tab.LevelUp:
                levelUpObject.SetActive(true);
                break;
        }
    }

    public int GetItemAmount(int id)
    {
        int num = 0;
        foreach (Slot slot in slots)
        {
            if (slot.slotsItem)
            {
                Item z = slot.slotsItem;
                if (z.ItemID == id)
                {
                    num += z.amountInStack;
                }
            }
        }
        return num;
    }

    public void RemoveItemAmount(int id, int amountToRemove)
    {
        foreach (Slot i in slots)
        {
            if (amountToRemove <= 0)
            {
                return;
            }

            if (i.slotsItem)
            {
                Item z = i.slotsItem;
                if (z.ItemID == id)
                {
                    int amountThatCanBeRemoved = z.amountInStack;
                    if (amountThatCanBeRemoved <= amountToRemove)
                    {
                        Destroy(z.gameObject);
                        amountToRemove -= amountThatCanBeRemoved;
                    }
                    else
                    {
                        z.amountInStack -= amountToRemove;
                        amountToRemove = 0;
                    }
                }
            }
            // not using a hotbar
            // hotbarParent.SelectItem();
        }
    }

    public void RemoveByID(int id)
    {
        foreach (Slot i in slots)
        {
            if (i.slotsItem)
            {
                Item z = i.slotsItem;
                if (z.ItemID == id)
                {
                    Destroy(z.gameObject);
                }
            }
        }
    }

    public void AddItems(List<Item> itemsToBeAdded)
    {
        IEnumerator waiter()
        {
            foreach (Item i in itemsToBeAdded)
            {
                AddItem(i);
                print($"Added item: {i.name} | {emptySlots.Count}/{slots.Length} empty slots left");
                yield return new WaitForSeconds(0.003f);
            }
        }
        StartCoroutine(waiter());
    }

    public void AddItem(Item itemToBeAdded, Item startingItem = null)
    {
        if (readyToAdd)
        {
            int amountInStack = itemToBeAdded.amountInStack;
            List<Item> stackableItems = new List<Item>();
            List<Slot> empty = new List<Slot>();
            emptySlots = empty;

            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].slotsItem && !slots[i].gameObject.GetComponent<EquipmentSlot>())
                    empty.Add(slots[i]);
            }

            if (
                startingItem
                && startingItem.ItemID == itemToBeAdded.ItemID
                && startingItem.amountInStack < startingItem.maxStackSize
            )
            {
                stackableItems.Add(startingItem);
            }

            foreach (Slot slot in slots)
            {
                if (slot.slotsItem)
                {
                    Item itemOfCurSlot = slot.slotsItem;
                    if (
                        itemOfCurSlot.ItemID == itemToBeAdded.ItemID
                        && itemOfCurSlot.amountInStack < itemOfCurSlot.maxStackSize
                        && itemOfCurSlot != startingItem
                    )
                    {
                        stackableItems.Add(itemOfCurSlot);
                    }
                }
            }

            foreach (Item i in stackableItems)
            {
                int amountThatCanBeAdded = i.maxStackSize - i.amountInStack;
                if (amountThatCanBeAdded > i.maxStackSize)
                {
                    i.amountInStack = i.maxStackSize;
                    amountInStack -= amountThatCanBeAdded;
                }
                else if (amountThatCanBeAdded <= i.maxStackSize)
                {
                    i.amountInStack += amountInStack;
                    if (i.amountInStack > i.maxStackSize)
                    {
                        i.transform.parent.gameObject.GetComponent<Slot>().slotsItem = null;
                        i.transform.parent = null;
                        for (int z = 0; z < slots.Length; z++)
                        {
                            if (
                                !slots[z].slotsItem
                                && !slots[z].gameObject.GetComponent<EquipmentSlot>()
                            )
                                empty.Add(slots[z]);
                        }
                        AddItem(i);
                    }

                    Destroy(itemToBeAdded.gameObject);
                    return;
                }
            }

            itemToBeAdded.amountInStack = amountInStack;

            foreach (Slot i in empty)
            {
                if (itemToBeAdded.amountInStack > itemToBeAdded.maxStackSize)
                {
                    Item newItem = Instantiate(itemToBeAdded.gameObject).GetComponent<Item>();
                    newItem.amountInStack = newItem.maxStackSize;
                    itemToBeAdded.amountInStack -= newItem.maxStackSize;
                    newItem.transform.parent = i.transform;
                    newItem.gameObject.SetActive(false);
                }
                else
                {
                    itemToBeAdded.transform.parent = i.transform;
                    itemToBeAdded.transform.gameObject.SetActive(false);
                    break;
                }
            }

            StartCoroutine(ItemWait());
        }
    }

    IEnumerator ItemWait()
    {
        readyToAdd = false;
        yield return new WaitForSeconds(0.001f);
        readyToAdd = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
