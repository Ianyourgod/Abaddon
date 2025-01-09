using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class uitesting : MonoBehaviour
{
    public UIStateManager uimanager;
    public DialogueVisualiser dialogue;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) uimanager.ToggleUIPage(UIState.Inventory, 0.2f);
        if (Input.GetKeyDown(KeyCode.T)) {
            dialogue.SetQueue(10, true, "testing", "Enim do culpa exercitation reprehenderit. Deserunt amet ea nisi dolore id do labore. Cillum tempor dolore et velit exercitation sunt quis sunt sunt dolor proident cupidatat elit labore sint. Excepteur quis nisi reprehenderit nostrud proident proident excepteur ut. Lorem aute consequat ea adipisicing veniam ullamco Lorem reprehenderit quis. Labore non officia ut labore aliquip ut sit officia nisi ad exercitation qui magna est. Laborum cupidatat sint ad. Anim nulla consectetur ad occaecat qui laborum occaecat. Laborum aliqua nulla elit ut est. Exercitation voluptate eu velit eiusmod laboris aute aliqua est Lorem adipisicing et occaecat sit dolore eiusmod. Ullamco amet irure non. Culpa incididunt adipisicing sit nulla nisi labore esse est pariatur nisi amet esse mollit sit elit. Proident exercitation proident culpa voluptate eu do. Consectetur enim culpa eu sint do veniam veniam officia deserunt officia. Quis ea proident elit velit enim et adipisicing occaecat reprehenderit sunt occaecat. Magna nisi nisi et aliqua exercitation minim adipisicing sint eu nulla consectetur officia. Dolore laborum non fugiat est nulla magna velit labore magna ut do culpa. Excepteur ex sit id sint minim labore aliqua quis irure dolor labore reprehenderit. Proident Lorem anim veniam in laboris sint proident dolore. Sit proident exercitation ea voluptate esse dolore mollit esse exercitation.");
            dialogue.AddToQueue(3, false, "Enim do culpa exercitation reprehenderit. Deserunt amet ea nisi dolore id do labore. Cillum tempor dolore et velit exercitation sunt quis sunt sunt dolor proident cupidatat elit labore sint. Excepteur quis nisi reprehenderit nostrud proident proident excepteur ut. Lorem aute consequat ea adipisicing veniam ullamco Lorem reprehenderit quis. Labore non officia ut labore aliquip ut sit officia nisi ad exercitation qui magna est. Laborum cupidatat sint ad. Anim nulla consectetur ad occaecat qui laborum occaecat. Laborum aliqua nulla elit ut est. Exercitation voluptate eu velit eiusmod laboris aute aliqua est Lorem adipisicing et occaecat sit dolore eiusmod. Ullamco amet irure non. Culpa incididunt adipisicing sit nulla nisi labore esse est pariatur nisi amet esse mollit sit elit. Proident exercitation proident culpa voluptate eu do. Consectetur enim culpa eu sint do veniam veniam officia deserunt officia. Quis ea proident elit velit enim et adipisicing occaecat reprehenderit sunt occaecat. Magna nisi nisi et aliqua exercitation minim adipisicing sint eu nulla consectetur officia. Dolore laborum non fugiat est nulla magna velit labore magna ut do culpa. Excepteur ex sit id sint minim labore aliqua quis irure dolor labore reprehenderit. Proident Lorem anim veniam in laboris sint proident dolore. Sit proident exercitation ea voluptate esse dolore mollit esse exercitation.");
            dialogue.PlayCurrentMessage();
            uimanager.ToggleUIPage(UIState.Dialogue, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (uimanager.currentState == null) {
                uimanager.OpenUIPage(UIState.Pause);
            }
            else {
                uimanager.ClosePages();
            }
        }
    }
}
