using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateManager : MonoBehaviour
{
    public enum UIState {
        Pause,
        Dialogue,
        Stats,
        Death,
        Win
    }
    UIState? currentState = null;
    //UIObject --------------------------------------------


    // // Start is called before the first frame update
    // void Start()
    // {
    //     visualizer = FindObjectOfType<DialogueVisualiser>();
    // }

    public void OpenUIPage(UIState newState) {
        switch (newState) {
            case UIState.Pause:
                // Code to open Pause UI
                break;
            case UIState.Dialogue:
                // Code to open Dialogue UI
                break;
            case UIState.Stats:
                // Code to open Stats UI
                break;
            case UIState.Death:
                // Code to open Death UI
                break;
            case UIState.Win:
                // Code to open Win UI
                break;
        }
    }

    public void CloseCurrentPage() {
        currentState = null;
    }
}
