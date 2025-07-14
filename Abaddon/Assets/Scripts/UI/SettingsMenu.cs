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
    public Keybind moveUpwardKeybind;
    public Keybind moveRightKeybind;
    public Keybind moveDownwardKeybind;
    public Keybind moveLeftKeybind;
    public Keybind rotateLeftKeybind;
    public Keybind rotateRightKeybind;

    [Header("Gameplay Keybinds")]
    public Keybind interactKeybind;
    public Keybind dropKeybind;
    public Keybind attackKeybind;
    public Keybind skipDialogueKeybind;
    public Keybind nextDialogueKeybind;
    public Keybind previousDialogueKeybind;
    public Keybind increaseVolumeKeybind;
    public Keybind decreaseVolumeKeybind;

    public void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
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
