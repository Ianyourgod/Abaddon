using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Helpers : MonoBehaviour
{
    public static Helpers singleton;

    [SerializeField]
    GameObject textFadePrefab;

    [SerializeField]
    GameObject darkener;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(this);
    }

    public void SpawnHurtText(string text, Vector3 position)
    {
        RealTextFadeUp damageAmount = Instantiate(textFadePrefab, position, Quaternion.identity)
            .GetComponent<RealTextFadeUp>();

        // TODO! use the red defined in the hex codes document
        damageAmount.SetText(text, Color.red, Color.white, 0.4f);
        damageAmount.SetBorderColor(Color.white, 0.0f);
        damageAmount.timeLimit = 1f;
    }
}

public static class StaticHelpers
{
    public static T Pop<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new InvalidOperationException("Cannot pop from an empty or null list.");
        }
        T item = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return item;
    }
}
