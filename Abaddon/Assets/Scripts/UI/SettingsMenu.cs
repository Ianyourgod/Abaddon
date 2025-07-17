using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu singleton;

    [Space(10)]
    [Header("Keybinds")]
    [Header("Movement Keybinds")]
    [SerializeField]
    public Keybind moveUpwardKeybind;

    [SerializeField]
    public Keybind moveRightKeybind;

    [SerializeField]
    public Keybind moveDownwardKeybind;

    [SerializeField]
    public Keybind moveLeftKeybind;

    [SerializeField]
    public Keybind rotateLeftKeybind;

    [SerializeField]
    public Keybind rotateRightKeybind;

    [Header("Gameplay Keybinds")]
    [SerializeField]
    public Keybind interactKeybind;
    public Keybind dropKeybind;

    [SerializeField]
    public Keybind attackKeybind;

    [SerializeField]
    public Keybind skipDialogueKeybind;

    [SerializeField]
    public Keybind nextDialogueKeybind;

    [SerializeField]
    public Keybind previousDialogueKeybind;

    [Header("Important objects")]
    [SerializeField]
    private GameObject backGround;

    public void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
        Disable();
    }

    public void Enable()
    {
        backGround.SetActive(true);
        Controller.main.enabled = false;
        UIStateManager.singleton.FadeInDarkener(5f, 0.75f);
    }

    public void Disable()
    {
        backGround.SetActive(false);
        if (Controller.main)
            Controller.main.enabled = true;
        if (UIStateManager.singleton)
            UIStateManager.singleton.FadeOutDarkener(5f);
    }

    public void ChangeMasterAudioLevel(float newLevel)
    {
        AudioListener.volume = newLevel;
    }

    public void ChangeMusicAudioLevel(float newLevel)
    {
        AudioManagerBetter.main.SetMusicVolume(newLevel);
    }

    public void ChangeSFXAudioLevel(float newLevel)
    {
        AudioManagerBetter.main.SetSfxVolume(newLevel);
    }
}
