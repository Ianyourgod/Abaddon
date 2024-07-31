using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    [SerializeField] UnityEngine.UI.Image panel;
    [SerializeField] Transform sprite;
    [SerializeField] float startingY = -600f;
    bool paused = false;

    public void Pause() {
        paused = true;
        Controller.main.enabled = false;
    }

    public void Unpause() {
        paused = false;
        Controller.main.enabled = true;
    }

    void Update() {
        if (paused && panel.color.a < 0.75f) {
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a + 0.01f);
            // drop down the image
        } else if (!paused && panel.color.a >= 0.05f) {
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a - 0.01f);
        }

        if (paused && sprite.localPosition.y < -10f) {
            sprite.localPosition = new Vector3(
                sprite.localPosition.x,
                Mathf.Lerp(sprite.localPosition.y, -10, 0.1f),
                sprite.localPosition.z
            );
        } else if (!paused && sprite.localPosition.y >= startingY) {
            sprite.localPosition = new Vector3(
                sprite.localPosition.x,
                Mathf.Lerp(sprite.localPosition.y, startingY, 0.1f),
                sprite.localPosition.z
            );
        }
    }
}