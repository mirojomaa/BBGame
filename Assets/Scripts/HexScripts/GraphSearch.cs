/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    public static BFSResult BFSGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        #region Dictonarys
        
        Dictionary<Vector3, Vector3?> visitedNodes = new Dictionary<Vector3, Vector3?>();
        Dictionary<Vector3, int> costSoFar = new Dictionary<Vector3, int>();
        Queue<Vector3> nodesToVisitQueue = new Queue<Vector3>();
        
        #endregion

        nodesToVisitQueue.Enqueue(startPoint);
        costSoFar.Add(startPoint, 0);
        visitedNodes.Add(startPoint, null);

        while (nodesToVisitQueue.Count > 0)
        {
            Vector3 currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3 neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
               // if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                  //  continue;

               // int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
               int nodeCost = 5;
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;

                if (newCost <= movementPoints)
                {
                    if (!visitedNodes.ContainsKey(neighbourPosition))
                    {
                        visitedNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar[neighbourPosition] > newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        visitedNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        return new BFSResult { visitedNodesDict = visitedNodes };
    }
    

    public static List<Vector3> GeneratePathBFS(Vector3 current, Dictionary<Vector3, Vector3?> visitedNodesDict)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(current);
        while (visitedNodesDict[current] != null)
        {
            path.Add(visitedNodesDict[current].Value);
            current = visitedNodesDict[current].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}
#region Struct

public struct BFSResult
{
    public Dictionary<Vector3, Vector3?> visitedNodesDict;

    public List<Vector3> GetPathTo(Vector3 destination)
    {
        if (visitedNodesDict.ContainsKey(destination) == false)
            return new List<Vector3>();
        return GraphSearch.GeneratePathBFS(destination, visitedNodesDict);
    }

    public bool IsHexPositionInRange(Vector3 position)
    {
        return visitedNodesDict.ContainsKey(position);
    }

    public IEnumerable<Vector3> GetRangePositions()
        => visitedNodesDict.Keys;
}

#endregion*/