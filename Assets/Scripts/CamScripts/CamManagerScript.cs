/*using UnityEngine;
public class CamManagerScript : MonoBehaviour
{
    [SerializeField] GameObject Player;
    //[SerializeField] GameObject Planet;
    [HideInInspector] public Vector3 gravityDirection;
    [SerializeField] float followSpeed = 5f;
    void Start()
    {
        transform.position = Vector3.Lerp(transform.position, Player.transform.position, followSpeed * Time.deltaTime);
       // gravityDirection = (transform.position - Planet.transform.position).normalized;
    }
    void Update()
    {
        //Smooth
        //Position :> Follow Player
        transform.position = Vector3.Lerp(transform.position, Player.transform.position, followSpeed * Time.deltaTime);
        //gravityDirection = (transform.position - Planet.transform.position).normalized;
        //automatisches mitrotieren der Kamera
        Quaternion toRotation = Quaternion.FromToRotation(transform.up, gravityDirection) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.1f);
    }
    //public void NewPlanet(GameObject newPlanet)
    //{
    //  Planet = newPlanet;
    //}
}*/