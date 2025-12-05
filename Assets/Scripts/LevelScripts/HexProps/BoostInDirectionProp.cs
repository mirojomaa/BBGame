using UnityEngine;
[ExecuteInEditMode]
public class BoostInDirectionProp : MonoBehaviour
{
    public Hex MyHex;
    Vector3 Direction;
    Quaternion desiredRot;
    //  void Start()
   // {
      //  MyHex = this.transform.parent.transform.parent.GetComponent<Hex>(); //sp�ter �ber instatniaten zuweisen
      //  }
      void Update()
      {
          if (Application.isPlaying) return;
        SetDesiredRotation();
        RotateTowardsDesiredPos();
    }
    void SetDesiredRotation()
    {
        Direction = new Vector3(MyHex.XDirection, 0, MyHex.ZDirection);
        desiredRot = Quaternion.LookRotation(Direction, Vector3.up);
    }
    void RotateTowardsDesiredPos()
    {
       // this.transform.rotation = Quaternion.RotateTowards(from: this.transform.rotation, to: desiredRot, maxDegreesDelta: angularSpeed * Time.deltaTime);
       transform.rotation = desiredRot;
    }
}
