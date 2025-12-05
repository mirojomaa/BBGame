#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RenderBounds : EditorWindow
{
    public static readonly HashSet<MeshFilter> RenderBoundsList = new HashSet<MeshFilter>();
    private  static readonly HashSet<MeshFilter> HasAllBoundsToModify = new HashSet<MeshFilter>();
    private const float LeftBoundMult = 0;
    private const float RightBoundMult = 11;
    private static float _multiplicationValue = 1;
    private const string IgnoreLayer = "Hex";
    private static bool noMultButFixedSize;
    [MenuItem("HaMiLeJa/ RenderBounds")]
    public static void ShowWindow()
    {
        GetWindow(typeof(RenderBounds));
    }
    private void OnGUI()
    {  
        GUILayout.Space(10);
        if (GUILayout.Button("Draw Bounds on Selected")) AddSelectedToList();
        GUILayout.Space(4);
        if (GUILayout.Button("Draw Bounds Ignore " + IgnoreLayer + " Selected")) AddSelectedToListIgnoreHex();
        GUILayout.Space(4);
        if (GUILayout.Button("Clear Bounds")) {RenderBoundsList.Clear(); Debug.Log("Everything cleared!!");}
        GUILayout.Space(4);
        if (GUILayout.Button("Change Bounds Size")) changeBoundsSize();
        _multiplicationValue = EditorGUILayout.Slider("Multiplicationvalue", _multiplicationValue, LeftBoundMult, RightBoundMult);
        GUILayout.Space(4);
        if (GUILayout.Button("Recalculate Bounds")) recalculateBoundsSize();
        GUILayout.Space(18);
        if (GUILayout.Button("Draw All Ignore " + IgnoreLayer + " Selected")) AddAllGameObjects();
        GUILayout.Space(4);
        if (GUILayout.Button("Get Local Bounds")) calculateBoundsParent();
        GUILayout.Space(4);
        noMultButFixedSize = EditorGUILayout.Toggle(noMultButFixedSize);
    }
    

    void calculateBoundsParent()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            obj.GetComponent<MeshFilter>().sharedMesh.bounds = GetLocalBoundsForObject(obj);
        }
    }
    static Bounds GetLocalBoundsForObject(GameObject go)
    {
        var referenceTransform = go.transform;
        var b = new Bounds(Vector3.zero, Vector3.zero);
        RecurseEncapsulate(referenceTransform, ref b);
        return b;
                       
        void RecurseEncapsulate(Transform child, ref Bounds bounds)
        {
            var mesh = child.GetComponent<MeshFilter>();
            if (mesh)
            {
                var lsBounds = mesh.sharedMesh.bounds;
                var wsMin = child.TransformPoint(lsBounds.center - lsBounds.extents);
                var wsMax = child.TransformPoint(lsBounds.center + lsBounds.extents);
                bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMin));
                bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMax));
            }
            foreach (Transform grandChild in child.transform)
            {
                RecurseEncapsulate(grandChild, ref bounds);
            }
        }
    }
    void AddSelectedToList()
    {
        RenderBoundsList.Clear();
        foreach (GameObject showMe in Selection.gameObjects)
        {
            if (showMe.transform.GetComponent<MeshFilter>() != null) RenderBoundsList.Add(showMe.transform.GetComponent<MeshFilter>());
            if (showMe.transform.childCount > 0) foreach (MeshFilter mf in showMe.transform.GetComponentsInChildren<MeshFilter>()) RenderBoundsList.Add(mf);
        }
        Debug.Log("You see the Render bounds of : " + RenderBoundsList.Count + " Meshfilters");
    }
    void AddSelectedToListIgnoreHex()
    {
        RenderBoundsList.Clear();
        foreach (GameObject showMe in Selection.gameObjects)
        {
            if (showMe.CompareTag("Hex"))
                foreach (MeshFilter mf in showMe.transform.GetComponentsInChildren<MeshFilter>()) RenderBoundsList.Add(mf);
            if (!showMe.CompareTag("Hex"))
            {
                if (showMe.transform.GetComponent<MeshFilter>() != null)
                    RenderBoundsList.Add(showMe.transform.GetComponent<MeshFilter>());
                if (showMe.transform.childCount > 0) foreach (MeshFilter mf in showMe.transform.GetComponentsInChildren<MeshFilter>()) RenderBoundsList.Add(mf);
            }
        }
        Debug.Log("You see the Render bounds of : " + RenderBoundsList.Count + " Meshfilters");
    }
    void AddAllGameObjects()
    {
        HexAutoTiling hex = FindObjectOfType<HexAutoTiling>();
        foreach (Transform hasHexTransform in hex.hasAllTheHexGameObjectsTransformsBeforeStart)
        {
            if (hasHexTransform.childCount > 0)
                foreach (MeshFilter mf in hasHexTransform.GetComponentsInChildren<MeshFilter>())
                {
                    if (mf != null && mf != hasHexTransform.GetComponent<MeshFilter>()) RenderBoundsList.Add(mf);
                }
        }
        Debug.Log("You see the Render bounds of : " + RenderBoundsList.Count + " Meshfilters");
    }
    void changeBoundsSize()
    {
        HashSet<Mesh> uniqueSharedMeshes = new HashSet<Mesh>();

        foreach (MeshFilter mf in RenderBoundsList)
        {
            uniqueSharedMeshes.Add(mf.sharedMesh);
        }

        List <Mesh> uniqueSharedMeshesList = uniqueSharedMeshes.ToList();
        
        if (Selection.gameObjects.Length == 0)
        {
            AddAllGameObjects();
            foreach (Mesh mesh in uniqueSharedMeshesList)
            {
                mesh.RecalculateBounds();
                mesh.bounds = new Bounds(mesh.bounds.center, new Vector3(mesh.bounds.size.x * _multiplicationValue, mesh.bounds.size.y, mesh.bounds.size.z * _multiplicationValue));
            }
        }
        
        foreach (GameObject showMe in Selection.gameObjects)
        {
            if (showMe.CompareTag("Hex")) foreach (MeshFilter mf in showMe.transform.GetComponentsInChildren<MeshFilter>()) HasAllBoundsToModify.Add(mf);
            if (!showMe.CompareTag("Hex"))
            {
                if (showMe.transform.GetComponent<MeshFilter>() != null) RenderBoundsList.Add(showMe.transform.GetComponent<MeshFilter>());
                if (showMe.transform.childCount > 0) foreach (MeshFilter mf in showMe.transform.GetComponentsInChildren<MeshFilter>()) HasAllBoundsToModify.Add(mf);
            }
        }
        Debug.Log("You see the Render bounds of : " + HasAllBoundsToModify.Count + " Meshfilters");
        foreach (MeshFilter mf in HasAllBoundsToModify)
        {
            mf.sharedMesh.RecalculateBounds();
            mf.sharedMesh.bounds = new Bounds(mf.sharedMesh.bounds.center, mf.sharedMesh.bounds.size * _multiplicationValue);
        }
    }
    void recalculateBoundsSize()
    {
        HashSet<Mesh> uniqueSharedMeshes = new HashSet<Mesh>();

        foreach (MeshFilter mf in RenderBoundsList)
        {
            uniqueSharedMeshes.Add(mf.sharedMesh);
        }

        List <Mesh> uniqueSharedMeshesList = uniqueSharedMeshes.ToList();
        
        if (Selection.gameObjects.Length == 0)
        {
            AddAllGameObjects();
            foreach (Mesh mesh in uniqueSharedMeshesList)
            {
                mesh.RecalculateBounds();
            }
        }
        
        foreach (GameObject showMe in Selection.gameObjects)
        {
            if (showMe.CompareTag("Hex")) 
                foreach (MeshFilter mf in showMe.transform.GetComponentsInChildren<MeshFilter>()) HasAllBoundsToModify.Add(mf);
            if (!showMe.CompareTag("Hex"))
            {
                if (showMe.transform.GetComponent<MeshFilter>() != null) RenderBoundsList.Add(showMe.transform.GetComponent<MeshFilter>());
                if (showMe.transform.childCount > 0) foreach (MeshFilter mf in showMe.transform.GetComponentsInChildren<MeshFilter>()) HasAllBoundsToModify.Add(mf);
            }
        }
        Debug.Log("Recalculating the Render bounds of : " + RenderBoundsList.Count + " Meshfilters");
        foreach (MeshFilter mf in HasAllBoundsToModify) mf.sharedMesh.RecalculateBounds();
    }
}
#endif
