using System;
using UnityEngine;
using NaughtyAttributes;

public class PortalManager : MonoBehaviour
{
    [InfoBox("Careful with those Settings! Better search for Portal and tweak the Settings there", EInfoBoxType.Warning)]
    [BoxGroup("Increase Cam speed")][Range(0f,10000f)][SerializeField] private float SetLastDistanceTreshhold = 3600f;
    [BoxGroup("Increase Cam speed")][Range(0f,0.5f)] [SerializeField] private float SetFollowRoughness = 0.01f;
    [BoxGroup("Increase Cam speed")][Range(0f,8f)] [SerializeField] private float SetlastDistanceSpeedIncreasePercentPerFrame = 1f;
    
    [BoxGroup("Specific Teleport Settings")] [Range(0,200)] [SerializeField] 
    [Tooltip("This is the velocity that the player regardless of force. Adjust the force in the Portal Settings of each Portal")]
    private float SetVelocityAfterTeleport = 70;
    [BoxGroup("Specific Teleport Settings")][Tooltip("How Much does the Speed when entering the Teleport influence the Player? This one is for Reduce")]
    [Range(0, 10)] [SerializeField] private float SetReduceSpeedInfluenceBeforeTeleport = 3;
    [BoxGroup("Specific Teleport Settings")] [Tooltip("How Much does the Speed when entering the Teleport influence the Player? This one is for Increase")]
    [Range(0,10)] [SerializeField] private float SetIncreaseSpeedInfluenceBeforeTeleport = 1;
    //statics
    public static bool CameraTeleportActive = false, StopGiveVelocityBack = true;
    [SerializeField] private GameObject CameraHelperRef;
    public static GameObject CameraHelper;
    public static Transform CameraHelperTransform;
    public static float distanceCamHelperPlayer, cashedVelocity;
    public static float lastDistanceTreshhold, followRoughness, lastDistanceSpeedIncreasePercentPerFrame,
                          velocityAfterTeleport, ReduceSpeedInfluenceBeforeTeleport, IncreaseSpeedInfluenceBeforeTeleport, 
                           distanceBetweenCamHelperAndPlayerCashed, verticalInput, horizontalInput, cashedlerpValue, cashedFovTemp;
    public static Vector3 CamHelperCashedPos;
    private void Awake()
    {    cashedlerpValue = followRoughness;
        CameraHelper = CameraHelperRef;
        CameraHelperTransform = CameraHelper.transform;
        CamHelperCashedPos = CameraHelperTransform.position;
        velocityAfterTeleport = SetVelocityAfterTeleport;
        ReduceSpeedInfluenceBeforeTeleport = SetReduceSpeedInfluenceBeforeTeleport;
        lastDistanceTreshhold = SetLastDistanceTreshhold;
        followRoughness = SetFollowRoughness;
        lastDistanceSpeedIncreasePercentPerFrame = SetlastDistanceSpeedIncreasePercentPerFrame;
        IncreaseSpeedInfluenceBeforeTeleport = SetIncreaseSpeedInfluenceBeforeTeleport;
    }

