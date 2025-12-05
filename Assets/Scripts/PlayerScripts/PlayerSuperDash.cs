using System.Collections;
using UnityEngine;
public class PlayerSuperDash : MonoBehaviour
{
    #region Inspector

    //[SerializeField] float boostCost = 1;
    [Space] [SerializeField] private float SuperDashForce;
    [SerializeField] private float ShadowDashDuration;
    [SerializeField] private AnimationCurve SuperDashcurve;
    [SerializeField] public bool isSuperDashing = false; //used to lock other boosts

    public Coroutine superDashCoroutine;

    [SerializeField] public float currentSuperDashForce = 0.0f;
    [SerializeField] private float disappearingDuringSuperDashStart;
    [SerializeField] private float disappearingDuringSuperDashEnd;
    public bool isDestroying = false;
    [SerializeField] bool superDashNotPossible = false;

    // public MeshRenderer mr;
    [SerializeField] LayerMask worldMask;
    int playerLayerInt;
    int playerDestructionLayerInt;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip startClip;
    [SerializeField] AudioClip endClip;
    [Space] [SerializeField] ParticleSystemTrails particleTrail;

    #endregion

    void FixedUpdate()
    {
        if (GameStateManager.gameState == GameStateManager.GameState.Start || !ReferenceLibrary.GameMng.AllowMovement ||
            ReferenceLibrary.ShadowDashPl.isShadowDashing) return;
        if (Input.GetButton("LeftBumper") && !isSuperDashing)
        {
            if (ReferenceLibrary.PlayerMov.MovementDirection.normalized == Vector3.zero) return;
            isSuperDashing = true;
            SuperDashStarter();
            if (ReferenceLibrary.Dash.IsBoosting) ReferenceLibrary.Dash.IsBoosting = false;
        }
        else if (!Input.GetButton("LeftBumper") && superDashNotPossible)
        {
            isSuperDashing = false;
            superDashNotPossible = false;
        }

#if UNITY_EDITOR
        if (Input.GetButton("Y") && !isSuperDashing)
        {
            if (ReferenceLibrary.PlayerMov.MovementDirection.normalized == Vector3.zero) return;
            isSuperDashing = true;
            SuperDashStarter();
            if (ReferenceLibrary.Dash.IsBoosting)
                ReferenceLibrary.Dash.IsBoosting = false;
        }
        else if (Input.GetButton("Y") && superDashNotPossible)
        {
            isSuperDashing = false;
            superDashNotPossible = false;
        }
#endif
        if (currentSuperDashForce != 0)
            ReferenceLibrary.PlayerRb.AddForce(ReferenceLibrary.PlayerMov.MovementDirection.normalized *
                                               currentSuperDashForce * 400 * Time.fixedDeltaTime);
    }
    public void SuperDashStarter()
    {
        if (superDashCoroutine != null) StopCoroutine(superDashCoroutine);
        ReferenceLibrary.GameMng.InputMade();
        superDashCoroutine = StartCoroutine(SuperDashCoroutine());
        particleTrail.StartSuperDashParticle();

    }
    private IEnumerator SuperDashCoroutine()
    {
        StartCoroutine(ReferenceLibrary.EnergyMng.ModifyEnergy(-ReferenceLibrary.GameMng.SuperDashCosts));

        if (audioSource.isPlaying == false)
        {
            audioSource.pitch = Random.Range(0.8f, 1.6f);
            audioSource.clip = startClip;
            audioSource.Play();
        }

        float t = 0;
        while (t < ShadowDashDuration)
        {
            t += Time.fixedDeltaTime;
            float curveValue = SuperDashcurve.Evaluate(t); // / ShadowDashDuration
            currentSuperDashForce += SuperDashForce * curveValue * Time.fixedDeltaTime;
            if (currentSuperDashForce >= disappearingDuringSuperDashStart &&
                currentSuperDashForce <= disappearingDuringSuperDashEnd) isDestroying = true;
            if (!ReferenceLibrary.GameMng.AllowMovement) break;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.3f);
        isDestroying = false;
        ReferenceLibrary.PlayerRb.velocity /= 2;
        currentSuperDashForce = 0;
        isSuperDashing = false;
    }
}