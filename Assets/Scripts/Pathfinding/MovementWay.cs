using System.Collections.Generic;
using UnityEngine;
public class MovementWay : MonoBehaviour
{
    public List<GameObject> Waypoints = new List<GameObject>();
    int childCount = 0;
    void Start()
    {
        childCount = transform.childCount;
        for (int i = 0; i <= childCount-1; i++)
        {
            GameObject toAdd = this.transform.GetChild(i).gameObject;
            Waypoints.Add(toAdd);
        }
    }
}