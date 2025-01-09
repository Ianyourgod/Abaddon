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

[Serializable]
public struct UIScreen {
    public UIState state;
    public GameObject screenObject;
    public UnityEvent onEnable, onDisable;

    public UIScreen(UIState state, GameObject screenObject = null, UnityEvent onEnable = null, UnityEvent onDisable = null) {
        this.state = state;
        this.screenObject = screenObject;
        this.onEnable = onEnable;
        this.onDisable = onDisable;
    }
}

public class UIStateManager : MonoBehaviour
{
    public static UIStateManager singleton;
    [SerializeField] private List<UIScreen> screens = new List<UIScreen>();

    [SerializeField] private GameObject darkener_prefab;
    private Image darkenerInstance;
    
    private float _darkenerOpacity = 1;
    public float darkenerOpacity {
        get { return _darkenerOpacity; } 
        set {
            _darkenerOpacity = value;
            var color = darkenerInstance.color;
            color.a = value;
            darkenerInstance.color = color;
        }
    }

    public UIState? currentState {get; private set;} = null;

    private void Awake() {
        singleton = this;
        darkenerInstance = Instantiate(darkener_prefab, transform).GetComponent<Image>();
        darkenerInstance.transform.SetAsFirstSibling();
        darkenerInstance.gameObject.SetActive(false);
    }

    public void ToggleUIPage(UIState newState, float? darkLevel = null) {
        if (currentState == newState) ClosePages();
        else {
            SetUIPage(newState);
            
            if (darkLevel != null) {
                darkenerOpacity = (float)darkLevel;
                SetDarkenedBackground(true);
            }
        }

    }


    public void OpenUIPage(UIState newState) {
        if (newState == currentState) return;
        
        ClosePages();
        SetUIPage(newState);
    }

    public void SetUIPage(UIState newState) {
        currentState = newState;
        foreach (var screen in screens.Where(screen => screen.state == newState)) {
            screen.screenObject.SetActive(true);
            screen.onEnable?.Invoke();
        }
    }

    public void ClosePages() {
        currentState = null;
        SetDarkenedBackground(false);
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
