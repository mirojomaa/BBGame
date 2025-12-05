using System;
using UnityEngine;
using UnityEditor;
public static class  NewParent
{
    private static GameObject LevelObj, Hex;
    private static String Hexlayer = "Hex";
    [MenuItem("HaMiLeJa/Parent Objects t%&Y", isValidateFunction: true)]
    public static bool TransformSValidate()
    {
        return Selection.gameObjects.Length > 0;
    }
    [MenuItem("HaMiLeJa/Parent Objects %&Y")]
    public static void ParrentTheObjects()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Hex = null; 
            LevelObj = go; 
            parenting();
        }
    }
    [MenuItem("HaMiLeJa/UnParent Objects t%&X", isValidateFunction: true)]
    public static bool TransformValidate()
    {
        return Selection.gameObjects.Length > 0;
    }
    [MenuItem("HaMiLeJa/UnParent Objects %&X")]
    
    public static void UnParentAll()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            go.transform.parent = null;
            Debug.Log("Das Objekt "+ go.name +" wurde unparented");
        }
    }
    public static void parenting ( )
    {
        HexCheck();
        if (Hex == null)
        {
            Debug.Log("Beweg mal das Objekt höher oder überhaupt in die Nähe von Hexes");
            return;
        }
        SetParent(Hex);
    }
    public static void  SetParent(GameObject newParent)
    {
        LevelObj.transform.parent = newParent.transform;
        if (newParent.transform.parent != null) Debug.Log("Das Objekt "+ LevelObj.name +" unter >>> " + LevelObj.transform.parent.name +" <<< platziert");
    }
    
    static void   HexCheck()
    {
        RaycastHit hit = new RaycastHit();
        float sphereCastRadius = 0.1f,
            sphereCastRadiusStop = 500,
            sphereCastRadiusDown = sphereCastRadius,
            sphereCastRadiusUp = sphereCastRadius,
            maxRayDistance = 1000;
        while (Hex == null && sphereCastRadiusDown <sphereCastRadiusStop*0.5f)
        {
            if (Physics.SphereCast(LevelObj.transform.position, sphereCastRadiusDown,
                -LevelObj.transform.up, out hit, maxRayDistance, LayerMask.GetMask(Hexlayer)))
                Hex = hit.transform.gameObject;
        
            sphereCastRadiusDown++;
        }
        while (Hex == null && sphereCastRadiusUp < sphereCastRadiusStop*0.5f)
        {
            if (Physics.SphereCast(LevelObj.transform.position, sphereCastRadiusUp,
                LevelObj.transform.up, out hit, maxRayDistance,
                LayerMask.GetMask("Hex")))
                Hex = hit.transform.gameObject;
            
            sphereCastRadiusUp++;
        }
        while (Hex == null && sphereCastRadiusDown <sphereCastRadiusStop)
        {
            if (Physics.SphereCast(LevelObj.transform.position, sphereCastRadiusDown,
                -LevelObj.transform.up, out hit, maxRayDistance, LayerMask.GetMask(Hexlayer)))
                Hex = hit.transform.gameObject;
            
            sphereCastRadiusDown++;
        }
        while (Hex == null && sphereCastRadiusUp < sphereCastRadiusStop)
        {
            if (Physics.SphereCast(LevelObj.transform.position, sphereCastRadiusUp,
                LevelObj.transform.up, out hit, maxRayDistance,
                LayerMask.GetMask(Hexlayer)))
                Hex = hit.transform.gameObject;
            
            sphereCastRadiusUp++;
        }
    }
}