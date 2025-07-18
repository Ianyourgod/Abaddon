using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemDropper : MonoBehaviour
{
    [SerializeField]
    public DropTableEntry[] dropTable;

    [SerializeField]
    public GameObject goldCoinPrefab;

    [SerializeField, Tooltip("Put both to zero to stop gold dropping")]
    public int minGoldDropAmount = 1; // Minimum amount of gold to drop

    [SerializeField, Tooltip("Put both to zero to stop gold dropping")]
    public int maxGoldDropAmount = 1; // Minimum amount of gold to drop
    private static float DECAY_RATE = 0.1f; // Rate at which the drop chance decays per same item in the players inventory

    public void ForceDrop(GameObject item)
    {
        if (item == null)
            return;

        // Instantiate the item at the dropper's position and rotation
        GameObject droppedItem = Instantiate(item, transform.position, Quaternion.identity);

        // Optionally, you can set the parent of the dropped item to the dropper
        droppedItem.transform.SetParent(transform);
    }

    public void UpdateDropChance(Item item, float chance)
    {
        for (int i = 0; i < dropTable.Length; i++)
        {
            if (dropTable[i].item == item)
            {
                dropTable[i].chance = chance;
                return;
            }
        }

        // If the item is not found, add it to the drop table
        DropTableEntry newEntry = new DropTableEntry(item, chance);
        List<DropTableEntry> updatedDropTable = dropTable.ToList();
        updatedDropTable.Add(newEntry);
        dropTable = updatedDropTable.ToArray();
    }

    private float DecayedChance(DropTableEntry entry)
    {
        if (entry.item == null)
            return entry.chance;

        // Take the base chance and subtract the decay based on the decay rate and # of those items in the player's inventory (with a minimum of 0)
        return Mathf.Max(
            entry.chance - Controller.main.inventory.GetItemAmount(entry.item.ItemID) * DECAY_RATE,
            0
        );
    }

#nullable enable
    public Item? GetRandomItem()
    {
        float maxValue = dropTable.Select(entry => DecayedChance(entry)).Sum();
        float randomValue = Random.Range(0f, maxValue);
        float cumulativeChance = 0f;

        foreach (var entry in dropTable)
        {
            cumulativeChance += DecayedChance(entry);
            if (randomValue <= cumulativeChance)
            {
                return entry.item;
            }
        }

        return null;
    }

    public void DropRandomItem()
    {
        // Drop a random item from the drop table
        print("dropping items");
        var item = GetRandomItem();
        if (item)
            Instantiate(item, transform.position, Quaternion.identity);

        // Drop a random amount of gold
        if (goldCoinPrefab == null || minGoldDropAmount <= 0 || maxGoldDropAmount <= 0)
        {
            print("No gold to drop");
            return;
        }

        var gold = Instantiate(goldCoinPrefab, transform.position, Quaternion.identity);
        gold.GetComponent<GoldItem>()
            .SetGoldCount(Random.Range(minGoldDropAmount, maxGoldDropAmount + 1));
    }

    public void UpdateProbabilities(DropTableEntry[] newDropTable) => dropTable = newDropTable;
}
