using System;
using UnityEditor;
using UnityEngine;
public class HotSwap : EditorWindow
{
    GameObject newType;
    [MenuItem("HaMiLeJa/Hotswap")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HotSwap));
    }
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("!! NO UNDO !!  One Way Ticket BE CAREFUL !!", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("HotSwap", EditorStyles.boldLabel);
        GUILayout.Label("Changes the selected object(s) to the choosen prefab", EditorStyles.helpBox);
        newType = EditorGUILayout.ObjectField("Will be swapped to: ", newType, typeof(GameObject), 
            false) as GameObject;
        GUILayout.Space(5);
        if (GUILayout.Button("Replace")) swapTheObjects();
    }
    void swapTheObjects()
    {
        foreach (GameObject swapMe in Selection.gameObjects)
        {
            GameObject newObject;
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(newType);
            newObject.transform.position = swapMe.transform.position;
            newObject.transform.rotation = swapMe.transform.rotation;
            newObject.transform.parent = swapMe.transform.parent;
            String rename = swapMe.name;
            newObject.name = rename;
            DestroyImmediate(swapMe);
        }
    }
}