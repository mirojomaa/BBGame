using UnityEngine;
public class MissionStateFindMission : MonoBehaviour
{
    public void FindMission()
    {
        int missionIndex = 0;
        missionIndex = Random.Range(0, ReferenceLibrary.MissLib.Missions.Count);
        MissionManager.CurrentMission = ReferenceLibrary.MissLib.Missions[missionIndex];
        ReferenceLibrary.MissLib.Missions.RemoveAt(missionIndex);
    }
}