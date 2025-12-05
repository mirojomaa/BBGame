using System.Collections;
using UnityEngine;
public class Hex : MonoBehaviour
{
    // private MaterialPropertyBlock mpb;
    // private MaterialPropertyBlock MPB
    // {
    //     get
    //     {
    //         if (mpb == null) mpb = new MaterialPropertyBlock();
    //         return mpb;
    //     }
    // }
    // static readonly int hexIDEnableGlow = Shader.PropertyToID("_enableGlow");
    #region InspectorGlow
    private bool isGlowing;
    #endregion
    
   #region Inspector
   [SerializeField] public Renderer hexRenderer;
    // AudioManager audManager;
    //AudioClipsHexes audioClipHexes;
    //[SerializeField] AudioSource myAudioSource;
    //private HexCoordinates hexCoordinates;
    public HexType hexType;
    public CollectableType collectableType = CollectableType.Type1; //To Be Used :)
    public ushort hexSwapIndex;
    AudioClip clip;
    [SerializeField] public ParticleSystem EffectParticle;
    #endregion
    //public Vector3Int HexCoords => hexCoordinates.GetHexCoords();
    System.Action OnEffectHx;
    // wie weit kann die Unit laufen
    /*
    public int GetCost()
        => hexType switch
        {
            HexType.Water => 20,
            HexType.Default => 10,
            HexType.SlowDown => 15,
            _ => throw new Exception($"Hex of type {hexType} not supported")
        };
    
    public bool IsObstacle()
    {
        return this.hexType == HexType.Obstacle;
    }*/
   // private void Awake()
  //  {

        // CollectableManager.AllCollectables.Clear(); //Eben geadded

        //if (hexType == HexType.DefaultCollectable)
      //  {
            //colRef.HexScript = this;
            //colRef.HexScript = this.gameObject.GetComponent<Hex>();


            /* ORIGNAL CODE
            CollectableManager.AllCollectables.Add(this.gameObject, colRef); //Nur adden, werte angepasst wird sp�ter
            */
            //Debug.Log("Hex Added to All Collectables");
       // }
  //  }
  
