using UnityEngine;

public class Pet : MonoBehaviour
{
    // We assume that every Pet has:
    // - an Item component

    [SerializeField]
    Item itemComponent;

    [SerializeField]
    Sprite petSprite;

    void Awake()
    {
        if (itemComponent == null)
        {
            itemComponent = GetComponent<Item>();
        }
        if (petSprite == null)
        {
            // if not set, default to the item sprite
            petSprite = GetComponent<SpriteRenderer>().sprite;
        }

        if (itemComponent == null)
        {
            Debug.LogError("Pet must have an Item component attached.");
        }
        if (petSprite == null)
        {
            Debug.LogError("Pet must have a SpriteRenderer with a sprite attached.");
        }
    }
}
