using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class DadHarry : GenericNPC
{
    [SerializeField]
    protected Message[] completionMessages;

    [SerializeField]
    public Controller.Quest questID;

    [SerializeField]
    protected Item reward;

    public override void StartConversation()
    {
        if (Controller.main == null)
            return;

        if (Controller.main.completed_quests.Contains(questID))
        {
            print("you're soooo cool");
            return;
        }

        if (Controller.main.GetQuestState().EmoBoySaved)
        {
            print("AAAAAAAA");
            Message[] m = messages;
            messages = completionMessages;
            base.StartConversation();
            messages = m;
            Item r = Instantiate(reward);
            Controller.main.inventory.AddItem(r);
            Controller.main.current_quests.Remove(questID);
            Controller.main.completed_quests.Add(questID);
        }
        else if (!Controller.main.current_quests.Contains(questID))
        {
            base.StartConversation();
            Controller.main.current_quests.Add(questID);
        }
        // TODO! text when you havent finished the quest
    }
}
