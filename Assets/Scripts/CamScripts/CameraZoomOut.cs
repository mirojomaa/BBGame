using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using NaughtyAttributes;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

public class CameraZoomOut : MonoBehaviour
{
    [Header("GameObjects")]
    [Space]
    public CinemachineVirtualCamera vcam;
    public static CinemachineVirtualCamera vcamera;
    
    private Transform lookAtCashed;
    private Transform targetAtCashed;
    public GameObject Moon;
    // public GameObject GhostLayer;

    private float
        xVelocity,
        zVelocity,
        xzVelocity,
        lerpedValue,

        lerpedValueMoonX,
        lerpedValueMoonZ,
        lerpedValueGhostLayerX,
        lerpedValueGhostLayerZ;


    [SerializeField] private float cashedFov = 79;
    [Header("Camera Zoomout")]
    [Space]
    
   [Tooltip("Wie weit soll es rauszoomen")][UnityEngine.Range(80,200)] [SerializeField] private float maxFov = 115;
   [Tooltip("Ab wann wird gezoomed abhängig vom movement")] [UnityEngine.Range(0, 20f)] [SerializeField] private float  StartZoomingValue = 0.01f;
   [Tooltip("Bis wann wird gezoomed abhängig vom movement")] [UnityEngine.Range(0,400f)][SerializeField] private float StopZoomingValue = 175f;
   [UnityEngine.Range(0.01f, 20)] 
   [Tooltip("Wie Smooth soll die Kamera zwischen den Werten Lerpen")] [SerializeField] private float zoomOutRoughness = 2;
   [UnityEngine.Range(0, 50)]
   [Tooltip("Davor wird nichts gemacht")] [SerializeField] private float ZoomOutDelay = 3;
   [UnityEngine.Range(0, 30)]
   [Tooltip("Vertikale und Horizontale Achse start zoom")] [SerializeField] private float HorizontalVerticalStartZoom = 3;

   [Header("Moon und GhostLayer")]
   [Space]
   [UnityEngine.Range(0.01f, 20)]
   [Tooltip("Wie Smooth soll die Kamera zwischen den Werten Lerpen")] [SerializeField] private float moonZoomOutRoughness = 1.5f;
   [SerializeField] [UnityEngine.Range(0, 15)] private float addXScaleMoon = 5;
   [SerializeField] [UnityEngine.Range(0, 15)] private float addZScaleMoon = 5;
   [SerializeField] private float cashedXScaleMoon = 1;
   [SerializeField] private float cashedZScaleMoon = 1;
  // [UnityEngine.Range(0.01f, 20)]
  //[Tooltip("Wie Smooth soll die Kamera zwischen den Werten Lerpen")] [SerializeField] private float ghostLayerZoomOutRoughness = 2;
  // [SerializeField] private float addXScaleGhostLayer = 10;
  // [SerializeField] private float addZScaleGhostLayer = 10;

  [Space] [Space] 
 private float shakeAngle, shakeStrength, shakeSpeed, shakeDuration, shakeNoisePercent, shakeDampingPercet, shakeRotationPercent;

 [Header("Camera Shake Management")] [Space]
 [Tooltip("Für das optionsmenü")] [SerializeField] private bool deactivateShaking = false;
 [Tooltip("Ab wann started das shaken")] [UnityEngine.Range(0f, 300)] [SerializeField] private float  StartShaking = 72f;
 [Tooltip("Je höher die zahl, desto weniger wirken alle effekte")] [UnityEngine.Range(0.02f, 2000)] [SerializeField] private float SpeedInflunceDampeningForAll = 1200;
 [Space]
 [Space]
 [Tooltip("Dampening nach dem overall Dampening zum finetunen")][UnityEngine.Range(0f, 1)] [SerializeField] private float minShakeDamping = 0.21f;
 [Tooltip("Dampening nach dem overall Dampening zum finetunen")][UnityEngine.Range(0f, 1)] [SerializeField] private float maxShakeDamping = 0.53f;
 
 [Space]
 [UnityEngine.Range(0f, 1)] [SerializeField] private float minShakeNoise = 0.23f;
 [UnityEngine.Range(0f, 1)] [SerializeField] private float maxShakeNoise = 0.56f;
 
 [Space]
 [UnityEngine.Range(0f, 1)] [SerializeField] private float minShakeRotation = 0.26f;
 [UnityEngine.Range(0f, 1)] [SerializeField] private float maxShakeRotation = 0.84f;
 
