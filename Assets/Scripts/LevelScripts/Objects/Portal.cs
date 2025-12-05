using UnityEngine;
using Cinemachine;
public class Portal : MonoBehaviour //Portal in zwei Richtungen, Frei untereinander verlinkbar
{
    private GameObject player;
    private CinemachineVirtualCamera cam = default;
    private bool DelayActive = true;
    [HideInInspector] public bool StartPortal = false;
    [HideInInspector] public bool GoalPortal = false;
    private float distanceBetweenCamHelperAndPlayerSqrtAndHalfed, cashedFovTemp, cashedFovNewCam,
                   cashedVelocity, cashedlerpValue, xzVelocity;
    [SerializeField] AudioSource myAudioSource;
    private Vector3 cashedCamHelperPos;
    [Tooltip ("Link the Goal Portal here")] [SerializeField] GameObject Goal;
    [Range(0,70)]
    [SerializeField] float zoomOutDuringTeleport = 18;
    [Range(25,200)]
    [SerializeField] private float forceWhenNothingIsPressed = 75;
    [SerializeField] ScriptableLevelObject settings;


    void Start()
    {
        cam = CameraZoomOut.vcamera;
        cashedlerpValue = PortalManager.followRoughness;
        if (cam != CameraZoomOut.vcamera) cashedFovNewCam = cam.m_Lens.FieldOfView;
        player = ReferenceLibrary.Player;
        cashedCamHelperPos = PortalManager.CameraHelper.transform.position;
        myAudioSource.clip = settings.Clip;
    }

    private void SetBackFollowSpeed()
    {
        if (!PortalManager.CameraTeleportActive)
            PortalManager.followRoughness = cashedlerpValue;
    }

    
    private void Update()
    {
        if (PortalManager.CameraTeleportActive)
        {
            //Calculate Helper Distance to player
           float distanceCamHelperPlayer =  MathLibary.sqrMagnitudeInlined(ReferenceLibrary.PlayerTransform.position - PortalManager.CameraHelperTransform.position);
           //modifiy fov
           if( distanceCamHelperPlayer > distanceBetweenCamHelperAndPlayerSqrtAndHalfed)
               cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, cashedFovTemp+zoomOutDuringTeleport, 1*Time.deltaTime);
           else if (distanceCamHelperPlayer < distanceBetweenCamHelperAndPlayerSqrtAndHalfed)
               cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, cashedFovTemp, 1*Time.deltaTime);
            //Zero out speed 
           if(distanceCamHelperPlayer > Mathf.Abs(64)) ReferenceLibrary.PlayerRb.velocity = Vector3.zero;
           if (distanceCamHelperPlayer < Mathf.Abs(64) && PortalManager.StopGiveVelocityBack) 
               PortalManager.StopGiveVelocityBack = !PortalManager.StopGiveVelocityBack;
           
           if (distanceCamHelperPlayer < Mathf.Abs(64))
           {
               float horizontalInput = Input.GetAxis("Horizontal");
               float verticalInput = Input.GetAxis("Vertical");
               if (horizontalInput == 0 || verticalInput == 0)
                   MathLibary.boostDirection(PortalManager.CameraHelper.transform.position,
                       ReferenceLibrary.PlayerPosition, forceWhenNothingIsPressed,  ReferenceLibrary.PlayerRb);
               
               ReferenceLibrary.PlayerRb.velocity = new Vector3(
                   horizontalInput*(PortalManager.velocityAfterTeleport + (cashedVelocity/PortalManager.ReduceSpeedInfluenceBeforeTeleport
                                                                        *PortalManager.IncreaseSpeedInfluenceBeforeTeleport)+forceWhenNothingIsPressed/5f),
                   0,
                   verticalInput*(PortalManager.velocityAfterTeleport+ (cashedVelocity/PortalManager.ReduceSpeedInfluenceBeforeTeleport
                                                                     *PortalManager.IncreaseSpeedInfluenceBeforeTeleport)+forceWhenNothingIsPressed/5f)
                   );
         
               cam.LookAt = player.transform;
               cam.Follow =  player.transform;
               PortalManager.followRoughness = cashedlerpValue;
               distanceCamHelperPlayer = 0;
               PortalManager.CameraHelper.transform.position = cashedCamHelperPos;
               PortalManager.CameraTeleportActive = false; SetBackFollowSpeed();
           }
         
            if (distanceCamHelperPlayer < PortalManager.lastDistanceTreshhold && distanceCamHelperPlayer >= 25f)
                PortalManager.followRoughness = PortalManager.followRoughness * (PortalManager.lastDistanceSpeedIncreasePercentPerFrame/100+1);
            if (distanceCamHelperPlayer < 100f)
                PortalManager.followRoughness = PortalManager.followRoughness * (PortalManager.lastDistanceSpeedIncreasePercentPerFrame/20+1);
            
            PortalManager.CameraHelper.transform.position = Vector3.Lerp( 
                PortalManager.CameraHelper.transform.position,
               player.transform.position, 
                PortalManager.followRoughness);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ReferenceLibrary.PlayerTag))
        {
            if (!GoalPortal) 
            {
                cashedVelocity = Mathf.Abs(ReferenceLibrary.PlayerRb.velocity.x + ReferenceLibrary.PlayerRb.velocity.z);
                StartPortal = true; GoalPortal = false;
                Goal.GetComponent<Portal>().GoalPortal = true;
                myAudioSource.clip = settings.Clip;
                myAudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                myAudioSource.Play();
                /*
                                if (!DelayActive)
                                {
                                    int numVcams = CinemachineCore.Instance.VirtualCameraCount;
                                    for (int i = 0; i < numVcams; ++i)
                                        CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(
                                            player.transform, -Goal.transform.position);
                                      player.transform.position = Goal.transform.position;
                                }
                  */
                if (DelayActive && !PortalManager.CameraTeleportActive)
                {
                    cashedFovTemp = cam.m_Lens.FieldOfView;
                    PortalManager.StopGiveVelocityBack = false;
                    PortalManager.CameraHelper.transform.position = player.transform.position;
                        cam.LookAt =  PortalManager.CameraHelper.transform; 
                        cam.Follow =  PortalManager.CameraHelper.transform;
                        player.transform.position = Goal.transform.position;
                        distanceBetweenCamHelperAndPlayerSqrtAndHalfed = MathLibary.CalculateDistanceSquared(PortalManager.CameraHelper, player)/2;
                        PortalManager.CameraTeleportActive = true;
                }
            }
            
            if(GoalPortal)
            {
                StartPortal = false;
                if (!myAudioSource.isPlaying)
                {
                    myAudioSource.clip = settings.Clip2;
                    myAudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                    myAudioSource.PlayDelayed(0.8f);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ReferenceLibrary.PlayerTag)&& GoalPortal)
        {
            StartPortal = false; GoalPortal = false;
        }
    }  
}