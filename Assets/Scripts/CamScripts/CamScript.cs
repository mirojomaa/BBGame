using UnityEngine;
public class CamScript : MonoBehaviour
{
    public GameObject target;
    [SerializeField] GameObject RotationObj;
    float sensitivity = 17f, minFov = 35,maxFov = 100;
    public float xValue = 0, zValue = 0;
    [SerializeField] float rotationSpeed = 0.2f;
    [SerializeField] float rotationSpeedMouse = 3.5f;
    [SerializeField] Quaternion standartRotation;
    public bool cameraRotated = false;
    //[SerializeField] Vector3 eulerRotation;
    // Rotation not used
    void CameraRotation()
    {
        // Rotation
        if (Input.GetMouseButton(1)) //Maus
        {
            cameraRotated = true;
            transform.RotateAround(target.transform.position, transform.up, Input.GetAxis("Mouse X") * rotationSpeedMouse);
            transform.RotateAround(target.transform.position, transform.right, -Input.GetAxis("Mouse Y") * rotationSpeedMouse);
        }
        zValue = Input.GetAxis("HorizontalJoystickAxis");
        xValue = Input.GetAxis("VerticalJoystickAxis");
        if (Input.GetButton("LBJanina")) //Controller
        {
            cameraRotated = true;
            RotationObj.transform.Rotate(new Vector3(-xValue * rotationSpeed * Time.deltaTime, 0, -zValue * rotationSpeed * Time.deltaTime), Space.Self);
            /*
            transform.RotateAround(target.transform.position, transform.up, -yRotationValue * rotationSpeed);
            transform.RotateAround(target.transform.position, transform.right, -xRotationValue * rotationSpeed);
            */
        }
        if (cameraRotated == true)
        {//Reset Rotation
            if (Input.GetButtonDown("RBJanina") || Input.GetMouseButtonDown(2)) ResetCamera();
        }
        /*
        if(yRotationValue >0) transform.RotateAround(target.transform.position, transform.right, yRotationValue * rotationSpeed);
        //this.transform.Rotate(new Vector3(xRotationValue * rotationSpeed * Time.deltaTime, yRotationValue * rotationSpeed * Time.deltaTime, 0), Space.World);
        */
        //zoom
        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * -sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }
    public void ResetCamera()
    {
        /*Quaternion rotation = Quaternion.Euler(RotationObj.transform.localRotation.x, RotationObj.transform.localRotation.y, RotationObj.transform.localRotation.z);
        eulerRotation = rotation.eulerAngles;
        //RotationObj.transform.Rotate(new Vector3(+eulerRotation.x, +eulerRotation.y, +eulerRotation.z), Space.Self);
        */
        RotationObj.transform.rotation = standartRotation;
        cameraRotated = false;
    }
}