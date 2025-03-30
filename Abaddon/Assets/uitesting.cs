using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class uitesting : MonoBehaviour
{
    public UIStateManager uimanager;
    public DialogueVisualiser dialogue;
    public Message[] messages;
    public PauseMenu pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) uimanager.ToggleUIPage(UIState.Inventory, 0.2f);
        if (Input.GetKeyDown(KeyCode.Escape)) uimanager.ToggleUIPage(UIState.Pause);

        if (Input.GetKeyDown(KeyCode.T)) {
            dialogue.SetQueueAndPlayFirst(messages);
            uimanager.ToggleUIPage(UIState.Dialogue, 0.5f);
        }
    }
}
