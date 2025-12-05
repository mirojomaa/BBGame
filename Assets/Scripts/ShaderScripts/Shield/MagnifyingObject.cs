using UnityEngine;
public class MagnifyingObject : MonoBehaviour
{
    [SerializeField ]private Renderer rend; 
    [SerializeField] private Camera cam;
    [SerializeField] private Transform trans;
    private Vector3 _screenPoint, transPos;
    private string objScreenPos = "_ObjScreenPos";
    void Update()
    {
        transPos = trans.position;
        trans.forward = cam.transform.position - transPos ; //lookat
        _screenPoint = cam.WorldToScreenPoint(transPos );
        _screenPoint.x /= Screen.width;
        _screenPoint.y /= Screen.height;
       rend.material.SetVector(objScreenPos, _screenPoint);
    }
}
