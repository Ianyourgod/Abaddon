using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericNPC : MonoBehaviour, CanCommunicateWith
{
    [SerializeField] public string npcName;
    [SerializeField] public Message[] messages;

    public virtual void StartConversation()
    {
        Debug.Log("Starting conversation");
        UIStateManager.singleton.OpenUIPage(UIState.Dialogue);
        DialogueVisualiser.singleton.SetQueueAndPlayFirst(messages);
    }
}


public interface CanCommunicateWith
{
    void StartConversation();
}