    #region OnTriggerHexTypes
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag))
        {
            if (hexType == HexType.SlowDown) SlowDownStarter();
            if (hexType == HexType.Trampolin) TrampolinStarter();
            if (hexType == HexType.ChangeDirection) ChangeDirectionStarter();
            if ((hexType == HexType.BoostForward)) BoostForwardStarter();
            if ((hexType == HexType.BoostInDirection)) BoostInDirectionStarter();
            highlightObjects(false, 0, hexSwapIndex);
        }
    }
    #region MaterialSwap
    #endregion
 
    public void highlightObjects(bool isProp, byte HighlightType, ushort matSwapIndex)
    {
        StartCoroutine(EnableHighlightDelayed(isProp, HighlightType, matSwapIndex));
        StartCoroutine(DisableHighlightDelayed(isProp, HighlightType, matSwapIndex));
    }
    #endregion
    IEnumerator EnableHighlightDelayed(bool isProp, byte HighlightType, ushort matSwapIndex)
    {
        yield return new WaitForSeconds(Highlightmanager.GlowEnableDelayHex);
        ToggleGlow(true, isProp, HighlightType, matSwapIndex);
    }
    IEnumerator DisableHighlightDelayed(bool isProp, byte HighlightType, ushort matSwapIndex)
    {
        yield return new WaitForSeconds(Highlightmanager.GlowDisableDelayHex);
        ToggleGlow(false, isProp, HighlightType, matSwapIndex);
    }
    #region PathHighlight
    public void ToggleGlow(bool isProp, byte HighlightType, ushort matSwapIndex)
    {
        if (!isProp)
        {
            if (!isGlowing)
             {
                 hexRenderer.sharedMaterial = Highlightmanager.glowMaterialsStatic[HighlightType];
                 //MPB.SetInt(hexIDEnableGlow, 1);
                 // hexRenderer.SetPropertyBlock(MPB);
             }
            else
            {
                hexRenderer.sharedMaterial = Highlightmanager.HexMaterialsUseStatic[matSwapIndex];
                // MPB.SetInt(hexIDEnableGlow, 0);
                // hexRenderer.SetPropertyBlock(MPB);
            }
             isGlowing = !isGlowing;
        }
        if (isProp)
        {
            if (!isGlowing)  Highlightmanager.GlowHighlight(matSwapIndex, HighlightType);
            else Highlightmanager.DisableGlowHighlight(matSwapIndex);
            isGlowing = !isGlowing;
        }
    }
    public void ToggleGlow(bool state, bool isProp, byte HighlightType, ushort matSwapIndex)
    {
        if (isGlowing == state) return;
        isGlowing = !state;
        ToggleGlow(isProp, HighlightType, matSwapIndex);
    }
    #endregion
    #region HexEffects
    [SerializeField ]ScriptableHexEffects hexEffectsSettings;
    #region ChangeDirection
    //[SerializeField] private float ChangeDirectionBoostForce = 200f;
    //private float ChangeDirectionBoostDuration = 0.8f;
    [Header ("ChangeDirection")] 
    //private bool isChangingDirection = false;
    private Coroutine changeDirectionCoroutine;
   // private bool allowStartChangeDirection = true;
    public void ChangeDirectionStarter()
    {
        
        if (ReferenceLibrary.GameMng.AllowHexEffects == false) return;
        ReferenceLibrary.GameMng.ChangeDirectionCounter++;
        if (ReferenceLibrary.GameMng.AllowChangeDirection == false) return;
        //  if (allowStartChangeDirection == false) return;
        //allowStartChangeDirection = false;
        ReferenceLibrary.PlayerRb.velocity = ReferenceLibrary.PlayerRb.velocity * -1;
        //  playerMov.OnChangeDirectionHex = true;
        StartCoroutine(MultiplicatorModificationOverTime());
        PlaySound();
        //  if (changeDirectionCoroutine != null)
        //     StopCoroutine(changeDirectionCoroutine);
        // isChangingDirection = true;
        //  changeDirectionCoroutine = StartCoroutine(ChangeDirectionCoroutine());
    }
    #region NO Use: old Coroutine
    /*
    private IEnumerator ChangeDirectionCoroutine()
    {
        OnEffectHex?.Invoke();
        playerRb.velocity = playerRb.velocity * -1;
        yield return new WaitForSeconds(0.5f);
        playerMov.OnChangeDirectionHex = true;
        //isChangingDirection = false;
        yield return null;
        playerMov.OnChangeDirectionHex = false;
    }
    */
    #endregion
    /*
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == Player)
        {
            allowStartChangeDirection = true;
        } 
    }
    */
    #endregion
    #region SlowDown
   // [Header("SlowDown")]
    //[SerializeField] private AnimationCurve slowDownCurve;
    //[SerializeField] private float SlowDownForce = 400f;
   // private float SlowDownDuration = 0.4f;
   // private bool IsSlowingDown = false; //used to lock other boosts
    //private Coroutine slowDownCoroutine;
    //[SerializeField] float SlowDownValue = 0.99f;
    public void SlowDownStarter()
    {
        if (ReferenceLibrary.GameMng.AllowHexEffects == false) return;
        ReferenceLibrary.GameMng.AllowMovement = false;
        if (EffectParticle != null) EffectParticle.Play();
        ReferenceLibrary.HexMov.SlowDownTimer = 0;
        ReferenceLibrary.HexMov.OnSlowDownHex = true;
        StartCoroutine(Coroutine_ChangeConstSpeedOverTime());
        ReferenceLibrary.PlayerRb.velocity = ReferenceLibrary.PlayerRb.velocity * 0.4f;
        StartCoroutine(MultiplicatorModificationOverTime());
        PlaySound(); //Sound
        /*
        if (slowDownCoroutine != null)
            StopCoroutine(slowDownCoroutine);
        slowDownCoroutine = StartCoroutine(SlowDownCoroutine());
        */
    }
    IEnumerator Coroutine_ChangeConstSpeedOverTime()
    {
        ReferenceLibrary.PlayerMov.constspeed = 30;
        yield return new WaitForSeconds(2f);
        while (ReferenceLibrary.PlayerMov.constspeed != ReferenceLibrary.PlayerMov.originalContspeed)
        {
            ReferenceLibrary.PlayerMov.constspeed += 1;
            yield return new WaitForFixedUpdate();
        }
        if (ReferenceLibrary.PlayerMov.constspeed > ReferenceLibrary.PlayerMov.originalContspeed)
            ReferenceLibrary.PlayerMov.constspeed = ReferenceLibrary.PlayerMov.originalContspeed;
        yield return null;
    }
    #region NO use Old Coroutine
    /*
    private IEnumerator SlowDownCoroutine()
    {
        OnEffectHex?.Invoke();
        float t = 0;
        // Vector3 halfVelocity = velocity * 0.5f;
        playerRb.velocity = playerRb.velocity / 2;
        while (t < SlowDownDuration)
        {
            if (gameMng.AllowHexEffects == false) break;
            t += Time.deltaTime;
            float curveValue = slowDownCurve.Evaluate(t);
            playerRb.velocity *= 0.99f; // *Time.deltaTime
            yield return null;
        }
        IsSlowingDown = false;
    }
    */
    #endregion
    #endregion
    #region BoostForward
    [Header("BoostForward")]
    [Range (30f, 70f)] [SerializeField] private float forwardForce = 50f; //Range
   // private float BoostForwardDuration = 0.4f;
   // public bool IsHexForwardBoosting = false; //used to lock other boosts
    private Coroutine hexBoostForwardCoroutine;
   // [SerializeField] private AnimationCurve boostCurve;
   public void BoostForwardStarter()
    {
        if (ReferenceLibrary.GameMng.AllowHexEffects == false) return;
        ReferenceLibrary.GameMng.BoostForwardCounter++;
        if (ReferenceLibrary.GameMng.AllowBoostForward == false) return;
        ReferenceLibrary.HexMov.BoostForwardTimer = 0;
        ReferenceLibrary.HexMov.CurrentHexFowardForce = forwardForce;
        ReferenceLibrary.HexMov.OnBoostForwardHex = true;
        StartCoroutine(MultiplicatorModificationOverTime());
        PlaySound();
        // if (hexBoostForwardCoroutine != null)
        //   StopCoroutine(hexBoostForwardCoroutine);
        // hexBoostForwardCoroutine = StartCoroutine(HexBoostForwardCoroutine());
    }
   #region NoUse old Coroutine;
    /*
    private IEnumerator HexBoostForwardCoroutine()
    {
        Debug.Log("BoostForward");
        OnEffectHex?.Invoke();
        float t = 0;
        playerMov.OnBoostForwardHex = true;
        playerMov.ForwardDirection = this.playerRb.velocity;
        while (t < BoostForwardDuration)
        {
            if (gameMng.AllowHexEffects == false) break;
            t += Time.deltaTime;
            //float curveValue = boostCurve.Evaluate(t);
           // playerMov.currentHexFowardForce += BoostForce * curveValue * Time.deltaTime; -> Boost DuratioN:0.8
            playerMov.CurrentHexFowardForce = forwardForce;
             yield return null;
        }
        playerRb.velocity = playerRb.velocity / 2;
        playerMov.OnBoostForwardHex = false;
        playerMov.CurrentHexFowardForce = 0;
        IsHexForwardBoosting = false;
    }
    */
    #endregion
    #endregion
    #region Trampolin
    [Header("Trampolin")]
   // float reboundDuration = 0.2f;
    [SerializeField] float tramoplinForce = 15f;
    //[SerializeField] float velocityInfluence = 0.5f;
    //private Coroutine trampolinCoroutine;
    public void TrampolinStarter()
    {
        if (ReferenceLibrary.GameMng.AllowHexEffects == false) return;
        StartCoroutine(MultiplicatorModificationOverTime());
        PlaySound();
        ReferenceLibrary.HexMov.rebounded = true;
        ReferenceLibrary.PlayerRb.velocity = new Vector3(ReferenceLibrary.PlayerRb.velocity.x * 0.1f, ReferenceLibrary.PlayerRb.velocity.y, ReferenceLibrary.PlayerRb.velocity.z * 0.1f);
        ReferenceLibrary.HexMov.OnTrampolinHex = true;
        ReferenceLibrary.HexMov.CurrentTrampolinForce = tramoplinForce;
        //if (trampolinCoroutine != null)
        //  StopCoroutine(trampolinCoroutine);
        //trampolinCoroutine = StartCoroutine(TrampolinCoroutine());
    }
    #region Useless: TrampolinCoroutine
    /*
    IEnumerator TrampolinCoroutine()
    {
        float timer = 0;
        playerMov.OnTrampolinHex = true;
        while (playerMov.rebounded == true)
        {
            timer += Time.fixedDeltaTime;
            if (timer < reboundDuration)
            {
                if (gameMng.AllowHexEffects == false) break;
                //Vector3 direction = Vector3.up;
                //Vector3 ReboundMovement = direction.normalized * (tramoplinForce * 10) * Time.deltaTime;
                playerMov.CurrentTrampolinForce = tramoplinForce;
                //playerRb.AddForce(ReboundMovement * Time.deltaTime * 100, ForceMode.Impulse);
                //playerRb.AddForce(ReboundMovement.normalized, ForceMode.Impulse);
                //playerRb.AddForce(direction.normalized * tramoplinForce * 100 * Time.deltaTime, ForceMode.Impulse);
            }
            else
            {
                playerMov.rebounded = false;
                timer = 0;
            }
            yield return null;
        }
        yield return null;
        playerMov.OnTrampolinHex = false;
    }
    */
    #endregion
    #endregion
    #region BoostinDirection
    [Header("BoostInDirection")]
    public float XDirection =1;
    public float ZDirection = 1;
    float YDirection = 0;
    Vector3 BoostInDirectionDirection;
    // Coroutine hexBoostInDirectionCoroutine;
    [Range(10, 30)] [SerializeField] float boostInDForce = 20;
    // float BoostInDirectionDuration = 0.3f;
    //  bool IsBoostingInDirection = false;
    void BoostInDirectionStarter()
    {
        if (ReferenceLibrary.GameMng.AllowHexEffects == false) return;
        ReferenceLibrary.HexMov.BoostInDirectionTimer = 0;
        ReferenceLibrary.PlayerRb.velocity = Vector3.zero;
        ReferenceLibrary.HexMov.CurrentHexInDirectionForce = boostInDForce;
        BoostInDirectionDirection = new Vector3(XDirection, YDirection, ZDirection);
        ReferenceLibrary.HexMov.HexInDirectionDirection = BoostInDirectionDirection.normalized;
        ReferenceLibrary.HexMov.OnBoostInDirectionHex = true;
        StartCoroutine(MultiplicatorModificationOverTime());
        //   if (hexBoostInDirectionCoroutine != null)
        //      StopCoroutine(hexBoostInDirectionCoroutine);
        //  hexBoostInDirectionCoroutine = StartCoroutine(HexBoostInDirectionCoroutine());
    }
    #region NoUse: coroutine
    /*private IEnumerator HexBoostInDirectionCoroutine()
    {
        OnEffectHex?.Invoke();
        float t = 0;
        playerMov.OnBoostInDirectionHex = true;
        playerRb.velocity = Vector3.zero; //playerRb.velocity * 0.2f;
        while (t < BoostInDirectionDuration)
        {
            if (gameMng.AllowHexEffects == false) break;
            t += Time.deltaTime;
            playerMov.HexInDirectionDirection = BoostInDirectionDirection;
            playerMov.CurrentHexInDirectionForce = playerMov.CurrentHexInDirectionForce* Time.deltaTime * 0.99f  * boostInDForce;
            yield return null;
        }
        //playerRb.velocity = playerRb.velocity / 2;
        playerMov.OnBoostInDirectionHex = false;
        playerMov.CurrentHexInDirectionForce = 100;
        IsBoostingInDirection = false;
        playerMov.HexInDirectionDirection = Vector3.zero;
        yield return null;
    }
    */
    #endregion
    #endregion
    IEnumerator MultiplicatorModificationOverTime()
    {
        ScoreManager.OnTemporaryMultiplicatorUpdate(hexEffectsSettings.value);
        yield return new WaitForSeconds(hexEffectsSettings.ModificationDuration);
        ScoreManager.OnTemporaryMultiplicatorUpdate(-hexEffectsSettings.value);
        yield return null;
    }
    #endregion
    #region Collectables
    [Header("Collectables")]
    //[SerializeField] SpawnHexCollectableInEditor spawnHexEditor;
    [Space] public GameObject MyCollectable; //HIdeInInsp
   // [SerializeField] GameObject collectablePrefab;

   #endregion
   #if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (hexType != HexType.BoostInDirection) return;
        BoostInDirectionDirection = new Vector3(XDirection, YDirection, ZDirection);

        float arrowLength = 5f;
       
        Vector3 forwardVector = BoostInDirectionDirection.normalized;
        Vector3 arrowLeft = Vector3.down * arrowLength * 0.2f;
        Vector3 arrowRight = -arrowLeft;
        Vector3 arrowTip = forwardVector * arrowLength;
        arrowLeft += forwardVector * arrowLength * .7f;
        arrowRight += forwardVector * arrowLength * .7f;
        
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawLine(arrowTip + new Vector3(0, 2, 0), this.transform.forward + new Vector3(0,2,0));
        Gizmos.DrawLine(arrowTip + new Vector3(0, 2, 0), arrowLeft + new Vector3(0, 2, 0));
        Gizmos.DrawLine(arrowTip + new Vector3(0, 2, 0), arrowRight + new Vector3(0, 2, 0));
    }
#endif
    void PlaySound()=> ReferenceLibrary.AudMng.HexAudMng.PlayHex(hexType);
}