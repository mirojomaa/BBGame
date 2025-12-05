using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableEnergyGenerator : ScriptableObject
{
    public float maxEnergy = 10f; //1;
    public float energyGenerationAmount = 1;//0.1f;
    public float energyGenerationRate = 2; //1f; //Alle 1Sekunde
    public AudioClip clip;
}
