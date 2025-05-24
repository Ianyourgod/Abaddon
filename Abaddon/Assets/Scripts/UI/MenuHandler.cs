using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler instance;
    [SerializeField] List<Behaviour> visibleComponents;
    [SerializeField] List<GameObject> visibleObjects;
    [SerializeField] List<GameObject> pausedObjects;
    [SerializeField] List<Behaviour> pausedComponents;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] Inventory inventory;
    [SerializeField] KeyCode inventoryKey;
    [SerializeField] TransitionIntoHandler transitionIntoHandler;

    public bool isPaused {get; private set;} = false;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventory.isInventoryOpen)
                inventory.CloseInventory();
            else
                TogglePauseMenu();
            return;
        }
        if(Input.GetKeyDown(inventoryKey))
            if (inventory.isInventoryOpen) inventory.CloseInventory();
            else inventory.OpenInventory();
    }

    void TogglePauseMenu() {
        // isPaused = !isPaused;
        // if (isPaused) {
        //     Pause();
        // }
        // else {
        //     Unpause();
        // }
    }

    void Pause() {
        Time.timeScale = 0f;
        SetAllListsTo(true);
        pauseMenu.Pause();
    }

    void Unpause() {
        Time.timeScale = 1f;
        SetAllListsTo(false);
        pauseMenu.Unpause();
    }

    void SetAllListsTo(bool input) {
        foreach (Behaviour g in visibleComponents) {
            g.enabled = input;
        }
        foreach (GameObject g in visibleObjects) {
            g.SetActive(input);
        }
        foreach (Behaviour g in pausedComponents) {
            g.enabled = !input;
        }
        foreach (GameObject g in pausedObjects) {
            g.SetActive(!input);
        }
    }

    public void Quit() {
        Unpause();
        transitionIntoHandler.SwitchSceneToMainMenu();
    }
}
