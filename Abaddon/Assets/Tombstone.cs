using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Tombstone : MonoBehaviour, CanBeInteractedWith
{
    [SerializeField] private Item[] items;

    public void SetItems(Item[] newItems)
    {
        var copiedItems = newItems.Where(i => i).Select(i => Instantiate(i)).ToArray();
        items = copiedItems;
    }

    public void Interact()
    {
        var nonVoidItems = items.Where(i => i != null).ToArray();
        print($"Picking up tombstone items: [{string.Join(", ", nonVoidItems.Select(i => i.name))}] ({nonVoidItems.Length}/{items.Length} items)");
        Controller.main.inventory.AddItems(items.ToList());
        items = new Item[0];
        GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
