using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slam : MonoBehaviour
{
    [SerializeField] private GameObject Slamparticle;
    [SerializeField] private Transform SpawnPoint;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            
            Instantiate (Slamparticle, SpawnPoint.position, SpawnPoint.rotation);
            //Slamparticle.SetActive(false);
            //Slamparticle.SetActive(true);
           
        }
    }
}
