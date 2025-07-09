using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Tombstone : MonoBehaviour, CanBeInteractedWith
{
    [SerializeField] private Item[] items;

    public void SetItems(Item[] newItems)
    {
        items = newItems;
    }

    public void Interact()
    {
        print("Picking up tombstone items");
        foreach (var item in items)
        {
            Controller.main.inventory.AddItem(item);
        }
        items = new Item[0];
    }
}
