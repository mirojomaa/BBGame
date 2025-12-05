#region Imports
using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using Unity.Burst;
using UnityEngine.Jobs;
#endregion
public class HexAutoTiling : MonoBehaviour
{
    #region Arrays
    private TransformAccessArray hasAllTheHexesTransformsNative;
    [NaughtyAttributes.InfoBox("To avoid thousands of || FindGameObjectsWithTag ||, we just have the list ready, copy to native and then null the list")]
    [SerializeField] public Transform[] hasAllTheHexGameObjectsTransformsBeforeStart;
   #endregion
   
   #region PrivateVariables
   private byte shortCircutToOrginCounter = 0;
   private float xPlusSnapShotPos, xMinusSnapShotPos, 
                  zPlusSnapShotPos, zMinusSnapShotPos,
                  zPlus, zMinus, xPlus, xMinus,
                  xMoveBack, zMoveback;

   [SerializeField] private float  xOriginPosition; 
   [SerializeField] private float zOriginPosition;
     private bool leftMove, rightMove, topMove, bottomMove, playerHasMoved, noSide;

     [SerializeField]private byte  startTilingTreshhold = 130, //später im Inspector bei mehr Level: default 150
                    declineBothSidesTreshhold = 10, //später im Inspector bei mehr Level: default 10
                    shortCircutTreshhold = 8; //später im Inspector bei mehr Level: default 8

    
     
    [Tooltip("ab wann soll er das Tiling anfangen?")] [SerializeField] private float tilingTreshold = 280f; //default 307.5

    public static ushort zTilingDistance = 598, //default 598
                         xTilingDistance = 691, //default 691
                         moveBackToOriginTreshhold = 1600; //später im Inspector bei mehr Level: default 1600
    #endregion
    
    #region Expressionbodys
    private bool tilingDistanceCheck => ReferenceLibrary.PlayerPosition.x > xPlusSnapShotPos
                                        || ReferenceLibrary.PlayerPosition.x < xMinusSnapShotPos
                                        || ReferenceLibrary.PlayerPosition.z > zPlusSnapShotPos
                                        || ReferenceLibrary.PlayerPosition.z < zMinusSnapShotPos;
    private bool returnToOriginDistanceCheck => shortCircutToOrginCounter > shortCircutTreshhold &&
                                        ReferenceLibrary.PlayerPosition.x > moveBackToOriginTreshhold
                                        || ReferenceLibrary.PlayerPosition.x < -moveBackToOriginTreshhold
                                        || ReferenceLibrary.PlayerPosition.z > moveBackToOriginTreshhold
                                        || ReferenceLibrary.PlayerPosition.z < -moveBackToOriginTreshhold;
    #endregion
    #region UnityUpdates
#if UNITY_EDITOR
    [NaughtyAttributes.Button()] public void FindAllTheHexesTransform()
    {
        if (Application.isPlaying) return;
        if(hasAllTheHexGameObjectsTransformsBeforeStart !=null)
        Array.Clear(hasAllTheHexGameObjectsTransformsBeforeStart, 0,
            hasAllTheHexGameObjectsTransformsBeforeStart.Length);
        int counter = 0;
        foreach (GameObject hex in GameObject.FindGameObjectsWithTag("Hex"))
        {
            hasAllTheHexGameObjectsTransformsBeforeStart[counter] = hex.transform;
            counter++;
        }
        
    }

    [NaughtyAttributes.Button()] public void SetPlayerPositionOnStart()
    {
        Transform player = FindObjectOfType<PlayerMovement>().gameObject.transform;
        xOriginPosition = player.position.x;
        zOriginPosition = player.position.z;
    }
#endif
    private void Awake()
    {
        hasAllTheHexesTransformsNative  = new TransformAccessArray(hasAllTheHexGameObjectsTransformsBeforeStart, 28);  //copy over all data from existing array
        hasAllTheHexGameObjectsTransformsBeforeStart = null; //null the existing array since it is no longer needed
    }
    void Start()
    {
        playerHasMoved = true;
        moveHexes();
    }
    void Update()
    {
        setFlags();
        if (bottomMove || topMove || leftMove || rightMove) moveHexes();
        limitTiling();
    }
    private void LateUpdate()
    {
        if (returnToOriginDistanceCheck)
        { 
            moveEverythingBackToOrigin();
            StartCoroutine(updateAllAfterOrigin(0.2f));
        }
    }
    #endregion
    
