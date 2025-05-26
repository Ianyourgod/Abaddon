using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UIState
{
    Pause,
    Dialogue,
    Inventory,
    Death,
    Win
}

[Serializable]
public struct UIScreen
{
    public UIState state;
    public GameObject screenObject;
    public KeyCode defaultKey;
    public UnityEvent onEnable, onDisable;

    public UIScreen(UIState state, GameObject screenObject = null, KeyCode defaultKey = KeyCode.None, UnityEvent onEnable = null, UnityEvent onDisable = null)
    {
        this.state = state;
        this.screenObject = screenObject;
        this.defaultKey = defaultKey;
        this.onEnable = onEnable;
        this.onDisable = onDisable;
    }
}

public class UIStateManager : MonoBehaviour
{
    public static UIStateManager singleton;
    [SerializeField] private GameObject darkener_prefab;
    [SerializeField] private List<UIScreen> screens = new List<UIScreen>();

    private Image darkenerInstance;

    private float _darkenerOpacity = 1;
    public float darkenerOpacity
    {
        get { return _darkenerOpacity; }
        set
        {
            _darkenerOpacity = value;
            var color = darkenerInstance.color;
            color.a = value;
            darkenerInstance.color = color;
        }
    }

    public UIState? currentState { get; private set; } = null;
    private float lerpSpeed = 0;
    private float intendedDarkValue = 0;

    private void Awake()
    {
        singleton = this;
        darkenerInstance = Instantiate(darkener_prefab, transform).GetComponent<Image>();
        darkenerInstance.transform.SetAsFirstSibling();
        darkenerInstance.gameObject.SetActive(false);
    }

    private void Start()
    {
        print("ui state manager awake");
    }

    private void Update()
    {
        screens.ForEach(screen =>
        {
            if (Input.GetKeyDown(screen.defaultKey)) ToggleUIPage(screen.state, 0.5f, 1.5f);
        });

        if (lerpSpeed != 0)
        {
            darkenerOpacity = Mathf.Lerp(darkenerOpacity, intendedDarkValue, Time.deltaTime * lerpSpeed);
            if (intendedDarkValue - darkenerOpacity < 0.03f)
            {
                lerpSpeed = 0;
                darkenerOpacity = intendedDarkValue;
                intendedDarkValue = 0;
            }
        }
    }

    public void FadeInDarkener(float startValue, float endValue, float speed)
    {
        darkenerOpacity = startValue;
        SetDarkenedBackground(true);
        lerpSpeed = speed;
        intendedDarkValue = endValue;
        // print($"dark value: {darkenerOpacity - intendedDarkValue} - (current: {darkenerOpacity}, intended: {intendedDarkValue})");
    }

    public void ToggleUIPage(UIState newState, float? darkLevel = null, float? lerpSpeed = null)
    {
        if (currentState == newState) ClosePages();
        else
        {
            SetUIPage(newState);

            if (darkLevel != null)
            {
                if (lerpSpeed != null)
                {
                    FadeInDarkener(0, (float)darkLevel, (float)lerpSpeed);
                }
                else
                {
                    darkenerOpacity = (float)darkLevel;
                    SetDarkenedBackground(true);
                }
            }
        }

    }


    public void OpenUIPage(UIState newState)
    {
        if (newState == currentState) return;

        ClosePages();
        SetUIPage(newState);
    }

    public void SetUIPage(UIState newState)
    {
        currentState = newState;
        foreach (var screen in screens.Where(screen => screen.state == newState))
        {
            print("setting screen " + screen.state + " to active");
            if (screen.screenObject) screen.screenObject.SetActive(true);
            screen.onEnable?.Invoke();
        }
    }

    public void ClosePages()
    {
        currentState = null;
        SetDarkenedBackground(false);
        foreach (var screen in screens)
        {
            if (!screen.screenObject || screen.screenObject.activeInHierarchy) screen.onDisable?.Invoke();
            if (screen.screenObject) screen.screenObject.SetActive(false);
        }
    }

    public void SetDarkenedBackground(bool shouldDarken) => darkenerInstance.gameObject.SetActive(shouldDarken);
    public void ToggleDarkenedBackground() => darkenerInstance.gameObject.SetActive(!darkenerInstance.IsActive());

    public void Win()
    {
        print("win");
        FadeInDarkener(0, 1, 1.5f);
        SetUIPage(UIState.Win);
    }
}
