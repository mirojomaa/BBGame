using UnityEngine;
public class WindconditionManager : MonoBehaviour
{
    public int PointsForWinCon = 20000;
    [HideInInspector] public int WinConPoints = 0;
    [SerializeField] CollectableHex CollectableHex;
    [SerializeField] AudioSource myAudioSource;
    void Awake() => WinConPoints = PlayerPrefs.GetInt("WinConPoints");
    private void Start()
    {
        if (CollectableHex == null) CollectableHex = FindObjectOfType<CollectableHex>();
        if (PlayerPrefs.GetInt("WinConHex") == 1) Destroy(CollectableHex.gameObject);
        // InstantiateWindConHexItem();
    }
    public void CheckForWinConMission() //Über MissionManager State NO MIssions left
    {
        Debug.Log("CheckForWinConMissions");
        if (MissionManager.MissionRound == 1)
        {
            if (MissionManager.CompletedMissions == MissionManager.MissionAmount) //Next Mission Round is initiaten in UI Coroutine
            { //ALLE MISSIONEN GESCHAFFT!
                if (PlayerPrefs.GetInt("WinConMissions") == 0)
                {
                    PlayerPrefs.SetInt("WinConMissions", 1);
                    StartCoroutine(ReferenceLibrary.UIMng.UIHexUnlocked());
                    PlaySound();
                }
                else
                {
                    StartCoroutine(ReferenceLibrary.UIMng.UIHexAlreadyUnlocked());
                    PlaySound(); //evt hier weglassen
                }
            }
            else
            {
                StartCoroutine(ReferenceLibrary.UIMng.UIHexUnlockedFailed());
                //Sound für Hex Unlocked Failed
            }
        }
        else MissionManager.StartNewMissionRoundAllowed = true;
    }
    public void CheckForWinConPoints(float value)
    {
        if(ScoreManager.CurrentScore >= PointsForWinCon)
        {
            WinConPoints = 1;
            ScoreManager.OnScoring -= CheckForWinConPoints;
            PlayerPrefs.SetInt("WinConPoints", 1);
            StartCoroutine(ReferenceLibrary.UIMng.WinConPointsCoroutine());
            PlaySound();
            Debug.Log("Win Con Points fullfilled");
        }
    }
    public void CheckForWinConHex()
    {
        PlayerPrefs.SetInt("WinConHex", 1);
        StartCoroutine(ReferenceLibrary.UIMng.WinConHexCoroutine());
        PlaySound();
        //Effect
        Destroy(CollectableHex.gameObject);
    }
    void InstantiateWindConHexItem()
    {
      //leer?
    }
    void PlaySound()
    {
        if (!myAudioSource.isPlaying) myAudioSource.Play();
    }
}