using UnityEngine;
public class MissionStatePrepareMission : MonoBehaviour
{ 
    public void PrepareMission()
    {
        switch (MissionManager.CurrentMission.missionType)
        {
            case MissionInformation.MissionType.CollectItem:
                CalculateCollectItemValues();
                PrepareCollectItem();
                ActivateCollectItemUI();
                break;
            case MissionInformation.MissionType.DestroyObjs:
                CalculateDestroyObjValues();
                PrepareDestroyObj();
                ActivateDestoryObjUi();
                break;
            case MissionInformation.MissionType.CollectPoints:
                CalculateCollectPointsValues();
                PrepareCollectPoints();
                ActivateCollectPointsUI();
                break;
            case MissionInformation.MissionType.BringFromAToB:
                PrepareBringItem();
                ActivateBringItemUI();
                break;
            default:
                break;
        }
    }
    int RandomInt(int min, int max) =>(Random.Range(min, max));
    #region Collect Item
    void CalculateCollectItemValues()
    {
        if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.easy)
        {
            MissionManager.CurrentMission.Amount = RandomInt(2, 5);
            MissionManager.CurrentMission.time = 80f + (MissionManager.CurrentMission.Amount * 40);
            MissionManager.CurrentMission.multiplicator = MissionManager.CurrentMission.Amount * 0.04f;
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.medium)
        {
            MissionManager.CurrentMission.Amount = RandomInt(5, 10);
            MissionManager.CurrentMission.time = 60f + (MissionManager.CurrentMission.Amount * 30);
            MissionManager.CurrentMission.multiplicator = MissionManager.CurrentMission.Amount * 0.05f;
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.hard)
        {
            MissionManager.CurrentMission.Amount = RandomInt(10, 15);
            MissionManager.CurrentMission.time = 40f + (MissionManager.CurrentMission.Amount * 15);
            MissionManager.CurrentMission.multiplicator = MissionManager.CurrentMission.Amount * 0.08f;
        }
    }
    void PrepareCollectItem()
    {
        ReferenceLibrary.ItemSpawner.SpawnCollectItem();
        MissionManager.MissionTimeLeft = MissionManager.CurrentMission.time;
        MissionManager.Progress = 0;
    }
    void ActivateCollectItemUI()
    {
        ReferenceLibrary.UIMng.ActivateBasicMissionUI();
        ReferenceLibrary.UIMng.ActivateCollectItemUI();
    }
    #endregion
    #region Destroy Obj
    void CalculateDestroyObjValues()
    {
        if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.easy)
        {
            MissionManager.CurrentMission.Amount = RandomInt(2, 5);
            MissionManager.CurrentMission.time = 100f + (MissionManager.CurrentMission.Amount * 40);
            MissionManager.CurrentMission.multiplicator = MissionManager.CurrentMission.Amount * 0.04f;
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.medium)
        {
            MissionManager.CurrentMission.Amount = RandomInt(5, 10);
            MissionManager.CurrentMission.time = 60f + (MissionManager.CurrentMission.Amount * 25);
            MissionManager.CurrentMission.multiplicator = MissionManager.CurrentMission.Amount * 0.05f;
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.hard)
        {
            MissionManager.CurrentMission.Amount = RandomInt(10, 15);
            MissionManager.CurrentMission.time = 40f + (MissionManager.CurrentMission.Amount * 15);
            MissionManager.CurrentMission.multiplicator = MissionManager.CurrentMission.Amount * 0.07f;
        }
    }
    void PrepareDestroyObj()
    {
        MissionManager.MissionTimeLeft = MissionManager.CurrentMission.time;
        MissionManager.Progress = 0;
        //ReferenceLibary.ItemSpawner.SpawnDestroyObj();
    }
    void ActivateDestoryObjUi()
    {
        ReferenceLibrary.UIMng.ActivateBasicMissionUI();
        ReferenceLibrary.UIMng.ActivateDestroyObjUI();
    }
    #endregion
    #region CollectPoints
    void CalculateCollectPointsValues()
    {
        if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.easy)
        {
            MissionManager.CurrentMission.Amount = 2000; //* Mathf.RoundToInt(ScoreManager.CurrentMultiplicator)
            MissionManager.CurrentMission.time = 80f;
            MissionManager.CurrentMission.multiplicator = 0.5f; //0.3f
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.medium)
        {
            MissionManager.CurrentMission.Amount = 5000;
            MissionManager.CurrentMission.time = 170;
            MissionManager.CurrentMission.multiplicator = 1f; //0.5f
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.hard)
        {
            MissionManager.CurrentMission.Amount = 9000; // NICHT H�HER ALS 9999 GEHEN; SONST MACHT DAS UI NICHT MIT
            MissionManager.CurrentMission.time = 200f;
            MissionManager.CurrentMission.multiplicator = 1.5f; //0.7f
        }
    }
    void PrepareCollectPoints()
    {
        MissionManager.MissionTimeLeft = MissionManager.CurrentMission.time;
        MissionManager.Progress = 0;
        float startPoints = ScoreManager.CurrentScore + 0;
        MissionManager.EndPoints = startPoints + MissionManager.CurrentMission.Amount;
    }
    void ActivateCollectPointsUI()
    {
        ReferenceLibrary.UIMng.ActivateBasicMissionUI();
        ReferenceLibrary.UIMng.ActivateCollectPointsUI();
    }
    #endregion
    #region Bring Item
    void CalculateBringItemValues()
    {
        CalculateDistance();
        Debug.Log(ReferenceLibrary.MissionMng.BringItemDistance);
        if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.easy)
        {
            MissionManager.CurrentMission.time = 100 + ReferenceLibrary.MissionMng.BringItemDistance * 1.6f;
            MissionManager.CurrentMission.multiplicator = 0.5f; //0.3f
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.medium)
        {
            MissionManager.CurrentMission.time = 60 + ReferenceLibrary.MissionMng.BringItemDistance * 1.4f;
            MissionManager.CurrentMission.multiplicator = 1f; //0.5f
        }
        else if (MissionManager.CurrentMission.missionDificulty == MissionInformation.MissionDifficulty.hard)
        {
            MissionManager.CurrentMission.time = 40 + ReferenceLibrary.MissionMng.BringItemDistance * 1.2f;
            MissionManager.CurrentMission.multiplicator = 1.5f; //0.7f
        }
    }
    void CalculateDistance()
    {
       Vector3 Verbindungsvector =  MissionItemSpawner.CurrentMissionItems[0].transform.position - MissionItemSpawner.CurrentMissionItems[1].transform.position;
       ReferenceLibrary.MissionMng.BringItemDistance = Mathf.Sqrt(Mathf.Pow(Verbindungsvector.x, 2) + Mathf.Pow(Verbindungsvector.y, 2) + Mathf.Pow(Verbindungsvector.z, 2));
    }
    void PrepareBringItem()
    {
        MissionManager.CurrentMission.Amount = 1;
        MissionManager.Progress = 0;
        ReferenceLibrary.ItemSpawner.SpawnBringItemAndGoal();
        CalculateBringItemValues();
        MissionManager.MissionTimeLeft = MissionManager.CurrentMission.time;
        MissionManager.ItemCollected = false;
        MissionManager.ItemDelivered = false;
    }
    void ActivateBringItemUI()
    {
        ReferenceLibrary.UIMng.ActivateBasicMissionUI();
        ReferenceLibrary.UIMng.ActivateBringItemUI();
    }
    #endregion
}