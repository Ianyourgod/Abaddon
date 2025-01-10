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
    private float lerpSpeed = 0;
    private float intendedDarkValue = 0;

    private void Awake() {
        singleton = this;
        darkenerInstance = Instantiate(darkener_prefab, transform).GetComponent<Image>();
        darkenerInstance.transform.SetAsFirstSibling();
        darkenerInstance.gameObject.SetActive(false);
    }

    private void Update() {
        if (lerpSpeed != 0) {
            print($"dark value: {intendedDarkValue - darkenerOpacity} - (current: {darkenerOpacity}, intended: {intendedDarkValue})");
            darkenerOpacity = Mathf.Lerp(darkenerOpacity, intendedDarkValue, Time.deltaTime * lerpSpeed);
            print($"dark value: {intendedDarkValue - darkenerOpacity} - (current: {darkenerOpacity}, intended: {intendedDarkValue})");
            if (intendedDarkValue - darkenerOpacity < 0.03f) {
                lerpSpeed = 0;
                darkenerOpacity = intendedDarkValue;
                intendedDarkValue = 0;
                print("finished lerping");
            }
        }
    }

    public void FadeInDarkener(float startValue, float endValue, float speed) {
        darkenerOpacity = startValue;
        SetDarkenedBackground(true);
        lerpSpeed = speed;
        intendedDarkValue = endValue;
        print($"dark value: {darkenerOpacity - intendedDarkValue} - (current: {darkenerOpacity}, intended: {intendedDarkValue})");
    }

    public void ToggleUIPage(UIState newState, float? darkLevel = null, float? lerpSpeed = null) {
        if (currentState == newState) ClosePages();
        else {
            SetUIPage(newState);
            
            if (darkLevel != null) {
                if (lerpSpeed != null) {
                    FadeInDarkener(0, (float)darkLevel, (float)lerpSpeed);
                }
                else {
                    darkenerOpacity = (float)darkLevel;
                    SetDarkenedBackground(true);
                }
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
            if (screen.screenObject) screen.screenObject.SetActive(true);
            screen.onEnable?.Invoke();
        }
    }

    public void ClosePages() {
        currentState = null;
        SetDarkenedBackground(false);
        foreach (var screen in screens) {
            if (!screen.screenObject || screen.screenObject.activeInHierarchy) screen.onDisable?.Invoke();
            if (screen.screenObject) screen.screenObject.SetActive(false);
        }
    }

    public void SetDarkenedBackground(bool shouldDarken) => darkenerInstance.gameObject.SetActive(shouldDarken);
    public void ToggleDarkenedBackground() => darkenerInstance.gameObject.SetActive(!darkenerInstance.IsActive());
}
