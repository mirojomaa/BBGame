using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableHexEffects : ScriptableObject
{
    public float value = 0.5f;
    public float ModificationDuration = 5f;
    public ParticleSystem particle;
}
