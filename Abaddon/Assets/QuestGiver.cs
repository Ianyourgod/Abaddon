using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : GenericNPC
{
    [SerializeField]
    private Message[] completionMessages;

    [SerializeField]
    public Controller.Quest questID;

    public override void StartConversation()
    {
        if (Controller.main == null)
            return;

        print(questID);
        print(Controller.main.completed_quests.ToString());
        if (Controller.main.completed_quests.Contains(questID))
        {
            Message[] m = messages;
            messages = completionMessages;
            base.StartConversation();
            messages = m;
        }
        else if (!Controller.main.current_quests.Contains(questID))
        {
            base.StartConversation();
            Controller.main.current_quests.Add(questID);
        }
        // TODO! text when you havent finished the quest
    }
}
