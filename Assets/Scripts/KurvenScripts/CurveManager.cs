
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
// using System;
// using System.ComponentModel;
// using Unity.Burst;
// using Unity.Burst.CompilerServices;
// using Unity.Collections;
// using Unity.Jobs;
using UnityEditor;
using Object = UnityEngine.Object;

public class CurveManager : MonoBehaviour
{
     #if UNITY_EDITOR
     public static bool drawPathfindingInEditMode = true;
     public static bool updateCurves = true;
     public static bool updatePathfinder;
     [Button("Toogle Pathfinding in Editor")] private void TogglePathfinding()
     {
          drawPathfindingInEditMode = !drawPathfindingInEditMode;
          if (drawPathfindingInEditMode) Debug.Log("Pathfinding now show up in the Editor");
          else Debug.Log("Pathfinding no longer show up in the Editor");
     }
   [Button()] public void UpdateCollider()
     {
          MeshColliderThatNeedRefresh?.Clear();
          foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BrueckeParent"))
          {
               foreach (Segment seg in obj.GetComponentsInChildren<Segment>())
               {
                    if (seg.gameObject.GetComponent<MeshCollider>().sharedMesh == null) continue;
                    if (seg.gameObject.GetComponent<MeshCollider>() != null)
                    {
                         MeshColliderThatNeedRefresh?.Add(seg.gameObject.GetComponent<MeshCollider>());
                         seg.gameObject.GetComponent<MeshCollider>().enabled = false;
                    }		
                        
               }
          }
     }
   [Button] private void getCurveData()
   {
        meshesInfo?.Clear();
        foreach (ChainParent brck in Resources.FindObjectsOfTypeAll<ChainParent>())
        {
             foreach (Segment seg in brck.GetComponentsInChildren<Segment>())
             {
                  if (seg.gameObject.GetComponent<MeshFilter>().sharedMesh == null) continue;
                  Mesh mesh = seg.gameObject.GetComponent<MeshFilter>().sharedMesh;
                  if (mesh.uv.Length > 0)
                  {
                       meshesInfo.Add(new mesh(
                            brck.transform.parent.name + "    -    " + brck.GetComponentInParent<Hex>().gameObject.name,
                            mesh.uv,
                            mesh.vertices,
                            mesh.triangles,
                            mesh.normals,
                            seg.gameObject.GetComponent<Renderer>().sharedMaterial,
                            mesh.bounds));
                  }
             }
        }
        int uv = 0, normal = 0, triangle = 0, vert = 0;
        foreach (mesh count in meshesInfo)
        {
             uv += count.uv.Length * 2;
             normal += count.normal.Length * 3;
             triangle += count.triangle.Length;
             vert += count.vert.Length * 3;
        }
        Debug.LogWarning("Total Vert count: " + vert + " || Total normal count: " + normal + " || Total triangle count: " + triangle + " || Total uv count: " + uv);
        Debug.LogWarning("average verts: " + vert/meshesInfo.Count + " || average normals: " + normal/meshesInfo.Count + " || average triangles: " + triangle/meshesInfo.Count + " || average uvs: " + uv/meshesInfo.Count);
   }
   [InfoBox(
        "The Meshinfo is also only for debug.  You can see how many verts, etc.  get used at the moment. Clear them for faster play mode", EInfoBoxType.Warning)]
   [SerializeField] public List<mesh> meshesInfo;
   [Button] public void ClearCurveData() => meshesInfo?.Clear();

   [Button("Bake Curves - Experimental")] public void BakeCurves()
   {  
        HexAutoTiling hexAutoTiling = FindObjectOfType<HexAutoTiling>();
     
        foreach (ChainParent brck in  Resources.FindObjectsOfTypeAll<ChainParent>())
        {
             brck.gameObject.SetActive(true);
             brck.UpdateMeshes();
        }
        Invoke("Actualbake",0.5f);
   }
   private void Actualbake()
   {
        updateCurves = false;
        HexAutoTiling hexAutoTiling = FindObjectOfType<HexAutoTiling>();
        GameObject parentObject = null;
        foreach (Transform trans in hexAutoTiling.hasAllTheHexGameObjectsTransformsBeforeStart)
        {
             if (trans.gameObject.GetComponentInChildren<Segment>())
                  parentObject  = new GameObject( trans.gameObject.GetComponentInChildren<ChainParent>().gameObject.name +" " + trans.gameObject.name +" - baked");

             foreach (Segment seg in trans.gameObject.GetComponentsInChildren<Segment>())
             {
                  GameObject copyMe = Instantiate(seg.gameObject);
                  copyMe.AddComponent<SerializeMesh>();
                //  copyMe.GetComponent<SerializeMesh>().Serialize("Segement " + counter + " ");
                  DestroyImmediate(copyMe.GetComponent<Segment>());
                copyMe.transform.SetParent(parentObject.transform);
             }
             if (trans.gameObject.GetComponentInChildren<Segment>())
             {
                  parentObject.transform.position = trans.gameObject.GetComponentInChildren<ChainParent>().gameObject
                       .transform.position;

                  parentObject.transform.rotation = trans.gameObject.GetComponentInChildren<ChainParent>().gameObject
                       .transform.rotation;
                  trans.gameObject.GetComponentInChildren<Segment>().gameObject.transform.parent.gameObject
                       .SetActive(false);
#pragma warning disable 618
                  Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Baked" + parentObject.name + ".prefab");
                  PrefabUtility.ReplacePrefab(parentObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
#pragma warning restore 618
             }
        }
   }
   
#endif
     // public  static Queue <int> meshIDS = new Queue <int>();

     // private NativeArray<int> allTheCurvePhysicsBakes;
     [SerializeField] private List<MeshCollider> MeshColliderThatNeedRefresh;
     private void Start()
     {
          foreach (MeshCollider meshCollider in MeshColliderThatNeedRefresh)
          {
               meshCollider.enabled = false;
               meshCollider.enabled = true;
          }    
           
          //Commented out because it doesnt work for some reason...
          // allTheCurvePhysicsBakes = new NativeArray<int>(meshIDS.ToArray(), Allocator.Persistent);
          // BakeJob bakeJob = new BakeJob
          // {
          //     meshIds = allTheCurvePhysicsBakes.AsReadOnly()
          // };
          // bakeJob.Schedule(meshIDS.Count, 14);
          
          
     }
     // private void OnDestroy()
     // {
     //      if(Application.isPlaying)
     //      allTheCurvePhysicsBakes.Dispose();
     // }
     //
}
[System.Serializable]
public struct mesh
{
     public string brückenname;
     public Vector2[] uv;
     public Vector3[] vert;
     public int[] triangle;
     public Vector3[] normal;
     public Material mat;
     public Bounds bounds;
     public mesh(string brückenname, Vector2[] uv, Vector3[] vert, int[] triangle, Vector3[] normal, Material mat, Bounds bounds)
     {
          this.uv = uv;
          this.vert = vert;
          this.triangle = triangle;
          this.normal = normal;
          this.mat = mat;
          this.bounds = bounds;
          this.brückenname= brückenname;
     }
}


// [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
// public struct BakeJob : IJobParallelFor
// { 
//      public NativeArray<int>.ReadOnly meshIds;
//      
//      public void Execute(int index)
//      {
//           Physics.BakeMesh(meshIds[index], false);
//      }
// }
