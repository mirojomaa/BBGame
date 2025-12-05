using UnityEditor;
using UnityEngine;
[ExecuteAlways]
public class SpawnHexCollectableInEditor : MonoBehaviour
{
    public GameObject ObjectToSpawn, CurrentItem;
    [SerializeField] Hex myHex;
    [SerializeField] GameObject MyProps;
    public bool DebugActiveObj, DebugIsRunning;
    void Update()
    {
        if (!Application.isPlaying) return;
        DebugIsRunning = true;
        EditModeSpawnAndDeletion();
    }
    #region Editor
    void EditModeSpawnAndDeletion()
    {
        if (myHex.hexType == HexType.DefaultCollectable)
        {
            if (CheckForSpawnAlloance()) DebugActiveObj = true; 
            SpawnObjectInEditMode();
        }
        else if (myHex.hexType != HexType.DefaultCollectable && CurrentItem != null) DestroyImmediate(CurrentItem);
        DebugActiveObj = false;
    }
    bool CheckForSpawnAlloance()
    {
        if (MyProps.GetComponentInChildren<Collectable>())
        {
            CurrentItem = MyProps.GetComponentInChildren<Collectable>().gameObject;
            return false;
        }
        return true;
    }
    void SpawnObjectInEditMode() => spawnObjectWithPrefabConnection(4, CurrentItem, gameObject, ObjectToSpawn, MyProps);
    #endregion
    public static void spawnObjectWithPrefabConnection (float y, GameObject Item, GameObject hex, GameObject ObjectToSpawn, GameObject MyProps)
    {
        Vector3 position = new Vector3(hex.transform.position.x, hex.transform.position.y + y, hex.transform.position.z);
#if UNITY_EDITOR
        Item = (GameObject)PrefabUtility.InstantiatePrefab(ObjectToSpawn);
#endif
        Item.transform.position = position;
        Item.transform.rotation = Quaternion.identity;
        Item.transform.parent = MyProps.transform;
    }
}
