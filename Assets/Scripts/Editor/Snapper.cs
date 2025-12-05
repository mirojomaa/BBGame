using UnityEditor;
using UnityEngine;
public static class Snapper
{
    private const string UNDO_STR_SNAP = "snap Objects";
    [MenuItem("HaMiLeJa/Snap Selected Object", isValidateFunction: true)]
    public static bool SnapTheThingsValidate() => Selection.gameObjects.Length > 0;
    [MenuItem("HaMiLeJa/Snap Selected Object")]
    public static void SnapTheThings()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, UNDO_STR_SNAP);
            go.transform.position = go.transform.position.Round();
        }
    }
    public static Vector3 Round(this Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        /*
        if(v.z % 1.73 > 0.865)
            v.z = Mathf.Ceil(v.z);
        if(v.z % 1.73 <= 0.865)
            v.z = Mathf.Floor(v.z);
            */
        return v;
    }
}