using UnityEngine;
[System.SerializableAttribute]
public class MissionInformation 
{
    public MissionType missionType;
    public MissionDifficulty missionDificulty;
    [Tooltip("Time in Seconds")]
    [HideInInspector] public float time = 180f;
    [HideInInspector] public float multiplicator = 1;
    [HideInInspector] public int Amount = 5;
    [Header("For Collect, Destroy and Bring")]
    public Item missionItem;
    public enum MissionDifficulty
    {
        easy, medium, hard
    }

    public enum MissionType
    {
        CollectItem, DestroyObjs, CollectPoints, BringFromAToB
    }

    public enum Item
    {
        none, CollectItem1, CollectItem2, Destroyable1, BringItem1, BringItem2
    }
}
