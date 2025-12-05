using UnityEngine;
public class BringItemGoalMissionInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(ReferenceLibrary.PlayerTag)) ReferenceLibrary.MissionMng.ActiveMissionState.BringItemDelivered(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag))ReferenceLibrary.MissionMng.ActiveMissionState.BringItemDelivered(gameObject);
    }
}