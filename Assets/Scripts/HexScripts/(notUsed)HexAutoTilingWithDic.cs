



/*


public class HexAutoTiling : MonoBehaviour
{
    public static List<GameObject> HexesToBeMoved = new List<GameObject>();
    private HexGrid _hexGrid;
    private int startTilingTreshhold = 130;
    float treshholdMoveBackToOrigin = 1300;
    

     [SerializeField] GameObject playerLocation;
     
     private float xPlusSnapShotPos;
    private float xMinusSnapShotPos;
    private float zPlusSnapShotPos;
    private float zMinusSnapShotPos;

    private float xOriginPosition;
    private float zOriginPosition;

    [Tooltip("ab wann soll er das Tiling anfangen?")] public float tilingTreshold = 307.5f; //default 307.5
    [Tooltip("wie weit soll er die Tiles nach z verschieben?") ] public static float zTilingDistance = 598; //default 438
    [Tooltip("wie weit soll er die Tiles nach xverschieben")] public static  float xTilingDistance = 691; //default 517
    
    

    // Start is called before the first frame update
    void Awake()

    {
        _hexGrid = FindObjectOfType<HexGrid>();
        HexesToBeMoved.Clear();
        HexesToBeMoved.AddRange(GameObject.FindGameObjectsWithTag("Hex"));
        
    }

    void Start()
    {
        xOriginPosition = playerLocation.transform.position.x;
        zOriginPosition = playerLocation.transform.position.z;
    }

    void Update()
 
    {
        /*
         *  foreach (Vector3Int key in hasAllTheHexes.Keys)
        {
            if (key.x > test.transform.position.x)
            {
                hasAllTheHexes.TryGetValue(key, out var value);
                
                Debug.Log(value.transform.position);
               value.transform.position = new Vector3(transform.position.x+5, value.transform.position.y , value.transform.position.z);
               Vector3Int newKey = new Vector3Int(key.x + Mathf.CeilToInt(5 ), key.y, key.z);
               hasAllTheHexes.Remove((key));
               hasAllTheHexes.Add(newKey,value);

            }
            
        }
        
     
    
           
        //snapshot position so it only needs to update at certain distance
        if (HexCoordinates.playerHasMoved)
        {
             xPlusSnapShotPos = playerLocation.transform.position.x + startTilingTreshhold;
             xMinusSnapShotPos = playerLocation.transform.position.x - startTilingTreshhold;
            
             zPlusSnapShotPos = playerLocation.transform.position.z + startTilingTreshhold;
             zMinusSnapShotPos = playerLocation.transform.position.z - startTilingTreshhold;

             HexCoordinates.playerHasMoved = false;
        }
        
        if (playerLocation.transform.position.x > xPlusSnapShotPos  ||
            playerLocation.transform.position.x < xMinusSnapShotPos ||
            playerLocation.transform.position.z > zPlusSnapShotPos  ||
            playerLocation.transform.position.z < zMinusSnapShotPos )
        {
            //the actual hex movement
        foreach (GameObject hex in HexesToBeMoved)
        {   if ( hex == null)
            return;
            if (playerLocation.transform.position.z + tilingTreshold < hex.transform.position.z)
                hex.transform.position = new Vector3(
                    hex.transform.position.x,
                    hex.transform.position.y,
                    hex.transform.position.z - zTilingDistance);
            
            if (playerLocation.transform.position.z - tilingTreshold > hex.transform.position.z)
                hex.transform.position = new Vector3(
                    hex.transform.position.x,
                    hex.transform.position.y,
                    hex.transform.position.z + zTilingDistance);
            
            if (playerLocation.transform.position.x + tilingTreshold < hex.transform.position.x)
                hex.transform.position = new Vector3(
                    hex.transform.position.x - xTilingDistance,
                    hex.transform.position.y,
                    hex.transform.position.z);
            
            if (playerLocation.transform.position.x - tilingTreshold > hex.transform.position.x)
                hex.transform.position = new Vector3(
                    hex.transform.position.x + xTilingDistance,
                    hex.transform.position.y,
                    hex.transform.position.z);

            HexCoordinates.playerHasMoved = true;
        }
        }
    }

    private void LateUpdate()
    {
        ///return to origin
        if ( 
               playerLocation.transform.position.x > treshholdMoveBackToOrigin  
            || playerLocation.transform.position.x < -treshholdMoveBackToOrigin
            || playerLocation.transform.position.z > treshholdMoveBackToOrigin  
            || playerLocation.transform.position.z < -treshholdMoveBackToOrigin  
            )
        {
            Vector3 moveEveryThingBack = new Vector3(
                playerLocation.transform.position.x - (xOriginPosition),
                0,
                playerLocation.transform.position.z - (zOriginPosition)
            );

            int numVcams = CinemachineCore.Instance.VirtualCameraCount;
            for (int i = 0; i < numVcams; ++i)
                CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(
                    playerLocation.transform, -moveEveryThingBack);
            
            for (int j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCount; j++)
            {
                foreach (GameObject allParentObjects in UnityEngine.SceneManagement.SceneManager.GetSceneAt(j).GetRootGameObjects())
                {
                    allParentObjects.transform.position -= moveEveryThingBack;
                    HexCoordinates.playerHasMoved = true;
                }
            }
        }
    }
    
}

*/





















