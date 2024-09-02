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

    public static string ToStringDirection(this Vector2 direction) {        
        if (direction == Vector2.up) {
            return "back";
        } else if (direction == Vector2.down) {
            return "front";
        } else if (direction == Vector2.left) {
            return "left";
        } else if (direction == Vector2.right) {
            return "right";
        }
        
        throw new Exception("Invalid direction");
    }

    // public static AnimationEventHandler.Animation Attack { get => new AnimationEventHandler.Animation("attack", 1, false); }
    // public static AnimationEventHandler.Animation Hurt { get => new AnimationEventHandler.Animation("hurt", 1, false); }
    // public static AnimationEventHandler.Animation Die { get => new AnimationEventHandler.Animation("death", 10, false); }
}