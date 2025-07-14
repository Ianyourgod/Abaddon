using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoBoy : GenericNPC
{
    [SerializeField]
    private Message[] beforeEnemiesDead;

    [SerializeField]
    private Message[] whenEnemiesDead;

    [SerializeField]
    private Message[] afterEnemiesBeforeQuestComplete;

    [SerializeField]
    private Message[] afterQuestComplete;

    [SerializeField]
    public GameObject[] enemies;

    public override void StartConversation()
    {
        if (Controller.main == null)
            return;

        if (Controller.main.completed_quests.Contains(Controller.Quest.SaveEmoBoy))
        {
            Message[] m = messages;
            messages = afterQuestComplete;
            base.StartConversation();
            messages = m;
        }
        else
        {
            bool allEnemiesDead = true;
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    allEnemiesDead = false;
                    break;
                }
            }

            Controller.QuestState qs = Controller.main.GetQuestState();
            if (allEnemiesDead)
            {
                if (!qs.EmoBoySaved)
                {
                    qs.EmoBoySaved = true;
                    Message[] m = messages;
                    messages = whenEnemiesDead;
                    base.StartConversation();
                    messages = m;
                }
                else
                {
                    Message[] m = messages;
                    messages = afterEnemiesBeforeQuestComplete;
                    base.StartConversation();
                    messages = m;
                }
            }
            else
            {
                Message[] m = messages;
                messages = beforeEnemiesDead;
                base.StartConversation();
                messages = m;
            }
        }
        // TODO! text when you havent finished the quest
    }
}
