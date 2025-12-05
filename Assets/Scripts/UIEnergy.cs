using System.Collections;
using UnityEngine;
public class UIEnergy : MonoBehaviour
{
    [SerializeField] private Material material;
    public ParticleSystem _energyPartikel;
    private string _control = "_control";
    void Update()
    {
        material.SetFloat(_control,  MathLibary.Remap(
            ReferenceLibrary.EnergyMng.MaxEnergyAmount*0.1f,
            ReferenceLibrary.EnergyMng.MaxEnergyAmount,
            0.45f,
            0,
            EnergyManager.CurrentEnergy));
        playUIVFX();
    }
    void playUIVFX()
    {
        if (EnergyManager.energyGotHigher && !_energyPartikel.isPlaying)
        {
            {
               StopCoroutine(playUIVFX_Coroutine());
               StartCoroutine(playUIVFX_Coroutine());
               EnergyManager.energyGotHigher = false;
            }
        }
    }
    IEnumerator playUIVFX_Coroutine()
    { 
        _energyPartikel.Play(true);
        yield return null;  
    }
}