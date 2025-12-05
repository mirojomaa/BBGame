using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetUVs : MonoBehaviour
{
    [Range(0, 16)] public int array_index = 0;
    private void OnValidate()
    {
        List<Vector3> uvs = new List<Vector3>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.GetUVs(0, uvs);
        for(int i = 0;  i<uvs.Count; i++)
        uvs[i] = new Vector3(uvs[i].x, uvs[i].y, array_index);
        mesh.SetUVs(0,uvs);
    }
}