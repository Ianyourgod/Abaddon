using System.Linq;
using UnityEngine;

public class PlayerTombstone : MonoBehaviour, CanBeInteractedWith
{
    public Animator animator;

    [SerializeField]
    private Item[] items;

    public void SetItems(Item[] newItems)
    {
        var copiedItems = newItems.Where(i => i).Select(i => Instantiate(i)).ToArray();
        items = copiedItems;
    }

    public void Interact()
    {
        if (Controller.main == null)
            return;

        var nonVoidItems = items.Where(i => i != null).ToArray();
        print(
            $"Picking up tombstone items: [{string.Join(", ", nonVoidItems.Select(i => i.name))}] ({nonVoidItems.Length}/{items.Length} items)"
        );
        Controller.main.inventory.AddItems(items.ToList());
        items = new Item[0];
        animator.Play("break");
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
