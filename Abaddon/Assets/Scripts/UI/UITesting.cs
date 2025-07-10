using UnityEngine;

public class UITesting : MonoBehaviour
{
    [SerializeField]
    private Message[] messages;
    private UIStateManager uiManager;

    void Awake()
    {
        uiManager = FindObjectOfType<UIStateManager>(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("#1");
            DialogueVisualiser.singleton.SetQueue(null, messages);
            print("#2");
            DialogueVisualiser.singleton.PlayCurrentMessage();
            print("#3");
            uiManager.ToggleUIPage(UIState.Dialogue);
            print("#4");
        }
    }
}
