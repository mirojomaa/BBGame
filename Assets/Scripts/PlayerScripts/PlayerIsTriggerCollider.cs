using UnityEngine;
public class PlayerIsTriggerCollider : MonoBehaviour
{
    void FixedUpdate() =>transform.position =  ReferenceLibrary.PlayerRb.transform.position;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.gameObject.layer == LayerMask.GetMask("World"))
            Debug.Log("CollidingT");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("World"))
            Debug.Log("CollidingC");
    }
}