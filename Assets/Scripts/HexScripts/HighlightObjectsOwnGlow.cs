using System.Collections;
using UnityEngine;
using NaughtyAttributes;
public class HighlightObjectsOwnGlow : MonoBehaviour
{
    [BoxGroup("Glow Einstellungen")][SerializeField]  private float glowAmount, CutoffEmissive;
     [HideInInspector] public float cashedEmissiveIntensity, cashedEmissiveCutoff;
    private MaterialPropertyBlock mpb;
    private MaterialPropertyBlock MPB
    {
        get
        {
            if (mpb == null) mpb = new MaterialPropertyBlock();
            return mpb;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(ReferenceLibrary.PlayerTag))
        {
            StartCoroutine(EnableHighlightDelayed());
            StartCoroutine(DisableHighlightDelayed());
        }
    }
    IEnumerator EnableHighlightDelayed()
    {
        yield return new WaitForSeconds(Highlightmanager.GlowEnableDelayObjects);
        MPB.SetFloat(Highlightmanager.propIDEmissiveIntensity, glowAmount );
        MPB.SetFloat(Highlightmanager.propIDEmissiveCutoff, CutoffEmissive );
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(MPB);
    }
    IEnumerator DisableHighlightDelayed()
    {
        yield return new WaitForSeconds(Highlightmanager.GlowDisableDelayObjects);
        MPB.SetFloat(Highlightmanager.propIDEmissiveIntensity, cashedEmissiveIntensity );
        MPB.SetFloat(Highlightmanager.propIDEmissiveCutoff, cashedEmissiveCutoff );
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(MPB);
    }
}
