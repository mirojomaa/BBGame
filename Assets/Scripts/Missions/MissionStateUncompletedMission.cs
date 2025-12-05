using UnityEngine;
public class MissionStateUncompletedMission : MonoBehaviour
{
    public void UpdateUncompletedMission()
    {
        switch (MissionManager.CurrentMission.missionType)
        {
            case MissionInformation.MissionType.CollectItem: UpdateCollectItem(); break;
            case MissionInformation.MissionType.DestroyObjs: UpdateDestroyObj(); break;
            case MissionInformation.MissionType.CollectPoints: UpdateCollectPoints(); break;
            case MissionInformation.MissionType.BringFromAToB: UpdateBringItem(); break;
            default: break;
        }
        Debug.Log("MissionUncomplete"); //Mission evt zur AllMission Liste zurückadden, wenn wir wollen, dass player jede mission erfüllen müssen //Effects, Sound
    }
    #region Collect Item
    void UpdateCollectItem()
    {
        ReferenceLibrary.UIMng.DeactivateCollectItemUI();
        ReferenceLibrary.UIMng.DeactivateBasicMissionUI();
        RemoveCollectables();
    }
    void RemoveCollectables()=> MissionItemSpawner.ClearCurrentMissionItemList();
    #endregion
    #region Destroy Obj
    void UpdateDestroyObj()
    {
        ReferenceLibrary.UIMng.DeactivateDestroyObjUI();
        ReferenceLibrary.UIMng.DeactivateBasicMissionUI();
    }
    #endregion
    #region Collect Points
    void UpdateCollectPoints()
    {
        ReferenceLibrary.UIMng.DeactivateBasicMissionUI();
        ReferenceLibrary.UIMng.DeactivateCollectPointsUI();
    }
    #endregion
    #region Bring Item
    void UpdateBringItem()
    {
        ReferenceLibrary.UIMng.DeactivateBasicMissionUI();
        ReferenceLibrary.UIMng.DeactivateBringItemUI();
    }
    #endregion
}