 [Space]
 [UnityEngine.Range(0f, 2)] [SerializeField] private float minShakeStrength =0.07f;
 [UnityEngine.Range(0f, 2)] [SerializeField] private float maxShakeStrength = 0.56f;

 [Space]
 [UnityEngine.Range(0f, 5)] [SerializeField] private float minShakeDuration = 0.61f;
 [UnityEngine.Range(0f, 5)] [SerializeField] private float maxShakeDuration = 1.32f;
 
 [Space]
 [UnityEngine.Range(0f, 8)] [SerializeField] private float minShakeSpeed = 1.8f;
 [UnityEngine.Range(0f, 8)] [SerializeField] private float maxShakeSpeed = 0.53f;
 
 [SerializeField] private float secNextShakeAllowed;
 [SerializeField] private bool nextShakeAllowed = true;
 [SerializeField] private CameraShakeCollision _cameraShakeCollision;
 [SerializeField] private CameraShake _cameraShake;
 [HideInInspector][SerializeField] private float smallerFOV;
 
 [SerializeField] float[] allMinMax = new float[12];
 private NativeArray<float> allMinMaxNative;
 private NativeArray<float> allOutputs;
 private JobHandle shakeJobHandle;
 
 #if UNITY_EDITOR

    [NaughtyAttributes.Button()] public void addCashedValues()
    {
        cashedFov = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView;
        cashedXScaleMoon = Moon.transform.localScale.x;
        cashedZScaleMoon = Moon.transform.localScale.z;
        smallerFOV = cashedFov - 7f;
        fillMinMaxArray();
    }
    private void fillMinMaxArray()
    {
        allMinMax[0] = minShakeStrength;
        allMinMax[1] = maxShakeStrength;
        allMinMax[2] = minShakeSpeed;
        allMinMax[3] = maxShakeSpeed;
        allMinMax[4] = minShakeDuration;
        allMinMax[5] = maxShakeDuration;
        allMinMax[6] = minShakeNoise;
        allMinMax[7] = maxShakeNoise;
        allMinMax[8] = minShakeDamping;
        allMinMax[9] = maxShakeDamping;
        allMinMax[10] = minShakeRotation;
        allMinMax[11] = maxShakeRotation;
    }
    private void OnValidate()
    {
        if(!Application.isPlaying)
        fillMinMaxArray();
    }
#endif
   
 private void Awake()
 {
     vcamera = vcam;
     allMinMaxNative = new NativeArray<float>(allMinMax,Allocator.Persistent);
     allOutputs = new NativeArray<float>(allMinMax.Length/2,Allocator.Persistent);
 }

 private void OnDestroy()
 {
     if (Application.isPlaying)
     {
         allOutputs.Dispose();
         allMinMaxNative.Dispose();
     }
 }

