using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu]
public class DestroyableScriptableObject : ScriptableObject
{  
    public  GameObject BrokenPrefab;
    public AudioClip CollisionClip, DestructionClip;
    [Space] public float ExplosiveForce = 1000, ExplosiveRadius = 2,
     FadeSpeed = 3.1f, DestroyDelay = 5f, SleepCheckDelay = 0.5f;
    [Space] public bool Respawn = true; 
     public float resetTimer = 3; 
    [Space] public float DestroyValue = 50, CollisionValue = 50;
    [Space] [Tooltip("Allow Destruction after \"Hitamount\" hits ")]
    public bool AllowAutomatedDestruction = true;
    public int HitAmount = 3;
    [Space] public bool ChangeMaterial = false;
    public Material Material01, Material02, Material03, Material04;
    [Space] public AudioMixerGroup CollisionGroup, DestroyGroup;
    [Space] [Tooltip("Better only use Y!")] public Vector3 DestroyedCubeAdditionalDistance;
}
