using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSender : MonoBehaviour
{
    Button button;

    public void SendTestingMessage()
    {
        DialogueVisualiser.singleton.WriteMessage(
            "This is a test message",
            1,
            TimeSettings.CharsPerSecond
        );
    }
}
