using System.Collections;
using UnityEngine;
public class PlayerBoost : MonoBehaviour
{
    #region Inspector
    //[SerializeField] float boostCost = 1;
    [SerializeField] private float BoostForce = 75;
    float boostDuration = 0.8f;
    [SerializeField] private AnimationCurve BoostDashcurve;
    private float curveValue;
    [SerializeField] public bool IsBoosting; //used to lock other boosts
    [SerializeField] private Coroutine boostCoroutine;
    [SerializeField] private AudioSource audioSource;

    float boostTimer;
    
    [SerializeField] ParticleSystem particle;
    #endregion

    void FixedUpdate()
    {
        if (GameStateManager.gameState == GameStateManager.GameState.Start || !ReferenceLibrary.GameMng.AllowMovement) return;
        // if (shadowDash.isShadowDashing == true || superDash.isSuperDashing == true) return;
        // if (rb.velocity.x == 0 || rb.velocity.z == 0) return; //kein kleiner Boost am Anfang erlaubt!
        if (Input.GetAxisRaw("RightTrigger") != 0 && !IsBoosting)
        {
            IsBoosting = true; BoostStarter();
        }

#if UNITY_EDITOR

        if (Input.GetButtonDown("X") && !IsBoosting)
        {
            IsBoosting = true; BoostStarter();
        }
#endif
        if (IsBoosting&& boostTimer < boostDuration)
        {
            // playerBoost.CurveValue
            boostTimer += Time.fixedDeltaTime;
             ReferenceLibrary.PlayerRb.AddForce(ReferenceLibrary.PlayerMov.MovementDirection.normalized * curveValue * BoostForce * 100 * Time.fixedDeltaTime);
        }
        else if (IsBoosting&& boostTimer > boostDuration)
        {
            ReferenceLibrary.PlayerRb.velocity =  ReferenceLibrary.PlayerRb.velocity / 2;
            IsBoosting = false;
        }
        else boostTimer = 0;
    }
    #region Dash Coroutine
public void BoostStarter()
    {
        IsBoosting = true;
        boostTimer = 0;
        if (boostCoroutine != null) StopCoroutine(boostCoroutine);
        ReferenceLibrary.GameMng.InputMade();
        boostCoroutine = StartCoroutine(BoostCoroutine());
    }
private IEnumerator BoostCoroutine()
    {
        if(ReferenceLibrary.GameMng.DashCosts != 0) StartCoroutine(ReferenceLibrary.EnergyMng.ModifyEnergy(-ReferenceLibrary.GameMng.DashCosts));
        //var emission = particle.emission;// emission.enabled = true;
        
        //Sound und trail particle auf world stellen? bzw trail world particle anstellen
        if (audioSource.isPlaying == false)
        { audioSource.pitch = Random.Range(0.8f, 1.6f); 
            audioSource.Play();
        }
        float t = 0;
        while (t < boostDuration)
        {
            t += Time.fixedDeltaTime;
            curveValue = BoostDashcurve.Evaluate(t);
            //currentBoostforce += BoostForce * CurveValue * Time.deltaTime;
            yield return null;
        }
        //emission.enabled = false;// rb.velocity = rb.velocity / 2;//rb.velocity = velocity; //IsBoosting = false;
    }
    #endregion
}