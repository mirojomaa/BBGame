using UnityEngine;
using UnityEngine.Audio;
public class MissionManager : MonoBehaviour
{
    public static int MissionAmount, CompletedMissions, MissionRound = 0;
    [SerializeField] private MissionStateNoMission NoMissionMissionState;
    [SerializeField] private MissionStateFindMission FindMissionState;
    [SerializeField] private MissionStatePrepareMission PrepareMissionState;
    [SerializeField] public MissionStateActiveMission ActiveMissionState;
    [SerializeField] private  MissionStateCompletedMission CompletedMissionState;
    [SerializeField] private  MissionStateUncompletedMission UncompletedMissionState;
    [SerializeField] public MissionStateNoMissionsLeft NoMissionLeft;
    [SerializeField] public static MissionInformation CurrentMission;
    bool lastMissionSuccesfull = false;
    // For Active State
    public static float MissionTimeLeft, Progress, EndPoints;  //For CollectPoints
    //For Bring Item
    public float BringItemDistance = 0;
    public static bool ItemCollected = false, ItemDelivered = false,
    //For NoMissionsLeft State
    StartNewMissionRoundAllowed = false;
    [Header("Audio")]
    [SerializeField] AudioClip newMissionClip;
    [SerializeField] AudioMixerGroup newMissionGroup;
    [SerializeField] AudioClip successfullClip;
    [SerializeField] AudioMixerGroup successfullGroup;
    [SerializeField] AudioClip unsuccesfullClip;
    [SerializeField] AudioMixerGroup unsuccessfullGroup;
   [SerializeField] public AudioClip missionCollectalbeClip;
   [SerializeField] public AudioMixerGroup missionCollectalbeGroup;
    static MissionState missionState = MissionState.noMission;
    enum MissionState
    {
        noMission,
        findMission,
        prepareMission,
        activeMission,
        CompletedMission,
        UncompletedMission,
        noMissionsLeft,
        transitionCase
    }

    #if UNITY_EDITOR
    [NaughtyAttributes.Button()] private void FillComponents()
    {
        NoMissionMissionState = GetComponentInChildren<MissionStateNoMission>();
        FindMissionState = GetComponentInChildren<MissionStateFindMission>();
        PrepareMissionState = GetComponentInChildren<MissionStatePrepareMission>();
        ActiveMissionState = GetComponentInChildren<MissionStateActiveMission>();
        CompletedMissionState = GetComponentInChildren<MissionStateCompletedMission>();
        UncompletedMissionState = GetComponentInChildren<MissionStateUncompletedMission>();
        NoMissionLeft = GetComponentInChildren<MissionStateNoMissionsLeft>();
    }
    #endif
    void Start() => missionState = MissionState.noMission;

    void Update()
    {
        if (GameStateManager.gameState == GameStateManager.GameState.Start) return;
        if (GameStateManager.GameOver) return;
        switch (missionState)
        {
            case MissionState.noMission: NoMissionMissionState.UpdateNoMission(); break;
            case MissionState.findMission: FindMissionState.FindMission(); SwitchToPrepareMissionState(); break;
            case MissionState.prepareMission: PrepareMissionState.PrepareMission(); SwitchToActiveMissionState(); break;
            case MissionState.activeMission: ActiveMissionState.UpdateActiveMission(); break;
            case MissionState.CompletedMission: CompletedMissionState.UpdateCompletedMission(); lastMissionSuccesfull = true;
                                                 CheckForAllMissionsDone(); CollectableManager.OnRespawnCollectables?.Invoke(); break;
            case MissionState.UncompletedMission: UncompletedMissionState.UpdateUncompletedMission(); lastMissionSuccesfull = false;
                                                  CheckForAllMissionsDone(); CollectableManager.OnRespawnCollectables?.Invoke(); break;
            case MissionState.noMissionsLeft: ReferenceLibrary.WinconMng.CheckForWinConMission(); missionState = MissionState.transitionCase; break;
            case MissionState.transitionCase: CheckForReactivation(); break;
            default: break;
        }
        /*
        if(Input.GetKeyDown(KeyCode.A))audioMng.PlayMissionSound(newMissionClip, newMissionGroup);
        if (Input.GetKeyDown(KeyCode.B))audioMng.PlayMissionSound(successfullClip, successfullGroup);
        if (Input.GetKeyDown(KeyCode.C))audioMng.PlayMissionSound(unsuccesfullClip, unsuccessfullGroup);
        if (Input.GetKeyDown(KeyCode.D)) audioMng.PlayMissionSound(missionCollectalbeClip, missionCollectalbeGroup);
        */
    }
    #region Switch State
    void SwitchToActiveMissionState()
    {
        ReferenceLibrary.AudMng.PlayMissionSound(newMissionClip, newMissionGroup);
        missionState = MissionState.activeMission;
    }
    void SwitchToPrepareMissionState() =>missionState = MissionState.prepareMission;
    public void SwitchToCompletedMissionState() =>missionState = MissionState.CompletedMission;
    public void SwitchToUncompletedMissionState() => missionState = MissionState.UncompletedMission;
    public void SwitchToNoMissionState()
    {
        ReferenceLibrary.UIMng.ActivateNoMissionUI();
        missionState = MissionState.noMission;
    }
    public void SwitchToFindMissionState()=>missionState = MissionState.findMission;
    void SwitchToNoMissionLeftState() => missionState = MissionState.noMissionsLeft;
        #endregion
    public void CheckForAllMissionsDone()
    {
        if(ReferenceLibrary.MissLib.Missions.Count == 0) SwitchToNoMissionLeftState(); // Alle Missionen wurden durchlaufen
        else
        {
            SwitchToNoMissionState();
            if(lastMissionSuccesfull) ReferenceLibrary.AudMng.PlayMissionSound(unsuccesfullClip, unsuccessfullGroup);
            else ReferenceLibrary.AudMng.PlayMissionSound(missionCollectalbeClip, missionCollectalbeGroup);
        }
    }
    void CheckForReactivation() //Used, when The first Mission Round is over
    {
        if(StartNewMissionRoundAllowed)
        {
            StartNewMissionRoundAllowed = false;
            NoMissionLeft.ReactiveMissions();
        }
    }
}