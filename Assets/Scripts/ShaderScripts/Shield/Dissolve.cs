using System.Collections;
using UnityEngine;
public class Dissolve : MonoBehaviour
{
    [SerializeField] private Renderer rend; 
    [SerializeField] private float DisolveSpeed = 0.8f;
    private string dissolve = "_Disolve";
    public IEnumerator Coroutine_DisolveShield(float target)
    {
        Debug.Log("Dissolving");
        float start = rend.material.GetFloat(dissolve);
        float lerp = 0;
        while (lerp < 1)
        {
            rend.material.SetFloat(dissolve, Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * DisolveSpeed;
            yield return null;
        }
        Debug.Log("Dissolving END");
    }
}