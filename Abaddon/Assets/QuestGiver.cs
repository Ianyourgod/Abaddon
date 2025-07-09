using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : GenericNPC
{
    [SerializeField] private Message[] completionMessages;
    [SerializeField] public Controller.Quest questID;

    public override void StartConversation()
    {
        if (Controller.main.completed_quests.Contains(questID))
        {
            Message[] m = messages;
            messages = completionMessages;
            base.StartConversation();
            messages = m;
        }
        else
        {
            base.StartConversation();
        }
    }
}