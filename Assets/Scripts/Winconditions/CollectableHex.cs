using UnityEngine;
public class CollectableHex : MonoBehaviour
{
    [SerializeField] private float rotation = 20;
    [HideInInspector] public GameObject ParentHex;
    void Update() => transform.Rotate(new Vector3(0, rotation * Time.deltaTime, 0));
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("2");
        if (collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag)) 
            ReferenceLibrary.WinconMng.CheckForWinConHex(); Debug.Log("1");
    }
}