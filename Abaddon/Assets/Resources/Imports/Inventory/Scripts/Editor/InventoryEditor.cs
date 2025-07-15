///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        GUILayout.Label(Resources.Load("Image") as Texture2D, GUILayout.ExpandWidth(true));
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        base.OnInspectorGUI();
        GUILayout.EndVertical();
    }
}
