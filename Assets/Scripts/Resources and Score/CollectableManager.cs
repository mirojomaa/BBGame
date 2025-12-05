using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using NaughtyAttributes;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;

public class CollectableManager : MonoBehaviour
{
    public delegate void RespawnCollectables();
    public static RespawnCollectables OnRespawnCollectables;
    public static bool rotateCollectablesInEditor = true;
    [InfoBox("The list will empty when the game starts. This is intended because it used native Container for multithreading and better cpu cache lining/ reading")]
    [BoxGroup("Debug")] [Tooltip("PreMade Array that has all the Transforms that then gets mem copied by the native Transformaccessarray")] 
    [SerializeField] public Transform[] allCollectableHexParentTransformsBeforeStart;
    [BoxGroup("Debug")] [Tooltip("PreMade Array that has all the bools at the start. When the Object is there but disabled it safes it and hands its data over a native Byte array")] 
    [SerializeField] public byte[] allCollectableActiveBoolsBeforeStart;
    [BoxGroup("Debug")]  [Tooltip("To safe some Bytes on the Objects, we have the relevant Hex components in an array. This gets hands over to the static variant")] 
    [SerializeField] public Hex[] allHexScriptsForCollectablesUseBeforeStart;
    [BoxGroup("Debug")][SerializeField] private byte[] allRandomSpeedsBeforeStart;
    [BoxGroup("Debug")] [SerializeField] public Transform[] allCollectablesTransformsBeforeStart;
    [InfoBox("Automaticly updates when slider change. Randomizes between those numbers so every Collectable gets a different rotation. Gets converted to bytes, so everything after the . gets cut \n Distance Treshold: In this distance no collectable will be spawned")]
    [BoxGroup("Configure")] [MinMaxSlider(0,255)] [SerializeField]  private Vector2 rotationRandomBetween = new Vector2(30, 120);
    [BoxGroup("Configure")] [Range(0,3000)][SerializeField] private float distanceTreshhold = 100f;
     
    private static int fixedLength;
    private static NativeQueue<int> allValidSpawnAbleHexID;
    TransformAccessArray allCollectableHexParentTransforms;
    private TransformAccessArray allCollectablesTransforms;
    NativeArray<byte> allCollectableActiveBools;
    private static Hex[] allHexScriptsForCollectablesUse;
    private NativeArray<byte> allTheRandomSpeeds;
    private JobHandle rotationJOB;
    private void Awake() => SetNativeContainer();

    void Start()
    {
        distanceTreshhold *= distanceTreshhold;
        OnRespawnCollectables = null;
        OnRespawnCollectables += spawnCollectableObjects;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        for (int i = 0; i < allRandomSpeedsBeforeStart.Length; i++) allRandomSpeedsBeforeStart[i] = (byte)UnityEngine.Random.Range((int)rotationRandomBetween.x, (byte)rotationRandomBetween.y);
    }

