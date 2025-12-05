using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringItemItemMissionInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ReferenceLibrary.MissionMng.ActiveMissionState.BringItemCollected(this.gameObject);

        }
    }
}
