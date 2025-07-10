using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DropTableEntry
{
    public Item item;
    public float chance;

    public DropTableEntry(Item item, float chance)
    {
        this.item = item;
        this.chance = chance;
    }

    public DropTableEntry(KeyValuePair<Item, float> entry)
    {
        item = entry.Key;
        chance = entry.Value;
    }
}

[CreateAssetMenu(fileName = "DropTableSO", menuName = "")]
public class DropTableSO : ScriptableObject
{
    [SerializeField]
    public DropTableEntry[] dropTable;

    public Dictionary<Item, float> ConvertToDictionary()
    {
        Dictionary<Item, float> dropDict = new Dictionary<Item, float>();
        foreach (var item in dropTable)
        {
            if (item.item != null)
            {
                dropDict[item.item] = item.chance;
            }
        }
        return dropDict;
    }

    public void SetTable(Dictionary<Item, float> newTable)
    {
        dropTable = new DropTableEntry[newTable.Count];
        int index = 0;
        foreach (var entry in newTable)
        {
            dropTable[index] = new DropTableEntry(entry.Key, entry.Value);
            index++;
        }
    }
};
