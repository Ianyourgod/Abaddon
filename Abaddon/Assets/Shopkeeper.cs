using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : GenericNPC
{
    [SerializeField] private Message[] purchaseMessages;
    [SerializeField] private Item[] shopContents;
    [SerializeField] private Inventory inventory;

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
        base.StartConversation();
    }
}