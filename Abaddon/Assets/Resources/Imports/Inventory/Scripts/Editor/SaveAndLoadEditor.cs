///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveAndLoad))]
public class SaveAndLoadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset Save Files"))
        {
            SaveAndLoad save = (SaveAndLoad)target;
            save.ResetSaveFiles();
        }
    }
}
