using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    [SerializeField] UnityEngine.UI.Image panel;
    bool paused = false;

    void Update() {
        if (paused && panel.color.a < 0.75f) {
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a + 0.01f);
        } else if (!paused && panel.color.a > 0.05f) {
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a - 0.01f);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
        }
    }
}