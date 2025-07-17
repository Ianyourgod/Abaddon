using System;
using TMPro;
using UnityEngine;

public class Keybind : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI buttonLabel;

    [SerializeField]
    private string keybindLabel = "Move Up";

    public KeyCode key = KeyCode.None;
    private bool listeningForKey = false;

    void OnEnable()
    {
        if (key != KeyCode.None)
        {
            if (!buttonLabel)
            {
                print("I ERRORED: " + name);
            }
            buttonLabel.text = $"{key} - {keybindLabel}";
        }
    }

    private void Update()
    {
        if (listeningForKey)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    buttonLabel.text = $"{kcode} - {keybindLabel}";
                    key = kcode;
                    listeningForKey = false;
                    break;
                }
            }
        }
    }

    public void StartListeningForKey()
    {
        listeningForKey = true;
        if (!buttonLabel)
        {
            print("I ERRORED: " + name);
        }
        buttonLabel.text = $"... - {keybindLabel}";
    }

    void OnValidate()
    {
        if (!buttonLabel)
        {
            print("I ERRORED: " + name);
        }
        buttonLabel.text = $"{key} - {keybindLabel}";
    }
}
