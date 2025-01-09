using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSender : MonoBehaviour
{
    DialogueVisualiser visualizer;
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        visualizer = FindObjectOfType<DialogueVisualiser>();
    }

    public void SendTestingMessage() {
        visualizer.WriteMessage("This is a test message", 1, true);
    }
}
