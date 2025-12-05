using UnityEngine;
public class HexMovements : MonoBehaviour
{
    #region Inspector
    [HideInInspector] public bool OnBoostForwardHex = false;
    [HideInInspector] public float CurrentHexFowardForce;
    [HideInInspector] public Vector3 ForwardDirection;

    [HideInInspector] public bool OnChangeDirectionHex = false;
    
    [HideInInspector] public bool OnBoostInDirectionHex = false;
    [HideInInspector] public float CurrentHexInDirectionForce;
    [HideInInspector] public Vector3 HexInDirectionDirection;
    //public float currentHexChangeDirectionForce;

    [HideInInspector] public bool OnTrampolinHex = false;
    [HideInInspector] public float CurrentTrampolinForce;

    [HideInInspector] public bool OnSlowDownHex = false;

    [HideInInspector] public float TrampolinTimer;
    [HideInInspector] public float BoostInDirectionTimer;
    [HideInInspector] public float BoostForwardTimer;
    [HideInInspector] public float SlowDownTimer;
    public bool rebounded = false;

    #endregion
    void FixedUpdate() =>HexEffects();
    void HexEffects()
    {
        if (!ReferenceLibrary.GameMng.AllowHexEffects) return;
        
        if (OnBoostForwardHex && BoostForwardTimer < 0.4f)
        {    // BOOST FORWARD
            BoostForwardTimer += Time.fixedDeltaTime;
            ForwardDirection = ReferenceLibrary.PlayerRb.velocity.normalized;
            ReferenceLibrary.PlayerRb.AddForce(ForwardDirection * CurrentHexFowardForce * 500 * Time.fixedDeltaTime);
        }
        else if (OnBoostForwardHex && BoostForwardTimer > 0.4f)
        {
            ReferenceLibrary.PlayerRb.velocity = ReferenceLibrary.PlayerRb.velocity / 2;
            OnBoostForwardHex = false;
        }
        else BoostForwardTimer = 0;
        
        if (OnBoostInDirectionHex && BoostInDirectionTimer < 0.3f)
        {   //  BOOST IN DIRECTION
            //  Debug.Log("HexInDirection");
            ReferenceLibrary.PlayerRb.AddForce(HexInDirectionDirection * CurrentHexInDirectionForce * 500 * Time.fixedDeltaTime);
            BoostInDirectionTimer += Time.fixedDeltaTime;
            //CurrentHexInDirectionForce = CurrentHexInDirectionForce * 0.99f;
        }
        else
        {
            BoostInDirectionTimer = 0;
            OnBoostInDirectionHex = false;
        }
        
        // SLOW DOWN
        /*
        if (OnSlowDownHex == true && SlowDownTimer < 0.4f)
        {
            SlowDownTimer += Time.fixedDeltaTime;
            rb.velocity *= 0.9f;
        }
        else
        {
            SlowDownTimer = 0;
            OnSlowDownHex = false;
        }
        */
        /*  // Change Direction
         if(OnChangeDirectionHex == true)
         {
             rb.AddForce(rb.velocity.normalized * 20 * Time.fixedDeltaTime); //*currentHexChangeDirectionForce 
         }
         */
        if (OnTrampolinHex && TrampolinTimer < 0.2)
        { // TRAMPOLIN
            TrampolinTimer += Time.fixedDeltaTime;
            ReferenceLibrary.PlayerRb.AddForce(Vector3.up * CurrentTrampolinForce * 10 * Time.fixedDeltaTime, ForceMode.Impulse); //CurrentTrampolinForce
        }
        else
        {
            OnTrampolinHex = false;
            TrampolinTimer = 0;
            rebounded = false;
        }
    }
}