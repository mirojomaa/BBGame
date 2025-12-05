using UnityEngine;
public class MissionStateNoMissionsLeft : MonoBehaviour
{ 
    public void ReactiveMissions()
    {
        if (MissionManager.MissionRound >= 5) return;
        ReferenceLibrary.MissLib.CopyMissionLists();
        ReferenceLibrary.MissionMng.SwitchToNoMissionState();
    }
}