using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    //#region OffsetValues
    
  //  public static float xOffset = 2, yOffset = 1, zOffset = 1.73f;
    
   // internal static bool playerHasMoved = true;
    /*
    internal Vector3Int GetHexCoords()
        => offsetCoordinates;
    
    [Header("Offset coordinates")] [SerializeField] public Vector3Int offsetCoordinates;
    
    #endregion
    

    private void Awake()
    {
        offsetCoordinates = ConvertPositionToOffset(transform.position);
    }

   // void Update()
  //  {

       // if (playerHasMoved == false)
       // {
        //    offsetCoordinates = ConvertPositionToOffset(transform.position);
     //   }
   // }
    #region OffsetHexesRound
    public static Vector3Int ConvertPositionToOffset(Vector3 position)
    {
        int x = Mathf.CeilToInt(position.x / xOffset);
        int y = Mathf.RoundToInt(position.y / yOffset);
        int z = Mathf.RoundToInt(position.z / zOffset);
        return new Vector3Int(x, y, z);
    }
    
    #endregion */
}
