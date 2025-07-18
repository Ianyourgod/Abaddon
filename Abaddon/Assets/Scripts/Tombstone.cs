using System.Linq;
using UnityEngine;

public class Tombstone : MonoBehaviour, CanBeDamaged
{
    [SerializeField]
    public Sprite brokenTombstoneSprite;

    [SerializeField]
    private Item[] items;

    public void SetItems(Item[] newItems)
    {
        var copiedItems = newItems.Where(i => i).Select(i => Instantiate(i)).ToArray();
        items = copiedItems;
    }

    public int Hurt(int damage)
    {
        if (Controller.main == null)
            return -1;

        var nonVoidItems = items.Where(i => i != null).ToArray();
        print(
            $"Picking up tombstone items: [{string.Join(", ", nonVoidItems.Select(i => i.name))}] ({nonVoidItems.Length}/{items.Length} items)"
        );
        Controller.main.inventory.AddItems(items.ToList());
        items = new Item[0];
        GetComponent<SpriteRenderer>().sprite = brokenTombstoneSprite;
        gameObject.layer = LayerMask.NameToLayer("Default");
        return -1;
    }
}
