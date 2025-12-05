using System.Collections;
using UnityEngine;
public class ShadowDash : MonoBehaviour
{
    #region Inspector
    [Space] private Coroutine shadowDashCoroutine;
    [SerializeField] private AnimationCurve shadowDashcurve;
    [SerializeField] public bool isShadowDashing = false; 
    private int playerLayerInt, playerNoCollisionLayerInt;
    //private bool shadowDashNotPossible = false;//used to lock other boosts
    [SerializeField] private float ShadowDashForce, ShadowDashDuration;
    [SerializeField] public float  currentShadowDashForce = 0.0f, 
                                   disappearingDuringShadowDashStart, 
                                   disappearingDuringShadowDashEnd;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;
    public ParticleSystem particle;
    public MeshRenderer mr, mr2;
    #endregion
    private void Start()
    {
        playerLayerInt = LayerMask.NameToLayer("Player");
        playerNoCollisionLayerInt = LayerMask.NameToLayer("PlayerNoCollision");
        audioSource.clip = clip;
    }
    void FixedUpdate()
    {
        if (GameStateManager.gameState == GameStateManager.GameState.Start || !ReferenceLibrary.GameMng.AllowMovement || ReferenceLibrary.SuperDash.isSuperDashing) return;
        if (!mr2.enabled && !mr.enabled && !particle.isPlaying) particle.Play(); else particle.Stop();
        
        if (Input.GetButton("RightBumper") && !isShadowDashing)
        {
            isShadowDashing = true;
            ShadowDashStarter();
            ReferenceLibrary.Dash.IsBoosting = false;
        }
        if (currentShadowDashForce != 0 ) ReferenceLibrary.PlayerRb.AddForce(ReferenceLibrary.PlayerMov.MovementDirection.normalized * currentShadowDashForce * 400 * Time.fixedDeltaTime);
    }
    #region Shadowdash Coroutine
    public void ShadowDashStarter()
    {
        if (shadowDashCoroutine != null) StopCoroutine(shadowDashCoroutine);
        ReferenceLibrary.GameMng.InputMade();
        shadowDashCoroutine = StartCoroutine(ShadowDashCoroutine());
    }
    private IEnumerator ShadowDashCoroutine()
    {
        StartCoroutine(ReferenceLibrary.EnergyMng.ModifyEnergy(-ReferenceLibrary.GameMng.ShadowDashCosts));
        audioSource.pitch = Random.Range(0.8f, 1.6f);
        audioSource.PlayDelayed(0.1f);
        float t = 0;
        while (t < ShadowDashDuration)
        {
            mr.enabled = true; mr2.enabled = true;
            t += Time.fixedDeltaTime;
            float curveValue = shadowDashcurve.Evaluate(t); // / ShadowDashDuration
            currentShadowDashForce += ShadowDashForce * curveValue * Time.fixedDeltaTime;
            if (currentShadowDashForce >= disappearingDuringShadowDashStart 
                && currentShadowDashForce <= disappearingDuringShadowDashEnd) 
            {
                gameObject.layer = playerNoCollisionLayerInt;
                ReferenceLibrary.GameMng.AllowHexEffects = false; mr.enabled = false; mr2.enabled = false;
            }
            if (!ReferenceLibrary.GameMng.AllowMovement) break;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.4f);
        ReferenceLibrary.PlayerRb.velocity = ReferenceLibrary.PlayerRb.velocity / 2;
        currentShadowDashForce = 0;
        isShadowDashing = false; ReferenceLibrary.GameMng.AllowHexEffects = true; mr.enabled = true; mr2.enabled = true;
        gameObject.layer = playerLayerInt;
    }
    #endregion
}


