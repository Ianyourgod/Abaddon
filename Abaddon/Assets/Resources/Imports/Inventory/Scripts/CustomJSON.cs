///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using System.Collections.Generic;
using UnityEngine;

public class CustomJSON : MonoBehaviour
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(List<T> list)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = list;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }
}

[System.Serializable]
public class Wrapper<T>
{
    public List<T> Items;
}
