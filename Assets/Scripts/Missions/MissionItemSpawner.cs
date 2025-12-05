using System.Collections.Generic;
using UnityEngine;

public class MissionItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject CollectItem1;
    [SerializeField] GameObject CollectItem2;
    [SerializeField] GameObject Destroyable;
    [SerializeField] GameObject BringItem1;
    [SerializeField] GameObject BringItem2;
    [SerializeField] GameObject BringItemGoal;
    [SerializeField] GameObject currentItem;
    [SerializeField] List<GameObject> AllItemSpawnPositions;
    [SerializeField] List<GameObject> PositionListInUse;
    int spawnAmount = 0;

    public static List<GameObject> CurrentMissionItems = new List<GameObject>();
    private void Awake()
    {
        AllItemSpawnPositions = new List<GameObject>(GameObject.FindGameObjectsWithTag("ItemPos"));
    }
    void PrepareSpawn()
    {
        PositionListInUse.Clear();

        foreach (GameObject pos in AllItemSpawnPositions)
            PositionListInUse.Add(pos);

        currentItem = CollectItem1; //Sonst meckert er meh
        ClearCurrentMissionItemList();
    }
    public static void ClearCurrentMissionItemList()
    {
        if (CurrentMissionItems.Count == 0)
        {
            CurrentMissionItems.Clear();
            return;
        }
        for (int i = CurrentMissionItems.Count - 1; i >= 0; i--) Destroy(CurrentMissionItems[i]);
        CurrentMissionItems.Clear();
    }
    void Spawner(GameObject itemToSpawn)
    {
        int random = Random.Range(0, PositionListInUse.Count);
        Vector3 itemPos = PositionListInUse[random].transform.position;
        Transform parentHex = PositionListInUse[random].transform.parent;
        GameObject item = Instantiate(itemToSpawn, itemPos, Quaternion.identity);
        item.transform.parent = parentHex.transform;
        CurrentMissionItems.Add(item);
        PositionListInUse.RemoveAt(random);
    }
    void SpawnAmount()
    {
        spawnAmount = MissionManager.CurrentMission.Amount;
        if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.easy)
            spawnAmount = MissionManager.CurrentMission.Amount * 4;
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.medium)
            spawnAmount = MissionManager.CurrentMission.Amount * 3;
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.hard)
            spawnAmount = MissionManager.CurrentMission.Amount * 2;
    }

    #region Collect Item
    public void SpawnCollectItem()
    {
        PrepareSpawn();
        SpawnAmount();
        FindCollectItem();
        CollectItemSpawner();
    }

    void FindCollectItem()
    {
        if (MissionManager.CurrentMission.missionItem == MissionInformation.Item.CollectItem1) currentItem = CollectItem1;
        else if (MissionManager.CurrentMission.missionItem == MissionInformation.Item.CollectItem2) currentItem = CollectItem2;
        else currentItem = CollectItem1;
    }
    void CollectItemSpawner()
    {
        if (spawnAmount >= AllItemSpawnPositions.Count) spawnAmount = AllItemSpawnPositions.Count;
        for (int i = 0; i < spawnAmount; i++) Spawner(currentItem); //i <= MissionManager.CurrentMission.Amount - 1 - spawnCounter;
    }
    #endregion
    
    #region Destroy Obj
    public void SpawnDestroyObj()
    {
        PrepareSpawn();
        SpawnAmount();
        FindDestroyObj();
        DestroyObjSpawner();
    }
    void FindDestroyObj() =>currentItem = Destroyable;
    void DestroyObjSpawner()
    {
        for (int i = 0; i < MissionManager.CurrentMission.Amount; i++) Spawner(currentItem);//i <= MissionManager.CurrentMission.Amount - 1 - spawnCounter;
    }
    #endregion
    #region Bring Item
    public void SpawnBringItemAndGoal()
    {
        PrepareSpawn();
        FindBringItem();
        BringItemItemSpawner();
        BringItemGoalSpawner();
    }
    void FindBringItem()
    {
        if (MissionManager.CurrentMission.missionItem == MissionInformation.Item.BringItem1) currentItem = BringItem1;
        else if (MissionManager.CurrentMission.missionItem == MissionInformation.Item.BringItem2) currentItem = BringItem2;
    }
    void BringItemItemSpawner() => Spawner(currentItem);
    void BringItemGoalSpawner() => Spawner(BringItemGoal);
    #endregion
}