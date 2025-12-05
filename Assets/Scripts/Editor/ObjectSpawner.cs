using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class ObjectSpawner : EditorWindow
{
    List<GameObject> hasAllTheRecentlySpawnedObjects = new List<GameObject>();
    private string preFix = "", postFix = "", parentName;
    private GameObject objectToSpawn, parentObject;
    int objectID = 1, howManyRows,howManyObjectsToInstantiate,
        xRowRepeatingPatternInput, zRowRepeatingPatternInput;
    
    float objectScale = 1f, 
        xOffsetInput, zOffsetInput, xRowOffsetStart, zRowOffsetStart;
    
    [MenuItem("HaMiLeJa/ Object Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ObjectSpawner));
    }
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Spawn Objects", EditorStyles.boldLabel);
        preFix = EditorGUILayout.TextField("Prefix", preFix);
        objectID = EditorGUILayout.IntField("Object ID", objectID);
        postFix = EditorGUILayout.TextField("Postfix", postFix);
        xOffsetInput = EditorGUILayout.FloatField("xOffset", xOffsetInput);
        zOffsetInput = EditorGUILayout.FloatField("zOffset", zOffsetInput);
        howManyObjectsToInstantiate = EditorGUILayout.IntField("How many", howManyObjectsToInstantiate);
        GUILayout.Space(18);
        GUILayout.Label("Do you want Rows? ", EditorStyles.boldLabel);
        howManyRows = EditorGUILayout.IntField("How many Rows", howManyRows);
        xRowOffsetStart = EditorGUILayout.FloatField("Row Offset Start X position", xRowOffsetStart);
        zRowOffsetStart = EditorGUILayout.FloatField("Row Offset Start Z position", zRowOffsetStart);
        GUILayout.Space(18);
        GUILayout.Label("Row Repeat Pattern at Row", EditorStyles.boldLabel);
        xRowRepeatingPatternInput = EditorGUILayout.IntField("Repeat pattern at Row X", xRowRepeatingPatternInput);
        zRowRepeatingPatternInput = EditorGUILayout.IntField("Repeat pattern at Row Z", zRowRepeatingPatternInput);
        GUILayout.Space(18);
        GUILayout.Label("What object to spawn?", EditorStyles.boldLabel);
        objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.5f, 3f);
        objectToSpawn = EditorGUILayout.ObjectField("Prefab to Spawn", 
            objectToSpawn, typeof(GameObject), false) as GameObject;
        GUILayout.Space(5);
        if (GUILayout.Button("Spawn Object")) SpawnObject();
        GUILayout.Space(15);
        if (GUILayout.Button("Destroy recent Objects"))
        {
            DestroySpawnedObjects();
            hasAllTheRecentlySpawnedObjects.Clear();
        }
        GUILayout.Space(15);
        GUILayout.Label("Parent Objects ", EditorStyles.boldLabel);
        parentName = EditorGUILayout.TextField("Parent Name", parentName);
        GUILayout.Space(15);
        if (GUILayout.Button("Parent recent Objects")) AddSpawnedObjectsToParent();
        if (GUILayout.Button("Connect to prefab")) ConnectToPrefab();
    }
    void ConnectToPrefab()
    {
        foreach (GameObject preFabMe in hasAllTheRecentlySpawnedObjects)
        {
            GameObject newObject;
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(objectToSpawn);
            newObject.transform.position = preFabMe.transform.position;
            newObject.transform.rotation = preFabMe.transform.rotation;
            newObject.transform.parent = preFabMe.transform.parent;
            String rename = preFabMe.name;
            newObject.name = rename;
            DestroyImmediate(preFabMe);
        }
    }
    private void AddSpawnedObjectsToParent()
    {
        if (hasAllTheRecentlySpawnedObjects == null || parentName == string.Empty)
        {
            Debug.Log("You forgot to Name the Parent or create the objects"); return;
        }
        parentObject= new GameObject(parentName);
        Selection.activeObject = parentObject;
        foreach(GameObject parentMe in hasAllTheRecentlySpawnedObjects) parentMe.transform.parent = parentObject.transform;   
    }
    private void DestroySpawnedObjects()
    {
        if (hasAllTheRecentlySpawnedObjects == null)
        {
            Debug.Log("First spawn objects"); return;
        }
        foreach (GameObject destroyMe in hasAllTheRecentlySpawnedObjects) DestroyImmediate(destroyMe);
        if (parentObject != null) DestroyImmediate(parentObject);
    }
    void SpawnObject()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("Please assign an object to be spawned."); return;
        }
        if (preFix == string.Empty)
        {
            Debug.LogError("Please enter a base name for the object."); return;
        }
        float xOffset, zOffset, xRowOffset = 0, zRowOffset = 0;
        int xRowCounter = 1, zRowCounter = 1;
        for (int j = 0; j < howManyRows; j++)
     {
            xRowOffset += xRowOffsetStart; zRowOffset += zRowOffsetStart;
            xOffset = 1; zOffset = 1;
            
            for (int i = 0; i < howManyObjectsToInstantiate; i++)
            {
                if (xRowCounter == xRowRepeatingPatternInput)
                {
                    xRowOffset = xRowOffsetStart;
                    xRowCounter = 1;
                }
                
                if (zRowCounter == zRowRepeatingPatternInput)
                {
                    zRowOffset = xRowOffsetStart;
                    zRowCounter = 1;
                }
                Vector3 spawnPos = new Vector3(xOffset+xRowOffset, 0f, zOffset+zRowOffset);
                GameObject newObject = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
                newObject.name = preFix + " " + objectID + " " + postFix;
                newObject.transform.localScale = Vector3.one * objectScale;
                hasAllTheRecentlySpawnedObjects.Add(newObject);
                objectID++; xOffset += xOffsetInput; zOffset += zOffsetInput;
            }
          if(xRowRepeatingPatternInput != 0) xRowCounter++;
          if(zRowRepeatingPatternInput != 0) zRowCounter++;
     }
    }
}
    