///////////////
/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Linq;

public class HexAutoTiling : MonoBehaviour
{
    public static List<GameObject> HexesToBeMoved = new List<GameObject>();
    private HexGrid _hexGrid;
    private int startTilingTreshhold = 130;
    float treshholdMoveBackToOrigin = 1300;
    

     [SerializeField] GameObject playerLocation;
     
     private float xPlusSnapShotPos;
    private float xMinusSnapShotPos;
    private float zPlusSnapShotPos;
    private float zMinusSnapShotPos;

    private float xOriginPosition;
    private float zOriginPosition;

    [Tooltip("ab wann soll er das Tiling anfangen?")] public float tilingTreshold = 300f; //default 307.5
    [Tooltip("wie weit soll er die Tiles nach z verschieben?") ] public static float zTilingDistance = 598; //default 438
    [Tooltip("wie weit soll er die Tiles nach xverschieben")] public static  float xTilingDistance = 691; //default 517
    
    public  static Dictionary<Vector3Int, GameObject> hasAllTheHexesDic = new Dictionary<Vector3Int, GameObject>();
    public  static Dictionary<Vector3Int, GameObject> hasAllTheHexesDicSnapShot = new Dictionary<Vector3Int, GameObject>();
    // Start is called before the first frame update
    void Awake()

    {
        _hexGrid = FindObjectOfType<HexGrid>();
        HexesToBeMoved.Clear();
        HexesToBeMoved.AddRange(GameObject.FindGameObjectsWithTag("Hex"));
        findHexDic();
        hasAllTheHexesDicSnapShot.Append(hasAllTheHexesDic);

    }

    void findHexDic()
    {
        foreach (GameObject hex in GameObject.FindGameObjectsWithTag("Hex"))
        {
            Vector3Int hexPositon = Vector3Int.RoundToInt(hex.transform.position);
            hasAllTheHexesDic[hexPositon] = hex;
        }
    }
   
    void Start()
    {
        xOriginPosition = playerLocation.transform.position.x;
        zOriginPosition = playerLocation.transform.position.z;
    }

    void Update()
 
