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
    ShopItem? shop_item;

    string item_name;

    ImageComponent image;

    public void SetShopItem(ShopItem s_item)
    {
        shop_item = s_item;
        var item_prefab = s_item.prefab;
        Item item = item_prefab.GetComponent<Item>();
        image = GetComponent<ImageComponent>();
        image.sprite = item.GetComponent<SpriteRenderer>().sprite;
        item_name = s_item.name;
    }

    void Awake()
    {
        if (shop_item != null)
        {
            SetShopItem(shop_item.Value);
        }
    }

    public void OnClick()
    {
        if (shop_item != null)
        {
            shop.ItemSelected(shop_item.Value);
        }
    }
}
