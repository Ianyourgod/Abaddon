using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueVisualizer : MonoBehaviour
{
    private float timeLeftToType = 0;
    private float startAmmount = 0;
    private string message;
    private TextMeshProUGUI textbox;

    void Start() {
        textbox = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //Typewriter Effect
        if (timeLeftToType > 0) {
            float percentageOfMessage = 1 - (timeLeftToType/startAmmount);

            string characters = message.Substring(0, 1 + (int)(message.Length * percentageOfMessage));
            textbox.text = characters;
            timeLeftToType -= Time.deltaTime;
            if (timeLeftToType <= 0) {
                print("done sending message");
            }
        }
    }

    public void StartMessage(string message) {
        startAmmount = 10;               //should later be a custom value
        timeLeftToType = startAmmount; 
        this.message = message;
    }
    public void StartMessage2(string message, float time) {
        startAmmount = time;               //should later be a custom value
        timeLeftToType = startAmmount; 
        this.message = message;
    }
}
