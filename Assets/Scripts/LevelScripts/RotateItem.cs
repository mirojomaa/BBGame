using UnityEngine;
public class RotateItem : MonoBehaviour
{
    #region Inspector
    [SerializeField] float rotation = 18;
    public bool RotationEnabled = true;
    #endregion
    void Update()
    {
        if (RotationEnabled == false) return;
        transform.Rotate(new Vector3(0, rotation * Time.deltaTime, 0));
    }
}
