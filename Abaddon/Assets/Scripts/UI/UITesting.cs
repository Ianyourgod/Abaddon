using UnityEngine;

public class UITesting : MonoBehaviour
{
    [SerializeField] private Message[] messages;
    private UIStateManager uiManager;

    void Awake()
    {
        uiManager = FindObjectOfType<UIStateManager>(true);
        print($"{uiManager.gameObject.name} {uiManager.gameObject.activeSelf}");
        print($"{DialogueVisualiser.singleton.gameObject.name} {DialogueVisualiser.singleton.gameObject.activeSelf}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("#1");
            DialogueVisualiser.singleton.SetQueue(messages);
            print("#2");
            DialogueVisualiser.singleton.PlayCurrentMessage();
            print("#3");
            uiManager.ToggleUIPage(UIState.Dialogue);
            print("#4");
        }
    }
}