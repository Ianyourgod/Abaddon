using UnityEditor;
using UnityEngine;

public class FindMissingScriptsRecursively : EditorWindow
{
    static int go_count = 0,
        components_count = 0,
        missing_count = 0;

    [MenuItem("Window/Find Missing Scripts Recursively")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            FindInSelected();
        }
    }

    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(
            $"Searched {go_count} GameObjects, {components_count} components, found {missing_count} missing"
        );
    }

    private static void FindInGO(GameObject g)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.Log(s + " has an empty script attached in position: " + i, g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject);
        }
    }
}
