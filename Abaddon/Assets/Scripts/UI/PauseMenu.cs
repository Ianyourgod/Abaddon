using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    [SerializeField] float lerpSpeed;
    [SerializeField] float springFactor;


    private Vector3 startingPosition;
    private float verticalDisplacement = -600;
    private bool reachedPosition = true;
    private bool paused = false;


    private void Start() {
        startingPosition = transform.position;
    }

    public bool IsPaused() => paused;


    public void Pause() {
        // Controller.main.enabled = false;
        reachedPosition = false;
        paused = true;
    }

    public void Unpause() {
        paused = false;
        reachedPosition = false;
        // Controller.main.enabled = true;
    }

    void Update() {
        if (!reachedPosition) {
            print($"moving to {(paused ? "paused" : "unpaused")} location");
            Vector3 endingPosition = startingPosition; 
            if (paused) endingPosition.y += verticalDisplacement; 
            transform.position = Vector3.Lerp(transform.position, endingPosition, lerpSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, endingPosition) <= 0.1f) { 
                reachedPosition = true;
                print("done!");
            }
        }
        // if (paused && panel.color.a < 0.75f) {
        //     panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a + 0.01f);
        //     // drop down the image
        // } else if (!paused && panel.color.a >= 0.05f) {
        //     panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a - 0.01f);
        // }

        // if (paused && sprite.localPosition.y < -10f) {
        //     sprite.localPosition = new Vector3(
        //         sprite.localPosition.x,
        //         Mathf.Lerp(sprite.localPosition.y, -10, 0.1f),
        //         sprite.localPosition.z
        //     );
        // } else if (!paused && sprite.localPosition.y >= startingY) {
        //     sprite.localPosition = new Vector3(
        //         sprite.localPosition.x,
        //         Mathf.Lerp(sprite.localPosition.y, startingY, 0.1f),
        //         sprite.localPosition.z
        //     );
        // }
    }
}