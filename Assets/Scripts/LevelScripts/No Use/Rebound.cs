/*using UnityEngine;
public class Rebound : MonoBehaviour
{
   // [SerializeField] Vector3 velocity;
    [SerializeField] private Vector3 direction;
   // [SerializeField] Vector3 VelocityForRebound;

    [SerializeField] float force = 1;
    bool collided;
    bool savedVelocity = false;

    float timer;
    [SerializeField] float duration = 0.2f;
    
    private void FixedUpdate()
    {
        if (collided)
        { //Addforce Impulse in die Richtung
            timer += Time.deltaTime;
            if(timer < duration) ReferenceLibrary.PlayerRb.AddForce(-direction * force, ForceMode.Impulse);
            else
            {
                timer = 0;
                collided = false;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag))
        {//über Addforce//Vector von wand zum Player -> richtung
            if (!collided)
            {
                collided = true;
                //playerRb.velocity = Vector3.zero;
                direction = (transform.position - ReferenceLibrary.PlayerRb.transform.position).normalized;
            }
        }
        /*
         savedVelocity = true;
        if (collision.gameObject.tag == "Player")
        {
            if( collided == false)
            {
                //collided = true;
                GameObject player = collision.gameObject;
                //velocity = playerRb.velocity;
                VelocityForRebound = new Vector3(Mathf.Abs(velocity.x), 0, Mathf.Abs(velocity.z)); //Mathf.Abs(velocity.y)
                direction = velocity.normalized;
                playerRb.velocity = new Vector3(direction.x * velocity.x, direction.y * velocity.y, direction.z * velocity.z);
                Debug.Log("CangesDirection");
            }
        }
        savedVelocity = false;
        #1#
    }
}*/