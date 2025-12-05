using UnityEngine;
[ExecuteInEditMode]
public class SpawnHexPropsManager : MonoBehaviour
{
    public static bool AllowEditorHexObjSpawn = true;
    public bool AllowEditorObjSpawn = true;
    void Update() =>AllowEditorHexObjSpawn = AllowEditorObjSpawn;
}