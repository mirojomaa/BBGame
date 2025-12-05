using UnityEngine;
public class RenderBoundsGizmos : MonoBehaviour
{
    #if UNITY_EDITOR
    public void  OnDrawGizmos()
    {
        if (RenderBounds.RenderBoundsList != null && RenderBounds.RenderBoundsList.Count == 0) return;
        Gizmos.color = Color.magenta;
        Mesh m;
        foreach (var mf in RenderBounds.RenderBoundsList)
        {
            Gizmos.matrix = mf.transform.localToWorldMatrix;
            if (mf.sharedMesh != null)
            {
                m = mf.sharedMesh;
                Gizmos.DrawWireCube(m.bounds.center, m.bounds.size);
            }
        }
    }
#endif
}

