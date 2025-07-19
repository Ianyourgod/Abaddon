using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uitesting : MonoBehaviour
{
    public UIStateManager uimanager;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            uimanager.ToggleUIPage(UIState.Inventory);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (uimanager.currentState == null) {
                uimanager.OpenUIPage(UIState.Pause);
            }
            else {
                uimanager.ClosePages();
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals)) {
            uimanager.darkenerOpacity += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            uimanager.darkenerOpacity -= 0.1f;
        }
    }
}
