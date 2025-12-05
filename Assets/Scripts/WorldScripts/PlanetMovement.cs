using UnityEngine;
public class PlanetMovement : MonoBehaviour
{
    float horizontal, vertical = 0;
    [SerializeField] float speed = 50;
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        transform.RotateAround(transform.position, transform.up, -horizontal * speed * Time.deltaTime);
        transform.RotateAround(transform.position, transform.right, -vertical * speed * Time.deltaTime);
        //this.transform.Rotate(new Vector3(-vertical * speed * Time.deltaTime, 0, horizontal * speed * Time.deltaTime), Space.Self);
    }
}