    {

        //snapshot position so it only needs to update at certain distance
        if (HexCoordinates.playerHasMoved)
        {
             xPlusSnapShotPos = playerLocation.transform.position.x + startTilingTreshhold;
             xMinusSnapShotPos = playerLocation.transform.position.x - startTilingTreshhold;
            
             zPlusSnapShotPos = playerLocation.transform.position.z + startTilingTreshhold;
             zMinusSnapShotPos = playerLocation.transform.position.z - startTilingTreshhold;

             HexCoordinates.playerHasMoved = false;
        }
        
         if (playerLocation.transform.position.x > xPlusSnapShotPos  ||
            playerLocation.transform.position.x < xMinusSnapShotPos ||
             playerLocation.transform.position.z > zPlusSnapShotPos  ||
            playerLocation.transform.position.z < zMinusSnapShotPos )
         {
          

        Dictionary<Vector3Int, GameObject> hasAllTheHexesDicTemp = new Dictionary<Vector3Int, GameObject>(hasAllTheHexesDic);
        foreach (Vector3Int key in hasAllTheHexesDic.Keys)
        {
            if ((int) playerLocation.transform.position.z + (int) tilingTreshold < key.z)
            {
                Debug.Log("z");
                hasAllTheHexesDicTemp.TryGetValue(key, out var value);
                value.transform.position = new Vector3(
                    value.transform.position.x,
                    value.transform.position.y,
                    value.transform.position.z - zTilingDistance);
                Vector3Int newKey = new Vector3Int(key.x, key.y, key.z - Mathf.CeilToInt(zTilingDistance));
                hasAllTheHexesDicTemp.Remove((key));
                hasAllTheHexesDicTemp.Add(newKey, value);

            }
         
        }
        hasAllTheHexesDic.Clear();
        hasAllTheHexesDic.Append(hasAllTheHexesDicTemp);
        hasAllTheHexesDicTemp.Clear();
      }
        
    }

    private void LateUpdate()
    {
        ///return to origin
        if ( 
               playerLocation.transform.position.x > treshholdMoveBackToOrigin  
            || playerLocation.transform.position.x < -treshholdMoveBackToOrigin
            || playerLocation.transform.position.z > treshholdMoveBackToOrigin  
            || playerLocation.transform.position.z < -treshholdMoveBackToOrigin  
            )
        {
            Vector3 moveEveryThingBack = new Vector3(
                playerLocation.transform.position.x - (xOriginPosition),
                0,
                playerLocation.transform.position.z - (zOriginPosition)
            );

            int numVcams = CinemachineCore.Instance.VirtualCameraCount;
            for (int i = 0; i < numVcams; ++i)
                CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(
                    playerLocation.transform, -moveEveryThingBack);
            
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                foreach (GameObject allParentObjects in SceneManager.GetSceneAt(j).GetRootGameObjects())
                {
                    allParentObjects.transform.position -= moveEveryThingBack;
                    
                }
            }
            HexCoordinates.playerHasMoved = true;
            hasAllTheHexesDic.Clear();
            hasAllTheHexesDic.Append(hasAllTheHexesDicSnapShot);
        }
    }
    
}

public static class Extensions
{
    public static void Append<K, V>(this Dictionary<K, V> first, Dictionary<K, V> second)
    {
        foreach (KeyValuePair<K, V> item in second) {
            first[item.Key] = item.Value;
        }
    }
}






