using System.Collections.Generic;
using UnityEngine;
public class MissionLibary : MonoBehaviour
{
    [SerializeField] List<MissionInformation> AllMissions = new List<MissionInformation>();
    [HideInInspector] public List<MissionInformation> Missions = new List<MissionInformation>();
    private void Awake()
    {
        MissionManager.MissionRound = 0;
        CopyMissionLists();
    }
    public void CopyMissionLists()
    {
        Missions.Clear();
        MissionManager.MissionAmount = AllMissions.Count;
        MissionManager.CompletedMissions = 0;
        MissionManager.MissionRound++;
        foreach (MissionInformation mission in AllMissions) Missions.Add(mission);
    }
}