    public static void cleanStart()
    {
        distanceCamHelperPlayer = 0;
        cashedVelocity = 0;
        distanceBetweenCamHelperAndPlayerCashed = 0;
        verticalInput = 0;
        horizontalInput = 0;
        cashedFovTemp = 0;
    }
    public static void calculateDistanceToPlayer()
    {
        distanceCamHelperPlayer = MathLibary.CalculateDistance(ReferenceLibrary.PlayerPosition, CameraHelperTransform.position);
    }
    public static void MoveCameraHelper()
    {
        CameraHelperTransform.position= Vector3.Lerp(
            CameraHelper.transform.position,
            ReferenceLibrary.Player.transform.position, 
            followRoughness);
    }
    public static void ModifyFov(float zoomOutDuringTeleport )
    {
        if( distanceCamHelperPlayer > distanceBetweenCamHelperAndPlayerCashed/2)
            CameraZoomOut.vcamera.m_Lens.FieldOfView = Mathf.Lerp(CameraZoomOut.vcamera.m_Lens.FieldOfView, cashedFovTemp+zoomOutDuringTeleport, 1*Time.deltaTime);
        else if (distanceCamHelperPlayer< distanceBetweenCamHelperAndPlayerCashed/2)
            CameraZoomOut.vcamera.m_Lens.FieldOfView = Mathf.Lerp(CameraZoomOut.vcamera.m_Lens.FieldOfView, cashedFovTemp, 1*Time.deltaTime);
    }
    public static void PrepareTeleportWithDelay(Vector3 goalPosition, ref bool isThisPortalActive)
    {      
        cashedFovTemp = CameraZoomOut.vcamera.m_Lens.FieldOfView;
        StopGiveVelocityBack = false;
        CameraHelperTransform.position = ReferenceLibrary.PlayerPosition;
        Debug.Log(CameraHelperTransform.position);
        CameraZoomOut.vcamera.LookAt =  CameraHelper.transform; 
        CameraZoomOut.vcamera.Follow = CameraHelper.transform;
        ReferenceLibrary.Player.transform.position = goalPosition;
        distanceBetweenCamHelperAndPlayerCashed = MathLibary.CalculateDistance(CameraHelper, ReferenceLibrary.Player);
        CameraTeleportActive = true;
        isThisPortalActive = true;
    }
    public static void LastDistanceSpeedUp(Byte triggerTreshholdOne, Byte triggerTreshholdTwo)
   {
       if (distanceCamHelperPlayer < lastDistanceTreshhold && distanceCamHelperPlayer >= triggerTreshholdTwo)
           followRoughness = followRoughness * (lastDistanceSpeedIncreasePercentPerFrame/100+1);
       if (distanceCamHelperPlayer < triggerTreshholdOne)
           followRoughness = followRoughness * (lastDistanceSpeedIncreasePercentPerFrame/20+1);
   }
   public static void ZeroOutSpeed(Byte triggerTreshhold)
   {
       if(distanceCamHelperPlayer > Mathf.Abs(triggerTreshhold)) ReferenceLibrary.PlayerRb.velocity = Vector3.zero;
       if (distanceCamHelperPlayer < Mathf.Abs(triggerTreshhold) && StopGiveVelocityBack) 
           StopGiveVelocityBack = !StopGiveVelocityBack;
   }
   public static void TeleportNoDelay(Vector3 goalPosition)
   {
       int numVcams = Cinemachine.CinemachineCore.Instance.VirtualCameraCount;
       for (int i = 0; i < numVcams; ++i)
           Cinemachine.CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(
               ReferenceLibrary.Player.transform, -goalPosition);
       ReferenceLibrary.Player.transform.position = goalPosition;
   }
  public static void ResetValuesAndBoost(Byte triggerTreshhold, float forceWhenNothingIsPressed, ref bool isThisPortalActive)
   {
       if (distanceCamHelperPlayer < Mathf.Abs(triggerTreshhold))
       {
           horizontalInput = Input.GetAxis("Horizontal"); verticalInput = Input.GetAxis("Vertical");
           if (horizontalInput == 0 || verticalInput == 0)
               MathLibary.boostDirection(CameraHelper.transform.position,
                   ReferenceLibrary.Player.transform.position, forceWhenNothingIsPressed,  ReferenceLibrary.PlayerRb);
               
           ReferenceLibrary.PlayerRb.velocity = new Vector3(
               horizontalInput*(velocityAfterTeleport + (cashedVelocity/ReduceSpeedInfluenceBeforeTeleport
                                                                    *IncreaseSpeedInfluenceBeforeTeleport)+forceWhenNothingIsPressed/5f),
               0,
               verticalInput*(velocityAfterTeleport+ (cashedVelocity/ReduceSpeedInfluenceBeforeTeleport
                                                                 *IncreaseSpeedInfluenceBeforeTeleport)+forceWhenNothingIsPressed/5f));
           CameraZoomOut.vcamera.LookAt = ReferenceLibrary.Player.transform;
           CameraZoomOut.vcamera.Follow =  ReferenceLibrary.Player.transform;
           followRoughness = cashedlerpValue;
           distanceCamHelperPlayer = 0;
           CameraHelper.transform.position = CamHelperCashedPos;
           CameraTeleportActive = false;
           isThisPortalActive = false;
           if (!CameraTeleportActive) followRoughness = cashedlerpValue; //setBackFollowSpeed
       }
   }
}