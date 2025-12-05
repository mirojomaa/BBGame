/*
using UnityEngine;
[ExecuteAlways]
#if UNITY_EDITOR
public class SetMPB : MonoBehaviour
{
    private MaterialPropertyBlock mpb;
    private MaterialPropertyBlock MPB
    {
        get
        {
            if (mpb == null) mpb = new MaterialPropertyBlock();
            return mpb;
        }
    }
    static readonly int shPropArrayIndex = Shader.PropertyToID("_ArrayIndexHex");
    [Range(0,3)]
    public int ArrayIndex;
    public void ApplyIndex()
    {
        GameObject childWithMeshFilter = this.transform.GetChild(0).GetChild(0).gameObject;
        MeshRenderer rnd = childWithMeshFilter.GetComponent<MeshRenderer>();
        MPB.SetInt(shPropArrayIndex, ArrayIndex);
        rnd.SetPropertyBlock(MPB);
    }
    private void OnValidate()=> ApplyIndex();
#endif
}
*/