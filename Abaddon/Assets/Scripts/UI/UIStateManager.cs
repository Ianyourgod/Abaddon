using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UIState {
    Pause,
    Dialogue,
    Inventory,
    Death,
    Win
}

public class UIStateManager : MonoBehaviour
{
    [SerializeField] private readonly GameObject _pauseScreen;
    [SerializeField] private readonly UnityEvent _pauseOnEnable;
    [SerializeField] private readonly UnityEvent _pauseOnDisable;
    private UIScreen pauseScreen;

    [SerializeField] private readonly GameObject _inventoryScreen;
    [SerializeField] private readonly UnityEvent _inventoryOnEnable;
    [SerializeField] private readonly UnityEvent _inventoryOnDisable;
    private UIScreen inventoryScreen;

    private Dictionary<UIState, UIScreen> mapping;

    [SerializeField] private readonly GameObject _dialogueScreen;
    [SerializeField] private readonly GameObject _deathScreen;
    [SerializeField] private readonly GameObject _winScreen;

    public static UIStateManager singleton;
    [SerializeField] private List<UIScreen> screens = new List<UIScreen>();
    [SerializeField] private GameObject darkener_prefab;
    private Image darkenerInstance;
    private float _darkenerOpacity = 1;
    public float darkenerOpacity {
        get {
            return _darkenerOpacity;
        } 
        set {
            _darkenerOpacity = value;
            var color = darkenerInstance.color;
            color.a = value;
            darkenerInstance.color = color;
        }
    }

    [Serializable] public struct UIScreen
    {
        public UIState state;
        public GameObject screenObject;
        public UnityEvent onEnable;
        public UnityEvent onDisable;
    }

    public UIState? currentState {get; private set;} = null;

    private void Awake() {
        singleton = this;
        darkenerInstance = Instantiate(darkener_prefab, transform).GetComponent<Image>();
        darkenerInstance.transform.SetAsFirstSibling();
        darkenerInstance.gameObject.SetActive(false);
        pauseScreen = new UIScreen() {
            state = UIState.Pause,
            screenObject = _pauseScreen,
            onEnable = _pauseOnEnable,
            onDisable = _pauseOnDisable
        };
        inventoryScreen = new UIScreen() {
            state = UIState.Inventory,
            screenObject = _inventoryScreen,
            onEnable = _inventoryOnEnable,
            onDisable = _inventoryOnDisable
        };
        mapping = new Dictionary<UIState, UIScreen>() {
            { UIState.Pause, pauseScreen },
            { UIState.Inventory, inventoryScreen },
        };
    }

    public UIScreen StateToScreen(UIState state) {
        switch (state) {
            case UIState.Pause:
                return pauseScreen;
            // case UIState.Dialogue:
            //     return dialogueScreen;
            case UIState.Inventory:
                return inventoryScreen;
            // case UIState.Death:
            //     return deathScreen;
            // case UIState.Win:
            //     return winScreen;
            default:
                throw new ArgumentException("Invalid UIState");
        }
    }

    public void ToggleUIPage(UIState newState) {
        if (currentState == newState) {
            ClosePages();
        }
        else {
            OpenUIPage(newState);
        }
    }

    public void OpenUIPage2(UIState newState) {
        if (newState == currentState) return;
        UIScreen screen;
        if (currentState != null) {
            screen = StateToScreen((UIState)currentState);
            screen.screenObject.SetActive(false);
            screen.onDisable?.Invoke();
        }
        
        screen = StateToScreen(newState);
        StateToScreen((UIState)currentState).screenObject.SetActive(true);
        screen.screenObject.SetActive(false);
        screen.onEnable?.Invoke();

        currentState = newState;
    }

    public void ClosePage2() {
        currentState = null;
        foreach (var screen in screens) {
            if (screen.screenObject.activeInHierarchy) {
                screen.onDisable?.Invoke();
            }
            screen.screenObject.SetActive(false);
        }
    }


    public void OpenUIPage(UIState newState) {
        if (newState == currentState) return;
        
        ClosePages();
        currentState = newState;
        foreach (var screen in screens.Where(screen => screen.state == newState)) {
            screen.screenObject.SetActive(true);
            screen.onEnable?.Invoke();
        }
    }

    public void ClosePages() {
        currentState = null;
        foreach (var screen in screens) {
            if (screen.screenObject.activeInHierarchy) {
                screen.onDisable?.Invoke();
            }
            screen.screenObject.SetActive(false);
        }
    }

    public void SetDarkenedBackground(bool shouldDarken) => darkenerInstance.gameObject.SetActive(shouldDarken);
    public void ToggleDarkenedBackground() => darkenerInstance.gameObject.SetActive(!darkenerInstance.IsActive());
}