/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class HexAutoTiling : MonoBehaviour
{
    public static List<GameObject> HexesToBeMoved = new List<GameObject>();
    private HexGrid _hexGrid;
    private int startTilingTreshhold = 130;
    float treshholdMoveBackToOrigin = 1300;
    

     [SerializeField] GameObject playerLocation;
     
     private float xPlusSnapShotPos;
    private float xMinusSnapShotPos;
    private float zPlusSnapShotPos;
    private float zMinusSnapShotPos;

    private float xOriginPosition;
    private float zOriginPosition;

    [Tooltip("ab wann soll er das Tiling anfangen?")] public float tilingTreshold = 307.5f; //default 307.5
    [Tooltip("wie weit soll er die Tiles nach z verschieben?") ] public static float zTilingDistance = 598; //default 438
    [Tooltip("wie weit soll er die Tiles nach xverschieben")] public static  float xTilingDistance = 691; //default 517
    
    

    // Start is called before the first frame update
    void Awake()

    {
        _hexGrid = FindObjectOfType<HexGrid>();
        HexesToBeMoved.Clear();
        HexesToBeMoved.AddRange(GameObject.FindGameObjectsWithTag("Hex"));
        
    }

    void Start()
    {
        xOriginPosition = playerLocation.transform.position.x;
        zOriginPosition = playerLocation.transform.position.z;
    }

    void Update()
 
    {
        
           foreach (Vector3Int key in _hexGrid.hexTileDict.Keys)
        {
            if (key.x > playerLocation.transform.position.x)
            {
                _hexGrid.hexTileDict.TryGetValue(key, out var value);
                
                Debug.Log(value.transform.position);
               value.transform.position = new Vector3(transform.position.x+5, value.transform.position.y , value.transform.position.z);
               Vector3Int newKey = new Vector3Int(key.x + Mathf.CeilToInt(5 ), key.y, key.z);
               _hexGrid.hexTileDict.Remove((key));
               _hexGrid.hexTileDict.Add(newKey,value);

            }
            
        }
        
     
    
           
        //snapshot position so it only needs to update at certain distance
        if (HexCoordinates.playerHasMoved)
        {
             xPlusSnapShotPos = playerLocation.transform.position.x + startTilingTreshhold;
             xMinusSnapShotPos = playerLocation.transform.position.x - startTilingTreshhold;
            
             zPlusSnapShotPos = playerLocation.transform.position.z + startTilingTreshhold;
             zMinusSnapShotPos = playerLocation.transform.position.z - startTilingTreshhold;

             HexCoordinates.playerHasMoved = false;
        }
        
        if (playerLocation.transform.position.x > xPlusSnapShotPos  ||
            playerLocation.transform.position.x < xMinusSnapShotPos ||
            playerLocation.transform.position.z > zPlusSnapShotPos  ||
            playerLocation.transform.position.z < zMinusSnapShotPos )
        {
            //the actual hex movement
        foreach (GameObject hex in HexesToBeMoved)
        {   if ( hex == null)
            return;
            if (playerLocation.transform.position.z + tilingTreshold < hex.transform.position.z)
                hex.transform.position = new Vector3(
                    hex.transform.position.x,
                    hex.transform.position.y,
                    hex.transform.position.z - zTilingDistance);
            
            if (playerLocation.transform.position.z - tilingTreshold > hex.transform.position.z)
                hex.transform.position = new Vector3(
                    hex.transform.position.x,
                    hex.transform.position.y,
                    hex.transform.position.z + zTilingDistance);
            
            if (playerLocation.transform.position.x + tilingTreshold < hex.transform.position.x)
                hex.transform.position = new Vector3(
                    hex.transform.position.x - xTilingDistance,
                    hex.transform.position.y,
                    hex.transform.position.z);
            
            if (playerLocation.transform.position.x - tilingTreshold > hex.transform.position.x)
                hex.transform.position = new Vector3(
                    hex.transform.position.x + xTilingDistance,
                    hex.transform.position.y,
                    hex.transform.position.z);

            HexCoordinates.playerHasMoved = true;
        }
        }
    }

    private void LateUpdate()
    {
        ///return to origin
        if ( 
               playerLocation.transform.position.x > treshholdMoveBackToOrigin  
            || playerLocation.transform.position.x < -treshholdMoveBackToOrigin
            || playerLocation.transform.position.z > treshholdMoveBackToOrigin  
            || playerLocation.transform.position.z < -treshholdMoveBackToOrigin  
            )
        {
            Vector3 moveEveryThingBack = new Vector3(
                playerLocation.transform.position.x - (xOriginPosition),
                0,
                playerLocation.transform.position.z - (zOriginPosition)
            );

            int numVcams = CinemachineCore.Instance.VirtualCameraCount;
            for (int i = 0; i < numVcams; ++i)
                CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(
                    playerLocation.transform, -moveEveryThingBack);
            
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                foreach (GameObject allParentObjects in SceneManager.GetSceneAt(j).GetRootGameObjects())
                {
                    allParentObjects.transform.position -= moveEveryThingBack;
                    HexCoordinates.playerHasMoved = true;
                }
            }
        }
    }
    
}
*/