    #region TilingRules
    void setFlags()
    {   
        if(tilingDistanceCheck)  
        {//only one if is less heavy + make sure that everything gets checked once
            if (ReferenceLibrary.PlayerPosition.x > xPlusSnapShotPos)
            {
                rightMove = true; compareTopBottom();
            }
            
            if (ReferenceLibrary.PlayerPosition.x < xMinusSnapShotPos)
            {
                leftMove = true; compareTopBottom();
            }

            if (ReferenceLibrary.PlayerPosition.z > zPlusSnapShotPos)
            { 
                topMove = true; compareRightLeft(); 
            }
            
            if (ReferenceLibrary.PlayerPosition.z < zMinusSnapShotPos)
            {
                bottomMove = true; compareRightLeft();
            }
        }
    }
    void compareTopBottom()
    {
         zPlus = Mathf.Abs(zPlusSnapShotPos - ReferenceLibrary.PlayerPosition.z);
         zMinus = Mathf.Abs(zMinusSnapShotPos - ReferenceLibrary.PlayerPosition.z);
         noSide = zPlus > declineBothSidesTreshhold;
        
        if ( noSide && zPlus < zMinus ) topMove = true;
        if (noSide && zPlus > zMinus) bottomMove = true;
    }
    void compareRightLeft()
    {
        xPlus = Mathf.Abs(xPlusSnapShotPos - ReferenceLibrary.PlayerPosition.x);
        xMinus = Mathf.Abs(xMinusSnapShotPos - ReferenceLibrary.PlayerPosition.x);
       noSide = xPlus > declineBothSidesTreshhold;
        
        if ( noSide && xPlus < xMinus) rightMove = true;
        if ( noSide && xPlus > xMinus) leftMove = true;
    }
    void setAllFalse()
    {
        bottomMove = false; topMove = false; 
        rightMove = false; leftMove = false;
    }
    void limitTiling() 
    { //snapshot position so it only needs to update at certain distance
        if (playerHasMoved)
        {
            xPlusSnapShotPos = ReferenceLibrary.PlayerPosition.x + startTilingTreshhold;
            xMinusSnapShotPos = ReferenceLibrary.PlayerPosition.x - startTilingTreshhold;
            zPlusSnapShotPos = ReferenceLibrary.PlayerPosition.z + startTilingTreshhold;
            zMinusSnapShotPos = ReferenceLibrary.PlayerPosition.z - startTilingTreshhold;
            playerHasMoved = false;
            shortCircutToOrginCounter++;
        }
    }
    void moveHexes()
    {
        HexPosJob hexTransformJob = new HexPosJob
        {
            xTilingDistanceJob =  xTilingDistance, 
            zTilingDistanceJob = zTilingDistance, tilingTreshholdJob = tilingTreshold,
            bottomMoveJob = bottomMove, rightMoveJob = rightMove, leftMoveJob = leftMove, topMoveJob = topMove,
            playerLocationXJob = ReferenceLibrary.PlayerPosition.x,
            playerLocationZJob = ReferenceLibrary.PlayerPosition.z,
            
        };
        hexTransformJob.Schedule(hasAllTheHexesTransformsNative);
         playerHasMoved = true;
           setAllFalse();
    }
    #endregion
    #region  OriginMethods
    void moveEverythingBackToOrigin()
    {
        Vector3 moveEveryThingBack = new Vector3(
            ReferenceLibrary.PlayerPosition.x - xOriginPosition,
            0,
            ReferenceLibrary.PlayerPosition.z - zOriginPosition);
        //Informs vcams
        int numVcams = CinemachineCore.Instance.VirtualCameraCount;
        for (byte i = 0; i < numVcams; ++i)
            CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(
                ReferenceLibrary.PlayerTransform, -moveEveryThingBack);
        //moves everything back
        for (ushort j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCount; j++)
        { 
            foreach (GameObject allParentObjects in UnityEngine.SceneManagement.SceneManager.GetSceneAt(j).GetRootGameObjects()) 
                allParentObjects.transform.position -= moveEveryThingBack;
        }
        //sets flags
        shortCircutToOrginCounter = 0;
    }
    IEnumerator updateAllAfterOrigin(float sec)
    { 
        yield return new WaitForSeconds(sec);
        setEverythingTrue();
        moveHexes();
        playerHasMoved = true;
    }
    void setEverythingTrue()
    {
        topMove = true; bottomMove = true;
        leftMove = true; rightMove = true;
    }
    #endregion
    private void OnDestroy() => hasAllTheHexesTransformsNative.Dispose();
}


[BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
public struct HexPosJob : IJobParallelForTransform
{ 
    [Unity.Collections.ReadOnly] public ushort xTilingDistanceJob, zTilingDistanceJob ;
  [Unity.Collections.ReadOnly] public float tilingTreshholdJob;
  [Unity.Collections.ReadOnly]  public bool bottomMoveJob, rightMoveJob, leftMoveJob, topMoveJob;
  [Unity.Collections.ReadOnly]  public float playerLocationXJob, playerLocationZJob;
  private ushort hor, hor2, vert, vert2;
  private bool markDirtyVector;
    public void Execute(int index, TransformAccess hasAllTheHexPosTransform)
    {
        hor = 0; hor2 = 0; vert = 0; vert2 = 0;
         markDirtyVector = false;
        if (rightMoveJob  && playerLocationXJob - tilingTreshholdJob > hasAllTheHexPosTransform.position.x)
            {
                hor2 = xTilingDistanceJob;
                markDirtyVector = true;
            }
            if (bottomMoveJob && playerLocationZJob + tilingTreshholdJob < hasAllTheHexPosTransform.position.z)
            {
                vert = zTilingDistanceJob;
                markDirtyVector = true;
            }
            if (topMoveJob && playerLocationZJob - tilingTreshholdJob > hasAllTheHexPosTransform.position.z)
            {
                vert2 = zTilingDistanceJob;
                markDirtyVector = true;
            }
            if (leftMoveJob  && playerLocationXJob + tilingTreshholdJob < hasAllTheHexPosTransform.position.x)
            {
                hor = xTilingDistanceJob;
                markDirtyVector = true;
            }
            if (markDirtyVector)
                hasAllTheHexPosTransform.position = new Vector3(
                    hasAllTheHexPosTransform.position.x - hor + hor2,
                    hasAllTheHexPosTransform.position.y,
                    hasAllTheHexPosTransform.position.z - vert + vert2);
    }
}