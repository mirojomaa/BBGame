using System;
using NaughtyAttributes;
using UnityEngine;

    [RequireComponent(typeof(MeshFilter))]
    public class SerializeMesh : MonoBehaviour
    {
        [HideInInspector] [SerializeField] Vector2[] uv;
        [HideInInspector] [SerializeField] Vector3[] verticies;
        [HideInInspector] [SerializeField]  int[] triangles;
        [HideInInspector] [SerializeField] string meshname;

        [Button()]
        public void serilizedads()
        {
            Serialize("Seg ");
        }
        public void Serialize(string counter)
        {
            
            Mesh mesh = null;
            mesh = GetComponent<MeshFilter>().sharedMesh;
            uv = mesh.uv;
            verticies = mesh.vertices;
            triangles = mesh.triangles;
            meshname = counter +  mesh.GetInstanceID().ToString();
            mesh.name = counter + mesh.GetInstanceID().ToString();
            // mesh.vertices = verticies;
            // mesh.triangles = triangles;
            // mesh.uv = uv;
            //
            // mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            // GetComponent<MeshCollider>().sharedMesh = mesh;
            // var mCollider = GetComponent<MeshCollider>().sharedMesh;
            // mCollider.uv = mesh.uv;
            // mCollider.vertices = mesh.vertices;
            // mCollider.triangles = mesh.triangles;
            // mCollider.name = mesh.GetInstanceID().ToString();
            
          
        }
        /*public Mesh Rebuild()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = verticies;
            mesh.triangles = triangles;
            mesh.uv = uv;
           
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
 
            return mesh;
        }*/

        private void Awake()
        {
            Mesh mesh2 = new Mesh();
            mesh2.vertices = verticies;
            mesh2.triangles = triangles;
            mesh2.uv = uv;
           
            mesh2.RecalculateNormals();
            mesh2.RecalculateBounds();
        }
    }