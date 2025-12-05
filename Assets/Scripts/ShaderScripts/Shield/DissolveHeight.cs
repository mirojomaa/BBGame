using UnityEngine;
public class DissolveHeight : MonoBehaviour
{
    private Material mat;
    string _DissolveStartHeight = "_DissolveStartHeight";
    void Update() => mat.SetFloat(_DissolveStartHeight, transform.position.y);
}