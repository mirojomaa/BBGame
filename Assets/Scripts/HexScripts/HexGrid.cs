/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{   
    private HexCoordinates hexCoordinates;
    private HexAutoTiling _hexAutoTiling;
    #region Dictonarys
    
    public  Dictionary<Vector3Int, Hex> hexTileDict = new Dictionary<Vector3Int, Hex>();
   protected Dictionary<Vector3, List<Vector3>> hexTileNeighboursDict = new Dictionary<Vector3, List<Vector3>>();
    
    #endregion
    
    private void Start()
    {

        _hexAutoTiling = FindObjectOfType<HexAutoTiling>();
    }
  
   // public Dictionary<Vector3Int, Hex> HexTileDict
  //  {
     //   get => hexTileDict;
     //   set => hexTileDict = value;
   // }
    #region GetNeighbours


public GameObject GetTileAt(Vector3 hexCoordinates)
    {
        var result = _hexAutoTiling.hasAllTheHexesDic[hexCoordinates.y];
        return result;
    }

    public List<Vector3> GetNeighboursFor(Vector3 hexCoordinates)
    {
        if (_hexAutoTiling.hasAllTheHexesDic.ContainsKey(hexCoordinates.y) == false)
            return new List<Vector3>();

        if (_hexAutoTiling.hasAllTheHexesDic.ContainsKey(hexCoordinates.y))
            return hexTileNeighboursDict[hexCoordinates];

        hexTileNeighboursDict.Add(hexCoordinates, new List<Vector3>());

        foreach (Vector3 direction in Direction.GetDirectionList(hexCoordinates.z))
        { 
            if (_hexAutoTiling.hasAllTheHexPos.Contains(hexCoordinates+ direction))
            {
                hexTileNeighboursDict[hexCoordinates].Add(hexCoordinates + direction);
            }
        }
        return hexTileNeighboursDict[hexCoordinates];
    }

    // public Vector3 GetClosestHex(Vector3 worldposition)
    // {
    //     worldposition.y = 0;
    //     return HexCoordinates.ConvertPositionToOffset(worldposition);
    // }
}

#endregion

#region EvenandUnvenNeighbours
public static class Direction
{
    public static List<Vector3> directionsOffsetOdd = new List<Vector3>
    {
        new Vector3(-1,0,1), //North 1
        new Vector3(0,0,1), //North 2
        new Vector3(1,0,0), //East
        new Vector3(0,0,-1), //South2 
        new Vector3(-1,0,-1), //South 1
        new Vector3(-1,0,0), //West
    };

    public static List<Vector3> directionsOffsetEven = new List<Vector3>
    {
        new Vector3(0,0,1), //North 1
        new Vector3(1,0,1), //North 2
        new Vector3(1,0,0), //East
        new Vector3(1,0,-1), //South2
        new Vector3(0,0,-1), //South 1
        new Vector3(-1,0,0), //West
    };

    public static List<Vector3> GetDirectionList(float z)
        => z % 2 == 0 ? directionsOffsetEven : directionsOffsetOdd;
   
}

#endregion
*/
