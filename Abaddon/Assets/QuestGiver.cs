using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : GenericNPC
{
    [SerializeField] private Message[] completionMessages;
    [SerializeField] private string tokenID;

    public override void StartConversation()
    {
        base.StartConversation();
    }
}