 void FixedUpdate()
    {
  
        xVelocity = math.abs(ReferenceLibrary.PlayerRb.velocity.x);
        zVelocity = math.abs(ReferenceLibrary.PlayerRb.velocity.z);
        xzVelocity = xVelocity + zVelocity;
        if (xzVelocity < zVelocity + HorizontalVerticalStartZoom)
            xzVelocity = zVelocity*2;
        if (xzVelocity < xVelocity + HorizontalVerticalStartZoom)
            xzVelocity = xVelocity*2;
         if(!PortalManager.CameraTeleportActive)
         {
            lerpedValue = MathLibary.RemapClamped( StartZoomingValue, StopZoomingValue, smallerFOV , maxFov, xzVelocity);
        
            lerpedValueMoonX = MathLibary.RemapClamped( StartZoomingValue, StopZoomingValue, cashedXScaleMoon, cashedXScaleMoon+ addXScaleMoon, xzVelocity);
            lerpedValueMoonZ = MathLibary.RemapClamped( StartZoomingValue, StopZoomingValue, cashedZScaleMoon, cashedZScaleMoon+ addZScaleMoon, xzVelocity);
            
       //      lerpedValueGhostLayerX = MathLibary.RemapClamped( StartZoomingValue, StopZoomingValue, cashedXScaleGhostLayer, cashedXScaleGhostLayer+ addXScaleGhostLayer, xzVelocity);
       //      lerpedValueGhostLayerZ = MathLibary.RemapClamped( StartZoomingValue, StopZoomingValue, cashedZScaleGhostLayer, cashedZScaleGhostLayer+ addZScaleGhostLayer, xzVelocity);
       // // Debug.Log(lerpedValue);
       //
       if (smallerFOV + ZoomOutDelay < lerpedValue)
       {
           vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, lerpedValue, zoomOutRoughness*Time.deltaTime);
           
           Moon.transform.localScale = new Vector3(
                   Mathf.Lerp(Moon.transform.localScale.x, lerpedValueMoonX, moonZoomOutRoughness*Time.deltaTime),
                   Moon.transform.localScale.y ,
                   Mathf.Lerp(Moon.transform.localScale.z, lerpedValueMoonZ, moonZoomOutRoughness*Time.deltaTime)
               )  ;
               
               // GhostLayer.transform.localScale = new Vector3(
               //                    Mathf.Lerp(GhostLayer.transform.localScale.x, lerpedValueGhostLayerX, ghostLayerZoomOutRoughness*Time.deltaTime),
               //                    GhostLayer.transform.localScale.y ,
               //                    Mathf.Lerp(GhostLayer.transform.localScale.z, lerpedValueGhostLayerZ, ghostLayerZoomOutRoughness*Time.deltaTime)
               //                );
        }
       if(_cameraShakeCollision.camShakeActivated && !deactivateShaking && xzVelocity > StartShaking && nextShakeAllowed || Input.GetKeyDown(KeyCode.P))
       {
       shakeAngle = Random.Range(1,9);
       /*shakeStrength = MathLibary.RemapClamped(StartZoomingValue, SpeedInflunceDampeningForAll, minShakeStrength,
           maxShakeStrength, xzVelocity);
       shakeDuration = MathLibary.RemapClamped(StartZoomingValue, SpeedInflunceDampeningForAll, minShakeDuration,
           maxShakeDuration, xzVelocity);

       shakeSpeed = MathLibary.RemapClamped(StartZoomingValue, SpeedInflunceDampeningForAll, minShakeSpeed, maxShakeSpeed,
           xzVelocity);
       shakeNoisePercent = MathLibary.RemapClamped(StartZoomingValue, SpeedInflunceDampeningForAll, minShakeNoise,
           maxShakeNoise, xzVelocity);

       shakeDampingPercet = MathLibary.RemapClamped(StartZoomingValue, SpeedInflunceDampeningForAll, minShakeDamping,
           maxShakeDamping, xzVelocity);
       shakeRotationPercent = MathLibary.RemapClamped(StartZoomingValue, SpeedInflunceDampeningForAll, minShakeRotation,
           maxShakeRotation, xzVelocity);*/

      // NativeArray<float> hasAltheMinMaxJob = new NativeArray<float>(allMinMax, Allocator.TempJob);
           ShakeJob shakeJob = new ShakeJob
           {
               StartZoomingValue = StartZoomingValue,
               SpeedInflunceDampeningForAll = SpeedInflunceDampeningForAll,
               xzVelocity = xzVelocity,
               allTheMinMax = allMinMaxNative.AsReadOnly(),
               allTheOutputs =  allOutputs
           };
           shakeJobHandle = shakeJob.Schedule(6, 6, shakeJobHandle);
           shakeJobHandle.Complete();
        //   hasAltheMinMaxJob.Dispose();
           _cameraShake.StartShake(new CameraShake.Einstellungen(shakeAngle, allOutputs[0], allOutputs[1], 
               allOutputs[2], allOutputs[3], allOutputs[4], allOutputs[5]));
           StartCoroutine(Coroutine_TimeBetweenShakes(secNextShakeAllowed));

       }
       _cameraShakeCollision.camShakeActivated = false;
     }
    }
 
 IEnumerator Coroutine_TimeBetweenShakes(float secondsuntilNextShakeAllowed)

    {
        if (secondsuntilNextShakeAllowed < 0)
            secondsuntilNextShakeAllowed = 0;
        nextShakeAllowed = false;
        yield return new WaitForSeconds(secondsuntilNextShakeAllowed);
        nextShakeAllowed = true;
    }
}
[BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
public struct ShakeJob :IJobParallelFor
{
    [Unity.Collections.ReadOnly] public float StartZoomingValue, SpeedInflunceDampeningForAll, xzVelocity;
    public  NativeArray<float>.ReadOnly allTheMinMax;
    [WriteOnly] public NativeArray<float> allTheOutputs;
    public void Execute(int index)
    {
        allTheOutputs[index]  = MathLibary.RemapClamped(StartZoomingValue, SpeedInflunceDampeningForAll,allTheMinMax[index%6], allTheMinMax[index%6+1],xzVelocity);
    }
}