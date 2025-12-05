using UnityEngine;
public class MissionStateActiveMission : MonoBehaviour
{
    public void UpdateActiveMission()
     {
        switch (MissionManager.CurrentMission.missionType)
        {
            case MissionInformation.MissionType.CollectItem: UpdateCollectItem(); break;
            case MissionInformation.MissionType.DestroyObjs: UpdateDestroyObj(); break;
            case MissionInformation.MissionType.CollectPoints: UpdateCollectPointsProgress(); UpdateCollectPoints(); break;
            case MissionInformation.MissionType.BringFromAToB: UpdateBringItem(); break;
            default: break;
        }
     }
    void MissionTimer() => MissionManager.MissionTimeLeft -= Time.deltaTime;
    void CheckForEnd()
    {
        if (MissionManager.Progress == MissionManager.CurrentMission.Amount)
        {
            ReferenceLibrary.MissionMng.SwitchToCompletedMissionState();
            MissionManager.CompletedMissions++;
        }
        else if (MissionManager.MissionTimeLeft <= 0) ReferenceLibrary.MissionMng.SwitchToUncompletedMissionState();
    }
    #region Collect Item
    void UpdateCollectItem()
    {
        MissionTimer();
        ReferenceLibrary.UIMng.UpdateBasicMissionUI();
        ReferenceLibrary.UIMng.UpdateCollectItemUI();
        CheckForEnd();
    }
    public void ItemCollected(GameObject item)
    {
        MissionManager.Progress++;
        MissionItemSpawner.CurrentMissionItems.Remove(item);
        Destroy(item);
        if (MissionManager.Progress == MissionManager.CurrentMission.Amount) return;
        ReferenceLibrary.AudMng.PlayMissionSound(ReferenceLibrary.MissionMng.missionCollectalbeClip, ReferenceLibrary.MissionMng.missionCollectalbeGroup);
    }
    #endregion
    #region Destroy Objs
    void UpdateDestroyObj()
    {
        MissionTimer();
        ReferenceLibrary.UIMng.UpdateBasicMissionUI();
        ReferenceLibrary.UIMng.UpdateDestroObjUI();
        CheckForEnd();
    }
    public static void ObjDestroyed() => MissionManager.Progress++;
    #endregion
    #region Collect Points
    void UpdateCollectPoints()
    {
        MissionTimer();
        ReferenceLibrary.UIMng.UpdateBasicMissionUI();
        ReferenceLibrary.UIMng.UpdateCollectPointsUI();
        CheckForCollectPointsEnd();
    }
    void UpdateCollectPointsProgress()
    {
        float differenz = MissionManager.EndPoints - ScoreManager.CurrentScore;
        MissionManager.Progress = MissionManager.CurrentMission.Amount - differenz;
    }
    void CheckForCollectPointsEnd()
    {
        if(ScoreManager.CurrentScore >= MissionManager.EndPoints)
        {
            MissionManager.CompletedMissions++;
            ReferenceLibrary.MissionMng.SwitchToCompletedMissionState();
        }
        else if (MissionManager.MissionTimeLeft <= 0)
        {
            ReferenceLibrary.MissionMng.SwitchToUncompletedMissionState();
        }
    }
    #endregion
    #region Bring Item
    void UpdateBringItem()
    {
        MissionTimer();
        ReferenceLibrary.UIMng.UpdateBasicMissionUI();
        CheckForBringItemEnd();
    }
    void CheckForBringItemEnd()
    {
        if (MissionManager.ItemDelivered == true)
        {
            MissionManager.CompletedMissions++;
            Debug.Log("ItemDelivered CheckForEnd");
            ReferenceLibrary.MissionMng.SwitchToCompletedMissionState();
        }
        else if (MissionManager.MissionTimeLeft <= 0) ReferenceLibrary.MissionMng.SwitchToUncompletedMissionState();
    }
    public void BringItemCollected(GameObject item)
    {
        MissionManager.ItemCollected = true;
        MissionItemSpawner.CurrentMissionItems.Remove(item);
        Destroy(item);
        ReferenceLibrary.UIMng.ChangeProgressState1();
    }
    public void BringItemDelivered(GameObject item)
    {
       if(MissionManager.ItemCollected == true)
       {
            ReferenceLibrary.UIMng.ChangeProgressState2();
            Debug.Log("2");
            MissionManager.ItemDelivered = true;
            MissionItemSpawner.CurrentMissionItems.Remove(item);
            Destroy(item);
       }
    }
    #endregion
}