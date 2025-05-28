using UnityEngine;

public class UITesting : MonoBehaviour
{
    [SerializeField] private Message[] messages;
    private UIStateManager uiManager;
    private DialogueVisualiser dialogueVisualiser;

    void Awake()
    {
        uiManager = FindObjectOfType<UIStateManager>(true);
        dialogueVisualiser = FindObjectOfType<DialogueVisualiser>(true);
        print($"{uiManager.gameObject.name} {uiManager.gameObject.activeSelf}");
        print($"{dialogueVisualiser.gameObject.name} {dialogueVisualiser.gameObject.activeSelf}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("#1");
            dialogueVisualiser.SetQueue(messages);
            print("#2");
            dialogueVisualiser.PlayCurrentMessage();
            print("#3");
            uiManager.ToggleUIPage(UIState.Dialogue);
            print("#4");
        }
    }
}