using System.Collections;
using UnityEngine;
public class EnergyManager : MonoBehaviour //for points and energy
{
    public bool DisableEnergyCosts = false;
    [Space] [SerializeField] float EnergyStartAmount = 10;
    public static float CurrentEnergy;
    [Tooltip("A limit of how many Energy the player can have")] 
    public float MaxEnergyAmount = 20f;
    [SerializeField] float stepSize = 0.1f;
    public static bool energyGotHigher = false;
    [Space] public float ConstantEnergyDecrease = 0.005f;
    [Space] [SerializeField] AudioSource myAudioSource;
    void Awake()
    {
        energyGotHigher = false;
        CurrentEnergy = EnergyStartAmount;
        InvokeRepeating("UpdateEnergy", 1f, 0.2f);
    }
    
    private void UpdateEnergy()
    {
        if (GameStateManager.gameState == GameStateManager.GameState.Start) return;
        /*
        if(CurrentEnergy > 10)
            CurrentEnergy -= ConstantEnergyDecrease; */
        if (CurrentEnergy >= 80)
            CurrentEnergy = CurrentEnergy - ConstantEnergyDecrease - ReferenceLibrary.GameMng.CurrentNoInputInfluence;
        else if (CurrentEnergy >= 60 && CurrentEnergy < 80)
            CurrentEnergy = CurrentEnergy - ConstantEnergyDecrease - ReferenceLibrary.GameMng.CurrentNoInputInfluence - 0.002f;
        else if (CurrentEnergy >= 40 && CurrentEnergy < 60)
            CurrentEnergy = CurrentEnergy - ConstantEnergyDecrease - ReferenceLibrary.GameMng.CurrentNoInputInfluence - 0.005f;
        else if(CurrentEnergy >= 20 && CurrentEnergy < 40)
            CurrentEnergy = CurrentEnergy - ConstantEnergyDecrease - ReferenceLibrary.GameMng.CurrentNoInputInfluence- 0.007f;
        else if( CurrentEnergy >= -0.5 && CurrentEnergy <20)
            CurrentEnergy = CurrentEnergy - ConstantEnergyDecrease - ReferenceLibrary.GameMng.CurrentNoInputInfluence;
        
        if (DisableEnergyCosts) CurrentEnergy = 25;
        if(!GameStateManager.GameOver) CheckEnergyAmount();
    }
    /*
    void ModifyEnergy(float value)
    {  
        float step = stepSize * Mathf.Sign(value);
        float stepsDone = 0;
        float timer = 0;
        while (stepsDone < Mathf.Abs(value))
        {
            timer = Time.deltaTime;
            if(timer > 0.1f)
            {
                CurrentEnergy += step;
                stepsDone += Mathf.Abs(step);
                timer = 0;
            } 
        }
        //CurrentEnergy += value;
    }
    */
    public IEnumerator ModifyEnergy(float value)
    {
        float absValue = Mathf.Abs(value), step = stepSize * Mathf.Sign(value), stepsDone = 0;
        while (stepsDone < absValue)
        {
            CurrentEnergy += step;
            stepsDone += stepSize;
            //UIManager.Instance.UpdateEnergyUI();
            if (Mathf.Approximately(CurrentEnergy, 0)) break;
            /* if (CurrentEnergy <= 0)
             {
                 Debug.Log("Breaked");break;
             }
            */
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    bool EnergyEmptyPlayed = false;
    void CheckEnergyAmount()
    {
        if (CurrentEnergy <= 0)
        {
            //if (startDash.Boosting == true) return; OLD
            if (EnergyEmptyPlayed == false)
            {
                EnergyEmptyPlayed = true;
                myAudioSource.Play();
            }
            ReferenceLibrary.GameMng.AllowMovement = false;
            //StopAllCoroutines(); OLD
            //if(GameManager.Instance.GameOver == false)
            ReferenceLibrary.GameStateMng.CheckForEndOfGame();
        }
        else
        {
            ReferenceLibrary.GameMng.AllowMovement = true;
            EnergyEmptyPlayed = false;
        }
        if(CurrentEnergy >= MaxEnergyAmount) CurrentEnergy = MaxEnergyAmount;
    }
    public bool CheckForRequiredEnergyAmount(float value)
    {
        if (value <= CurrentEnergy) return true;
        return false; //Sound machen
    }
}