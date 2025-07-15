using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : GenericNPC
{
    [SerializeField]
    private Message[] purchaseMessages;

    [SerializeField]
    private ShopItem[] shopContents;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private Sprite image;

    /*
    Message[] m = messages;
    messages = completionMessages;
    base.StartConversation();
    messages = m;
    Item r = Instantiate(reward);
    inventory.AddItem(r);
    */

    public override void StartConversation()
    {
        onFinish = () =>
        {
            print("opening ui");
            UIStateManager.singleton.OpenUIPage(UIState.Shop);
            Shop.singleton.SetItems(shopContents);
            Shop.singleton.SetShopkeeperImage(image);
        };
        base.StartConversation();
    }
}
