using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
public class ReferenceLibrary : MonoBehaviour
{
    //----------------   ReferenceManager -------------------//
    public static List<Object> hasAllTheSingleHandedReferences;
    [SerializeField]private List<Object> hasAllTheReferenceFromRefLibrary;
    //----------------   References in Level -------------------//
    // Player
    [SerializeField] private  GameObject PlayerRef;
    [SerializeField] private Rigidbody PlayerRbRef;
    [SerializeField] private Transform PlayerTransformRef;
    [SerializeField] private  PlayerSuperDash SuperDashRef;
    [SerializeField] private  ShadowDash ShadowDashPlRef;
    [SerializeField] private  PlayerBoost DashRef;
    [SerializeField] private DownDash DownDashPlRef;
    [SerializeField] private  PlayerMovement PlayerMovRef;
    [SerializeField] private  HexMovements HexMovRef;
    [SerializeField] private  PlayerParticle ParticlesPlayerRef;
    // Missions
    [SerializeField] private MissionManager MissionMngRef;
    [SerializeField] private MissionItemSpawner ItemSpawnerRef;
    [SerializeField] private MissionLibary MissLibRef;
    //Audio
    [SerializeField] private UIManager UIMngRef;
    [SerializeField] private CollectableManager ColMngRef;
    [SerializeField] private GameManager GameMngRef;
    [SerializeField] private ScoreManager ScoreMngRef;
    [SerializeField] private WindconditionManager WinconMngRef;
    [SerializeField] private AudioManager AudMngRef;
    [SerializeField] private GameStateManager GameStateMngRef;
    [SerializeField] private EnergyManager EnergyMngRef;
    
    //----------------   statics  -------------------//
    // Player
    public static GameObject Player;
    public static Rigidbody PlayerRb;
    public static Transform PlayerTransform;
    public static Vector3 PlayerPosition;
    public static string PlayerTag;
    public static PlayerSuperDash SuperDash;
    public static ShadowDash ShadowDashPl;
    public static PlayerBoost Dash;
    public static DownDash DownDashPl;
    public static PlayerMovement PlayerMov;
    public static HexMovements HexMov;
    public static PlayerParticle ParticlesPlayer;
    // Hex

    // Missions
    public static MissionManager MissionMng;
    public static MissionItemSpawner ItemSpawner;
    public static MissionLibary MissLib;

    //Audio  //public static AudioClipsHexes audioClipHexes;
    public static UIManager UIMng;
    public static CollectableManager ColMng;
    public static GameManager GameMng;
    public static ScoreManager ScoreMng;
    public static WindconditionManager WinconMng;
    public static AudioManager AudMng;
    public static GameStateManager GameStateMng;
    public static EnergyManager EnergyMng;
    
    //----------------   Unity Loops  -------------------//
    private void Awake()
    {
        AssignRefs();
        AddToReferenceNullList(hasAllTheReferenceFromRefLibrary);
    }
    private void Start() => NullEverything();
    private void Update()=>PlayerPosition = PlayerTransform.position;
    
    //----------------   Methods assign on Game Start -------------------//
    private void AssignRefs()
    {
        Player = PlayerRef;
        PlayerRb = PlayerRbRef;
        PlayerTransform = PlayerTransformRef;
        PlayerTag = Player.tag;
        SuperDash = SuperDashRef;
        ShadowDashPl = ShadowDashPlRef;
        Dash = DashRef;
        DownDashPl = DownDashPlRef;
        PlayerMov = PlayerMovRef;
        HexMov = HexMovRef;
        ParticlesPlayer = ParticlesPlayerRef;
        MissionMng = MissionMngRef;
        ItemSpawner = ItemSpawnerRef;
        MissLib = MissLibRef;
        UIMng = UIMngRef;
        ColMng = ColMngRef;
        GameMng = GameMngRef;
        ScoreMng = ScoreMngRef;
        WinconMng = WinconMngRef;
        AudMng = AudMngRef;
        GameStateMng = GameStateMngRef;
        EnergyMng = EnergyMngRef;
    }
    //----------------   Null  -------------------//
    void NullEverything()
    {
        if (hasAllTheSingleHandedReferences != null)
        {
        for (int i = 0; i < hasAllTheSingleHandedReferences.Count; i++) hasAllTheSingleHandedReferences[i] = null;
        hasAllTheSingleHandedReferences = null;
        }
    }
    //----------------   Subscribe to NullList -------------------//
    public static void AddToReferenceNullList(params Object[] objectListToNull)
    {
        foreach(Object obj in objectListToNull) hasAllTheSingleHandedReferences.Add(obj);
    } 
    public static void AddToReferenceNullList(List<Object> PreparedObjectNullList)
    {
        for (int i = 0; i < PreparedObjectNullList.Count; i++) PreparedObjectNullList[i] = null;
    }
#if UNITY_EDITOR
    //----------------   Premade Null List  -------------------//
    void AddToReferenceNullListOnlyForRefLibrary(params Object[] objectListToNull)
    {
        if (hasAllTheReferenceFromRefLibrary == null) return;
        hasAllTheReferenceFromRefLibrary.Clear();
        foreach(Object obj in objectListToNull)
            hasAllTheReferenceFromRefLibrary.Add(obj);
        objectListToNull = null;
    }
    [NaughtyAttributes.Button()] public void FillNullHashsetRefLibary() => 
        AddToReferenceNullListOnlyForRefLibrary(PlayerRef, PlayerRbRef, PlayerTransformRef, SuperDashRef, SuperDashRef, ShadowDashPlRef,
            DashRef, DownDashPlRef, PlayerMovRef, HexMovRef, ParticlesPlayerRef, MissionMngRef, ItemSpawnerRef,MissLibRef,
            UIMngRef, ColMngRef, GameMngRef, ScoreMngRef, WinconMngRef, AudMngRef, GameStateMngRef,EnergyMngRef );
    //----------------   Editor Assign  -------------------//
    [NaughtyAttributes.Button()] public void populateList()
    {
        //Player
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerRbRef = PlayerRef.GetComponent<Rigidbody>();
        PlayerTransformRef = PlayerRef.GetComponent<Transform>();
        SuperDashRef = PlayerRef.GetComponent<PlayerSuperDash>();
        ShadowDashPlRef = PlayerRef.GetComponent<ShadowDash>();
        DashRef = PlayerRef.GetComponent<PlayerBoost>();
        DownDashPlRef = PlayerRef.GetComponent<DownDash>();
        PlayerMovRef = PlayerRef.GetComponent<PlayerMovement>();
        HexMovRef = PlayerRef.GetComponent<HexMovements>();
        ParticlesPlayerRef = PlayerRef.GetComponentInChildren<PlayerParticle>();
        //Mission
        MissionMngRef = FindObjectOfType<MissionManager>();
        ItemSpawnerRef = FindObjectOfType<MissionItemSpawner>();
        MissLibRef = FindObjectOfType<MissionLibary>();
        //Audio  //audioClipHexes = AudioManager.Instance.gameObject.GetComponent<AudioClipsHexes>();
        UIMngRef = FindObjectOfType<UIManager>();
        ColMngRef = FindObjectOfType<CollectableManager>();
        GameMngRef = FindObjectOfType<GameManager>();
        ScoreMngRef = FindObjectOfType<ScoreManager>();
        WinconMngRef = FindObjectOfType<WindconditionManager>();
        AudMngRef = FindObjectOfType<AudioManager>();
        GameStateMngRef = FindObjectOfType<GameStateManager>();
        EnergyMngRef = FindObjectOfType<EnergyManager>();
    }
#endif
}