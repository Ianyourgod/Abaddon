using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ImageComponent = UnityEngine.UI.Image; //To make the Image class be the correct image component and not some weird microsoft stuff

public class ShopSlot : MonoBehaviour
{
    [SerializeField]
    Shop shop;

    [SerializeField]
    Item item;
    ImageComponent image;

    void Awake()
    {
        if (item)
        {
            image = GetComponent<ImageComponent>();
            image.sprite = item.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void OnClick()
    {
        if (item)
        {
            shop.ItemSelected(item);
        }
    }
}
