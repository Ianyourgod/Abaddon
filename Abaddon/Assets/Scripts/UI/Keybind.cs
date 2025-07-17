using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Keybind : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI buttonLabel;

    [SerializeField]
    private string keybindLabel = "Move Up";

    public KeyCode key = KeyCode.None;
    private bool listeningForKey = false;
    private Color originalColor;
    private Color hoverColor = Color.yellow;

    void Awake()
    {
        bool failedToParse = !UnityEngine.ColorUtility.TryParseHtmlString(
            "#f54929",
            out hoverColor
        );
        if (failedToParse)
        {
            Debug.LogError("Failed to parse hover color for settings.");
        }
    }

    void OnEnable()
    {
        if (buttonLabel)
            originalColor = buttonLabel.color;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonLabel)
            buttonLabel.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonLabel)
            buttonLabel.color = originalColor;
    }
}
