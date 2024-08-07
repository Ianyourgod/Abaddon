using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using Unity.VisualScripting;

public class Helpers : MonoBehaviour {
    public static Helpers singleton;

    [SerializeField] GameObject textFadePrefab;

    private void Awake()
    {
        if (singleton == null) singleton = this;
        else Destroy(this);
    }

    public void SpawnHurtText(string text, Vector3 position) {
        RealTextFadeUp damageAmount = Instantiate(
            textFadePrefab,
            position, 
            Quaternion.identity
        ).GetComponent<RealTextFadeUp>();
        
        damageAmount.SetText(text, Color.red, Color.white, 0.4f);
    } 
}

public static class StaticHelpers {
    public static bool IsInList(this string input, params string [] list) {
        foreach (string item in list) {
            if (input == item) return true;
        }
        return false;
    }
}