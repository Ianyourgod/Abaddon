using System;
using TMPro;
using UnityEngine;

public class Keybind : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI buttonLabel;
    public KeyCode key = KeyCode.None;
    private bool listeningForKey = false;

    void OnEnable()
    {
        if (key != KeyCode.None)
        {
            buttonLabel.text = key.ToString();
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
                    buttonLabel.text = kcode.ToString();
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
        buttonLabel.text = "...";
    }
}
