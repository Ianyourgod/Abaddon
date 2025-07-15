using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericNPC : MonoBehaviour, CanCommunicateWith, CanBeInteractedWith
{
    [SerializeField]
    public string npcName;

    [SerializeField]
    protected Message[] messages;

    protected Action onFinish = null;

    public virtual void StartConversation()
    {
        Debug.Log("Starting conversation");
        UIStateManager.singleton.OpenUIPage(UIState.Dialogue);
        DialogueVisualiser.singleton.SetQueueAndPlayFirst(onFinish, messages);
    }

    public void Interact()
    {
        StartConversation();
    }
}

public interface CanCommunicateWith
{
    void StartConversation();
}
