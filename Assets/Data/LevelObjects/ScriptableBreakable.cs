using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ScriptableBreakable : ScriptableObject
{

  public bool ResetAfterDestruction = true;
  public float ResetAfterTime;
  
  private GameObject player;
  public GameObject breakablePrefab;
  public AudioClip audioOnBreak;
  public float audioDelay = 0.01f;
  public GameObject particleToSpawn;

}
