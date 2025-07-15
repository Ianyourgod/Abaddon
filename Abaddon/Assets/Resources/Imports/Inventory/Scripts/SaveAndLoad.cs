///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    [Header("Manual Save & Load")]
    [Tooltip("Select a key that will save the game when clicked.")]
    public KeyCode SaveKey;

    [Tooltip("Select a key that will load the game when clicked.")]
    public KeyCode LoadKey;

    [Header("Auto Save & Load")]
    [Tooltip("Check this if you want your game to save itself automaticly.")]
    public bool autoSave;

    [Tooltip("Check this if you want your game to load itself automaticly on startup.")]
    public bool autoLoad;

    [Tooltip(
        "Select the time period in seconds between multiple saves. It is recommended to use something bigger (eg. 2 minutes) but not necessary."
    )]
    public float timeBetweenSaves;

    Inventory inv;

    [HideInInspector]
    public List<Item> items = new List<Item>();

    [HideInInspector]
    bool autoSaved;

    private void Start()
    {
        inv = GetComponent<Inventory>();
        Item[] tempArray = null;
        tempArray = Object.FindObjectsOfType<Item>();
        items = tempArray.OrderBy(e => e.ItemID).ToList();
        items = items.GroupBy(x => x.ItemID).Select(x => x.First()).ToList();
        if (autoLoad)
            StartCoroutine(AutoLoad(0.2f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(SaveKey))
            Save();
        else if (Input.GetKeyDown(LoadKey))
            Load();

        if (autoSave && !autoSaved)
            StartCoroutine(AutoSave(timeBetweenSaves));
    }

    void Save()
    {
        int items = 0;
        for (int i = 0; i < inv.slots.Length; i++)
        {
            if (inv.slots[i].slotsItem)
                items++;
        }
        List<Slot> allSlots = new List<Slot>();

        foreach (Slot slot in inv.slots)
            allSlots.Add(slot);
        foreach (Slot slot in inv.equipSlots)
            allSlots.Add(slot);

        Slot[] readySlots = allSlots.ToArray();

        if (items >= 1)
        {
            List<ItemLoad> itemsToLoad = new List<ItemLoad>();
            for (int i = 0; i < readySlots.Length; i++)
            {
                Slot z = readySlots[i];
                if (z.slotsItem)
                {
                    ItemLoad h = new ItemLoad(z.slotsItem.ItemID, z.slotsItem.amountInStack, i);
                    itemsToLoad.Add(h);
                }
            }

            string json = CustomJSON.ToJson(itemsToLoad);
            File.WriteAllText(Application.persistentDataPath + transform.name, json);

            print("Saving...");
        }
        else
            print("There are no items in your inventory!");
    }

    void Load()
    {
        List<ItemLoad> itemsToLoad = CustomJSON.FromJson<ItemLoad>(
            File.ReadAllText(Application.persistentDataPath + transform.name)
        );

        List<Slot> allSlots = new List<Slot>();

        foreach (Slot slot in inv.slots)
            allSlots.Add(slot);
        foreach (Slot slot in inv.equipSlots)
            allSlots.Add(slot);

        Slot[] readySlots = allSlots.ToArray();

        if (itemsToLoad != null)
        {
            if (itemsToLoad.Count > 0)
            {
                print("Loading...");
                for (int i = itemsToLoad[0].slotIndex; i < readySlots.Length; i++)
                {
                    foreach (ItemLoad z in itemsToLoad)
                    {
                        if (i == z.slotIndex)
                        {
                            Item b = Instantiate(items[z.id - 1], allSlots[i].transform)
                                .GetComponent<Item>();
                            b.amountInStack = z.amount;
                            break;
                        }
                    }
                }
                StartCoroutine(Refresh(0.0000001f));
            }
            else
                print("There aren't any save files found!");
        }
        else
            print("There aren't any save files found!");
    }

    IEnumerator AutoSave(float time)
    {
        autoSaved = true;
        yield return new WaitForSeconds(time);
        Save();
        autoSaved = false;
    }

    IEnumerator AutoLoad(float time)
    {
        yield return new WaitForSeconds(time);
        Load();
    }

    IEnumerator Refresh(float time)
    {
        inv.inventoryObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        inv.inventoryObject.gameObject.SetActive(false);
    }

    public void ResetSaveFiles()
    {
        List<ItemLoad> itemsToLoad = new List<ItemLoad>();

        string json = CustomJSON.ToJson(itemsToLoad);
        File.WriteAllText(Application.persistentDataPath + transform.name, json);
    }
}

[System.Serializable]
public class ItemLoad
{
    public int id,
        amount,
        slotIndex;

    public ItemLoad(int ID, int AMOUNT, int SLOTINDEX)
    {
        id = ID;
        amount = AMOUNT;
        slotIndex = SLOTINDEX;
    }
}
