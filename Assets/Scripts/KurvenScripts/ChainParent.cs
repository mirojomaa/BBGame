using System;
using UnityEngine;
using System.Linq;
using UnityEngine.PlayerLoop;

// Das ist der ParentKontroller von allen Segmenten
[ExecuteInEditMode]
public class ChainParent : MonoBehaviour
{
	[Tooltip("Die Form vom 2d Mesh das extrudiert wird als Scriptable Object")] public Mesh2D mesh2D = null; // Das 2d Shape, dass extrudiert wird
	[Tooltip("Falls das ganze Konstrukt geschlossen werden soll")] public bool closed; // Falls die Strecke geschlossen werden soll
	[Tooltip("Trianguliert das Mesh mehr. Generiert Edge Loops")] public float MeshDensity= 2; // Density vom Mesh
	[Tooltip("Die Art wie die UV's projeziert werden")]public UVMode uvMode = UVMode.TiledWithFix; // Das ist für das UV

	
	private void Awake()
	{
		UpdateMeshes();
	}
	// Regenerate meshes beim instanziieren  // eventuell später fertig speichern
#if UNITY_EDITOR
	private void OnValidate() => CurveManager.updateCurves = true;

	private void OnDrawGizmos()
	{
		if(CurveManager.updateCurves)
		{
			CurveManager.updateCurves = false;
			UpdateMeshes();
		}
	}
	public void OnDrawGizmosSelected()
	{
		CurveManager.updateCurves = true;
		Segment[] allSegments = GetComponentsInChildren<Segment>();
		Segment[] segmentsWithMesh = allSegments.Where( s => s.HasValidNextPoint ).ToArray();
		Segment[] segmentsWithoutMesh = allSegments.Where( s => s.HasValidNextPoint == false ).ToArray();
		foreach( Segment seg in segmentsWithMesh  ) Gizmos.DrawSphere(seg.transform.position, 0.7f);
		foreach( Segment seg in segmentsWithoutMesh  ) Gizmos.DrawSphere(seg.transform.position, 0.5f);
	}
#endif
	public void UpdateMeshes() 
	{	// Iterriere durch alles Childs und update die Meshes
		Segment[] allSegments = GetComponentsInChildren<Segment>();
		Segment[] segmentsWithMesh = allSegments.Where( s => s.HasValidNextPoint ).ToArray();
		Segment[] segmentsWithoutMesh = allSegments.Where( s => s.HasValidNextPoint == false ).ToArray();
		// Rechnen die komplette Länge der Strecke um eine normalisierte Koordinate zu kriegen für wie weit du am Track bist
		// 0 = Start im Track | 0.5 = Mitten im Track | 1.0  ist das Ende vom Track
		float[] lengths = segmentsWithMesh.Select( x => x.GetBezierRepresentation( Space.Self ).GetArcLength() ).ToArray();
		float totalRoadLength = lengths.Sum();
		float startDist = 0f;
		for( int i = 0; i < segmentsWithMesh.Length; i++ ) 
		{
			float endDist = startDist + lengths[i];
			Vector2 uvzStartEnd = new Vector2(
				startDist / totalRoadLength,		// Prozent am track Start
				endDist / totalRoadLength			// Prozent am  track Ende
			);
			segmentsWithMesh[i].UpdateMesh( uvzStartEnd );
			startDist = endDist;
		}
		// Clear alle Segmente ohne meshes
		foreach( Segment seg in segmentsWithoutMesh ) seg.UpdateMesh( Vector2.zero );
		
	}
	
}