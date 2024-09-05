using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Loot Tables", order = 1)]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public class DroppableItem
    {
        public GameObject item;

        [Range(0, 1)] public float dropChance;

        public static implicit operator GameObject(DroppableItem item) => item.item;
    }
    
    public List<DroppableItem> items = new List<DroppableItem>();
    private List<(DroppableItem, float)> itemChances = new List<(DroppableItem, float)>();


    //Called every time a value is changed in the inspector and checks if the total drop chance is more than 100%
    private void OnValidate()
    {
        float maxChance = 0;
        foreach (DroppableItem item in items) {
            maxChance += item.dropChance;
            itemChances.Add((item, maxChance));
        }
        if (maxChance > 1) {
            Debug.LogError($"Total chance of items in loot table '{name}' is more than 100%. Items in table are: ({string.Join(", ", items.Select(x => $"{x.item.name}: {x.dropChance * 100}%"))}) for a total of {maxChance * 100}%");
        }
    }

    public DroppableItem PickItem() {
        float randomPercent = UnityEngine.Random.value;
        foreach (var (item, chance) in itemChances.OrderBy(x => x.Item2))
            if (randomPercent <= chance) return item;

        return null;
    }

    //For testing that the loot table pulls each item with the correct probability
    public void RunTests(int n) {
        Dictionary<string, int> results = new Dictionary<string, int>();
        for (int i = 0; i < n; i++) {
            DroppableItem possibleDrop = PickItem();
            if (possibleDrop is not null) {
                string name = possibleDrop.item.name;
                if (results.ContainsKey(name)) {
                    results[name]++;
                } else {
                    results[name] = 1;
                }
            }
        }
        Debug.Log($"Results of 1000 tests: ({string.Join(", ", results.Select(x => $"{x.Key}: {x.Value}"))})");
    }
}