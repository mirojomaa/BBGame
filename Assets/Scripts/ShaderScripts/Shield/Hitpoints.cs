using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hitpoints : MonoBehaviour
{
   /*// public List<Vector3> pointsOut = new List<Vector3>();
   public Vector3 pointOut;
    void OnCollisionEnter(Collision other)
    {
        if( other.gameObject.layer == 6){
        // Print how many points are colliding with this transform
        Debug.Log("Points colliding: " + other.contacts.Length);

        // Print the normal of the first point in the collision.
        Debug.Log("Normal of the first point: " + other.contacts[0].normal);
        Debug.Log("test: " + other.contacts[0].point);
        pointOut = other.contacts[0].point;
        pointOut  = new Vector3(pointOut.x, pointOut.y + 10, pointOut.z);
        // Draw a different colored ray for every normal in the collision
        foreach (var item in other.contacts)
        {
            Debug.DrawRay(item.point, item.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        }
        }
    }*/
}