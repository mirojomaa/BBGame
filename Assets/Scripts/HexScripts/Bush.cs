using System.Collections;
using UnityEngine;
public class Bush : MonoBehaviour
{
    [SerializeField] private int headshakes = 4;
    [SerializeField] private float rotationAngle = 80, rotDuration = 0.3f, force = 5;
    private float angles;
    private bool rotationAllowed = true;
    private int maxHeadshakes;
    private Transform thisGameObject;
    private void Awake()
    {
        maxHeadshakes = headshakes;
        thisGameObject = gameObject.transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(ReferenceLibrary.PlayerTag))
        {
            Vector3 posOther = other.transform.position;
            angles = Vector3.Angle(posOther, thisGameObject.transform.position)*100;
            thisGameObject.gameObject.transform.Rotate(0,angles,0,Space.Self);
            if (rotationAllowed) StartCoroutine(Rotate(thisGameObject.gameObject,headshakes,rotDuration, rotationAngle , Vector3.down));
        }
    }
    public IEnumerator Rotate(GameObject rotateMe,int headshakes , float duration, float angle, Vector3 firstDirection)
    {
        Quaternion startRot = rotateMe.transform.rotation;
        rotationAllowed = false;
        if (headshakes == 0)
        {
            rotationAllowed = true;
            yield break;
        }
        if (headshakes == maxHeadshakes)
            startRot = Quaternion.Euler(rotateMe.transform.rotation.x,angles,
                rotateMe.transform.rotation.z);
    
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            rotateMe.transform.rotation = startRot * Quaternion.AngleAxis(t / duration * angle, firstDirection);
            yield return null;
        }
        headshakes--;
        yield return Rotate(rotateMe,headshakes,Random.Range(1.2f,1.4f)*duration,
            Random.Range(angle, angle + 5), -firstDirection);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == ReferenceLibrary.Player)
        {
            Rigidbody rb = ReferenceLibrary.PlayerRb;
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 4, rb.velocity.z);
            Vector3 movementDirection = rb.velocity.normalized;
            float timer = 0;
            while (timer <= 0.3f)
            {
                rb.AddForce(movementDirection * force * Time.deltaTime, ForceMode.Force);
                timer+= Time.deltaTime;
            }
            rb.AddForce(movementDirection * force * 100 *Time.deltaTime, ForceMode.Force);
        }
    }
}