    [NaughtyAttributes.Button()] public void fillCollectableListsBeforeStart()
    {
        //------------ Fill local scope Lists for easier handling ---------//
        HexAutoTiling hexAutoTiling = FindObjectOfType<HexAutoTiling>();   //get all the hex Objectgs
        HashSet<Collectable> collectablesHashSet = new HashSet<Collectable>();   // make a new local hashset, to guarantee everything we add is unique
        
        foreach (Transform hexTransform in hexAutoTiling.hasAllTheHexGameObjectsTransformsBeforeStart)
        {
            foreach (Collectable collectable in hexTransform.GetComponentsInChildren<Collectable>())
                collectablesHashSet.Add(collectable);  //adding process to hashset
        }
        List<Collectable> collectableList = collectablesHashSet.ToList();  //convert that hashset to a list. Not needed but its easier this way and it is a editor tool anayway 
        
        
        
        //------------ Resize Arrays ---------//
          fixedLength  = collectableList.Count;
        Array.Resize(ref allCollectableHexParentTransformsBeforeStart, fixedLength );
          Array.Resize(ref allHexScriptsForCollectablesUseBeforeStart, fixedLength );
          Array.Resize(ref allCollectableActiveBoolsBeforeStart, fixedLength );
          Array.Resize(ref allCollectablesTransformsBeforeStart, fixedLength );
          Array.Resize(ref allRandomSpeedsBeforeStart, fixedLength );

          for (int i = 0; i < fixedLength ; i++) allRandomSpeedsBeforeStart[i] = (byte)UnityEngine.Random.Range((int)rotationRandomBetween.x, (byte)rotationRandomBetween.y);

          for (var index = 0; index < collectableList.Count; index++)   //Transforms for rotationRandomBetween
              allCollectablesTransformsBeforeStart[index] = collectableList[index].gameObject.transform;
          

          //------------ Fill Serialized Container that then will get converted to Native Container ---------//
        int counter = 0;  //index id counter. The Collectable List stays fixed
        foreach (Collectable collectableScript in collectableList) 
        {
           Hex hexScript = collectableScript.GetComponentInParent<Hex>();
           allCollectableHexParentTransformsBeforeStart[counter] = hexScript.gameObject.transform;  //Fill with HexObject Transform
           allHexScriptsForCollectablesUseBeforeStart[counter] = hexScript;   //fill with the Script "Hex" so we have a premade List and dont need get component anymore
           if (collectableScript.enabled)  //check if the script is enabled
               allCollectableActiveBoolsBeforeStart[counter] = 1;    //if yes, we add true to the bool list (needed for multithreading later)
           else allCollectableActiveBoolsBeforeStart[counter] = 0;  //if not active we add false
           counter++;   //we add one more to the indexid counter
        }

        //------------ Change the Serialized Propertys---------//
        SerializedObject serializedHex;  // for setting the Collectable 
        SerializedObject serializedCollectable; // for setting the array index, named CollectableIndexID
        
        for (int i = 0; i < allHexScriptsForCollectablesUseBeforeStart.Length; i++)
        {
            serializedHex = new SerializedObject(allHexScriptsForCollectablesUseBeforeStart[i]);
            if (allHexScriptsForCollectablesUseBeforeStart[i].gameObject)
            {
                serializedHex.FindProperty("MyCollectable").objectReferenceValue = collectableList[i].gameObject; 
                serializedHex.ApplyModifiedPropertiesWithoutUndo();
                
                serializedCollectable = new SerializedObject(collectableList[i]);
                serializedCollectable.FindProperty("CollectableIndexID").intValue = i;
                serializedCollectable.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        //------------ Disable all deactived collectables Gameobjects---------//
        for (int i = 0; i < allCollectableActiveBoolsBeforeStart.Length; i++)
        {   //not reaaaally needed it is just a safety check
            if(allCollectableActiveBoolsBeforeStart[i] == 0) collectableList[i].gameObject.SetActive(false);
        }
    }
    [NaughtyAttributes.Button()] public void ToogleRotationCollectableInEditor() => rotateCollectablesInEditor = !rotateCollectablesInEditor;
    private void OnDrawGizmos()
    {
        if (Application.isPlaying || !rotateCollectablesInEditor ) return;
        if (rotationJOB.IsCompleted == false) return;
       NativeArray<byte> RandomSpeedsForEditor =
                new NativeArray<byte>(allRandomSpeedsBeforeStart, Allocator.TempJob);
            rotateCollectablesJob rotateJob = new rotateCollectablesJob
            {
                RandomSpeed =  RandomSpeedsForEditor,
                deltaTime = Time.deltaTime
            };
            TransformAccessArray JustForJob = new TransformAccessArray(allCollectablesTransformsBeforeStart,12);
            rotateJob.Schedule(JustForJob).Complete();
            RandomSpeedsForEditor.Dispose();
            JustForJob.Dispose();
        
        
    }
#endif
    void Update()
    {  
        if (Input.GetKeyDown(KeyCode.M)) spawnCollectableObjects();
        if (rotationJOB.IsCompleted)
        {  rotateCollectablesJob rotateJob = new rotateCollectablesJob
            {
                deltaTime = Time.deltaTime,
                RandomSpeed =  allTheRandomSpeeds
            };
            rotationJOB = rotateJob.Schedule( allCollectablesTransforms, rotationJOB);
        }
    }

    private void SetNativeContainer()
    {
        if (!Application.isPlaying) return;
        allValidSpawnAbleHexID= new NativeQueue<int>(Allocator.Persistent);  //Queue has the Objects that are Valid for Spawn
        allCollectableHexParentTransforms = new TransformAccessArray(allCollectableHexParentTransformsBeforeStart, 28); //Native Transforms are much faster than normal
        allCollectableActiveBools = new NativeArray<byte>(allCollectableActiveBoolsBeforeStart, Allocator.Persistent); // Keep a native bool list for checking if ValidSpawn
        allHexScriptsForCollectablesUse = allHexScriptsForCollectablesUseBeforeStart;  //better have an static List premade than having a getcomponend of ref type
        allCollectablesTransforms = new TransformAccessArray(allCollectablesTransformsBeforeStart, 28);
        allTheRandomSpeeds = new NativeArray<byte>(allRandomSpeedsBeforeStart,Allocator.Persistent);
        
        allCollectablesTransformsBeforeStart = null;
        allCollectableHexParentTransformsBeforeStart = null;   //null not needed  Serialized Container
        allCollectableActiveBoolsBeforeStart = null; //  since we have native Containter
        allHexScriptsForCollectablesUseBeforeStart = null;
        allRandomSpeedsBeforeStart = null;
    }

    private void OnApplicationQuit()
    {
        rotationJOB.Complete();
        DestroyAll();
    }

    private void OnDestroy()
    {
        rotationJOB.Complete();
        Invoke("DestroyAll",0.1f);
    }

    void DestroyAll()
    {
        if (!Application.isPlaying) return;
        //Dispose all NativeContainer
        allValidSpawnAbleHexID.Dispose();
        allCollectableHexParentTransforms.Dispose();
        allCollectableActiveBools.Dispose();
        allCollectablesTransforms.Dispose();
if(rotationJOB.IsCompleted) allTheRandomSpeeds.Dispose(rotationJOB);
    }
    public void CollectableCollected( float energyValue , int collectableIndexID)
    {   //Invoked by the Collectable when the Collectable gets collected 
        allCollectableActiveBools[collectableIndexID] = 0;  
        EnergyManager.energyGotHigher = true;
        StartCoroutine(ReferenceLibrary.EnergyMng.ModifyEnergy(energyValue));
        ReferenceLibrary.AudMng.HexAudMng.PlayHex(HexType.DefaultCollectable);
        allCollectablesTransforms[collectableIndexID].gameObject.SetActive(false);
    }
    public void spawnCollectableObjects()
    {
        HexCollectablePosJob spawnCheckJob = new HexCollectablePosJob
        {
            hasAllTheHexCollectableActiveBoolsJob = allCollectableActiveBools,
            hasAllTheValidSpawnAbleHexesIDJob =  allValidSpawnAbleHexID.AsParallelWriter(),
            PlayerPosJob = ReferenceLibrary.PlayerPosition,
            distanceSquared =  distanceTreshhold
        };
       JobHandle spawnCheckhandle = spawnCheckJob.Schedule(allCollectableHexParentTransforms);
       spawnCheckhandle.Complete();
       
       Debug.Log("counterCollectableRespawned " + allValidSpawnAbleHexID.Count);
       
       while (!allValidSpawnAbleHexID.IsEmpty())
           allHexScriptsForCollectablesUse[allValidSpawnAbleHexID.Dequeue()].MyCollectable.SetActive(true);
    }
}
//[NativeContainerSupportsDeallocateOnJobCompletion]  
[BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
struct rotateCollectablesJob : IJobParallelForTransform
{
    [Unity.Collections.ReadOnly] public float deltaTime;
    [Unity.Collections.ReadOnly] public NativeArray<byte> RandomSpeed;
    public void Execute(int index, TransformAccess transform)
    {
        transform.localRotation *= Quaternion.Euler(0, RandomSpeed[index]*deltaTime, 0);
    }
}

[BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
public struct HexCollectablePosJob : IJobParallelForTransform
{
    public NativeArray<byte> hasAllTheHexCollectableActiveBoolsJob;
    [NativeDisableParallelForRestriction] [WriteOnly] public NativeQueue<int>.ParallelWriter hasAllTheValidSpawnAbleHexesIDJob;
    [Unity.Collections.ReadOnly] public Vector3 PlayerPosJob;
    [Unity.Collections.ReadOnly] public float distanceSquared;
    public void Execute(int index, TransformAccess hasAllTheHexCollectableTransform)
    {
        if (hasAllTheHexCollectableActiveBoolsJob[index] == 0 && MathLibary.sqrMagnitudeInlined(PlayerPosJob - hasAllTheHexCollectableTransform.position) > distanceSquared)
        {
            hasAllTheValidSpawnAbleHexesIDJob.Enqueue(index);
            hasAllTheHexCollectableActiveBoolsJob[index] = 1;
        }
    }

}