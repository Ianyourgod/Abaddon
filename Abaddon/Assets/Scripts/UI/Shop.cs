using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ImageComponent = UnityEngine.UI.Image; //To make the Image class be the correct image component and not some weird microsoft stuff

public class Shop : MonoBehaviour
{
    [SerializeField] float timeToEnter;
    [SerializeField, Range(0, 1)] float shopDarknessLevel;

    void Start()
    {
        print("hi");
    }

    public void OnEnable()
    {
        Controller.main.enabled = false;
        UIStateManager.singleton.FadeInDarkener(timeToEnter, shopDarknessLevel);
    }

    public void OnDisable()
    {
        Controller.main.enabled = true;
        UIStateManager.singleton.FadeOutDarkener(timeToEnter, shopDarknessLevel);
    }
}