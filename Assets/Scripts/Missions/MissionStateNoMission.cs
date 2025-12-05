using UnityEngine;
public class MissionStateNoMission : MonoBehaviour
{
    private bool randomDurationSet = true;
    public static float duration = 50;
    private void Start() => ReferenceLibrary.UIMng.TimerUntilNexMission();

    public void UpdateNoMission()
    {
        if(!randomDurationSet)
        {
            randomDurationSet = true;
            int d = GetRandomDuration();
            duration = d;
        }
        duration -= Time.deltaTime;
        if(duration <= 0)
        {
            randomDurationSet = false;
            ReferenceLibrary.MissionMng.SwitchToFindMissionState();
            duration = 0;
            ReferenceLibrary.UIMng.DeactivateNoMissionUI();
        }
        else ReferenceLibrary.UIMng.TimerUntilNexMission();
    }
    int GetRandomDuration()
    {
        int i = Random.Range(40, 80);
        return i;
    }
}