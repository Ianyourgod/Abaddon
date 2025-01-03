using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSender : MonoBehaviour
{
    DialogueVisualizer visualizer;
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        visualizer = FindObjectOfType<DialogueVisualizer>();
    }

    public void SendTestingMessage() {
        visualizer.StartMessage2("This is a test message", 1);
    }
}
