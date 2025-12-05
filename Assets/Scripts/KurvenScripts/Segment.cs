using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer) )]
[ExecuteInEditMode]
public class Segment : UniqueMesh 
{
   //Was serialized wird
	public float tangentLength = 3; // Tangenten Größe. Das ist nur die Tangente vom ersten Punkt. Das nächste Segment kontrolliert den Endpunkt von dieser Tangentenlänge
	public Ease rotationEasing = Ease.InOut;
	// Das wird nicht serialized
	MeshExtruder meshExtruder = new MeshExtruder();

	#if UNITY_EDITOR
	private void OnValidate()
	{
		if (Application.isPlaying) return;
		CurveManager.updateCurves = true;
	} 
	private void OnDrawGizmosSelected() => CurveManager.updateCurves = true;
#endif
	// Properties 
	public bool HasValidNextPoint => TryGetNextSegment() != null;
	bool IsInValidChain => transform.parent.Ref()?.GetComponent<ChainParent>() != null;
	public ChainParent ChainParent => transform.parent == null ? null : transform.parent.GetComponent<ChainParent>(); //einfacher bedingungsoperator
	Mesh2D Mesh2D => ChainParent.mesh2D; // wir müssen "nur" das 2d mesh konfigurieren, als scriptable Objekt

	// Das wird das Mesh refreshen
	public void UpdateMesh( Vector2 nrmCoordStartEnd ) 
	{

		// Genereiert das Mesh nur, wenn wir den nächsten Kontrollpunkt haben
		if( HasValidNextPoint ) 
		{
			meshExtruder.Extrude(
				mesh: Mesh,
				mesh2D: ChainParent.mesh2D,
				bezier: GetBezierRepresentation( Space.Self ),
				rotationEasing: rotationEasing,
				uvMode: ChainParent.uvMode,
				nrmCoordStartEnd: nrmCoordStartEnd,
				edgeLoopsPerMeter: ChainParent.MeshDensity,
				tilingAspectRatio: GetTextureAspectRatio()
			);
		} 
		else if( meshCached != null ) DestroyImmediate( meshCached );
	}
	float GetTextureAspectRatio() 
	{
		Texture texture = GetComponent<MeshRenderer>().sharedMaterial.Ref()?.mainTexture;
		return texture != null ? texture.AspectRatio() : 1f;
	}
	// Erhält einen von den 4 Bezier Kontrollpunkten
	// Das verhindern Doppelttransformation von Spaces
	public Vector3 GetControlPoint( int i, Space space ) 
	{
		Vector3 FromLocal( Vector3 localPos ) => space == Space.Self ? localPos : transform.TransformPoint( localPos );
		Vector3 FromWorld( Vector3 worldPos ) => space == Space.World ? worldPos : transform.InverseTransformPoint( worldPos );
		if( i < 2 ) 
		{
			if( i == 0 ) return FromLocal( Vector3.zero );
			if( i == 1 ) return FromLocal( Vector3.forward * tangentLength );
		} 
		
		else 
		{
			Segment next = TryGetNextSegment();
			Transform nextTf = next.transform;
			if( i == 2 ) return FromWorld( nextTf.TransformPoint( Vector3.back * next.tangentLength ) );
			if( i == 3 ) return FromWorld( nextTf.position );
		}
		return default;
	}
	 
	// Gibt dir das nächste Segment, falls es existiert
	// Und falls looping an ist, macht es noch ein Segmnet um die Strecke zu schließen
	Segment TryGetNextSegment() 
	{
		if( !IsInValidChain ) return null;
		int thisIndex = transform.GetSiblingIndex();
		bool isLast = thisIndex == transform.parent.childCount-1;
		Segment GetSiblingSegment( int i ) => transform.parent.GetChild( i ).GetComponent<Segment>();
		if( isLast && ChainParent.closed ) return GetSiblingSegment( 0 ); // First segment
		if( !isLast ) return GetSiblingSegment( thisIndex + 1 ); // Next segment
		return null;
	}

	// Gibt die Bezier Orientation zurück vom segment
	// Wir brauchen es in beidem, World- und Localspace. Meshes sind in LocalSpace, während die gizmos in world space sind.
	// Das wird vom Segment Inspector genutzt

	public OrientedCubicBezier3D GetBezierRepresentation( Space space ) 
	{
		return new OrientedCubicBezier3D(
			GetUpVector( 0, space ),
			GetUpVector( 3, space ),
			GetControlPoint( 0, space ),
			GetControlPoint( 1, space ),
			GetControlPoint( 2, space ),
			GetControlPoint( 3, space )
		);
	}
	// Gibt den Vector up vom ersten oder letzten Kontrollpunkt im jetzigen space
	Vector3 GetUpVector( int i, Space space ) 
	{
		if( i == 0 ) return space == Space.Self ? Vector3.up : transform.up;
		if( i == 3 ) 
		{
			Vector3 wUp = TryGetNextSegment().transform.up;
			return space == Space.World ? wUp : transform.InverseTransformVector( wUp );
		}
		return default;
	}
}
