using System.Collections.Generic;
using UnityEngine;
public class MeshExtruder
{  //In dieser Klasse werden die meshes (2d) extrudiert (3d).
	List<Vector3> verts = new List<Vector3>();
	List<Vector3> normals = new List<Vector3>();
	List<Vector2> uvs0 = new List<Vector2>();
	List<Vector2> uvs1 = new List<Vector2>();
	List<Vector3> waypoints = new List<Vector3>();
	List<int> triIndices = new List<int>();
	public void Extrude( Mesh mesh, Mesh2D mesh2D, OrientedCubicBezier3D bezier, Ease rotationEasing, UVMode uvMode,
		Vector2 nrmCoordStartEnd, float edgeLoopsPerMeter, float tilingAspectRatio ) 
	{  // Clear was vorher war um sauber zu starten
		mesh.Clear();
		verts.Clear();
		normals.Clear();
		uvs0.Clear();
		uvs1.Clear();
		triIndices.Clear();
		waypoints.Clear();
		LengthTable table = null; 	// UVs/Texture 
		if(uvMode == UVMode.TiledWithFix) table = new LengthTable( bezier, 12 );
		float curveArcLength = bezier.GetArcLength(), tiling = tilingAspectRatio;	// Tiling von den uvs
		if( uvMode == UVMode.Tiled || uvMode == UVMode.TiledWithFix ) 
		{
			float uSpan = mesh2D.CalcUspan(); // World space units werden zu UV Coordinaten umgerechnet
			tiling *= curveArcLength / uSpan;
			tiling = Mathf.Max( 1, Mathf.Round( tiling ) ); // Snap zum nächsten int um richtig zu tilen
		}
		// Generieren von vertices	// Foreach edge loop // Berechnen der edge loops  // Müssen mindestens 2
		int targetCount = Mathf.RoundToInt( curveArcLength * edgeLoopsPerMeter ), edgeLoopCount = Mathf.Max( 2, targetCount );
		for( int ring = 0; ring < edgeLoopCount; ring++ ) 
		{
			float t = ring / (edgeLoopCount-1f);
			OrientedPoint op = bezier.GetOrientedPoint( t, rotationEasing );
			float tUv = t; // Bereite UV coordinaten vor
			if( uvMode == UVMode.TiledWithFix ) tUv = table.ToPercentage( tUv );
			float uv0V = tUv * tiling,
			 uv1U = Mathf.Lerp( nrmCoordStartEnd.x, nrmCoordStartEnd.y, tUv ); // Normalizierte Coordinaten

			for( int i = 0; i < mesh2D.VertexCount; i++ ) 
			{ 	// Foreach vertex in der  2D shape
				verts.Add( op.LocalToWorldPos( mesh2D.vertices[i].point ) );
				normals.Add( op.LocalToWorldVec( mesh2D.vertices[i].normal ) );
				uvs0.Add( new Vector2( mesh2D.vertices[i].u, uv0V ) );
				uvs1.Add( new Vector2( uv1U, 0 ) );
			}
		}
		for( int edgeLoop = 0; edgeLoop < edgeLoopCount - 1; edgeLoop++ )
		{ 	// Hier werden die Triangles generiert. Ich habe den approach genommen, dass es über kreuz geht
			// Foreach edge loop (bis auf den letzen, da es ein step weiter schaut)
			int rootIndex = edgeLoop * mesh2D.VertexCount;
			int rootIndexNext = (edgeLoop+1) * mesh2D.VertexCount;
			// Foreach  linepaare verts im  2D shape
			for( int line = 0; line < mesh2D.LineCount; line += 2 )
			{
				int lineIndexA = mesh2D.lineIndices[line], lineIndexB = mesh2D.lineIndices[line+1],
					currentA = rootIndex + lineIndexA, currentB = rootIndex + lineIndexB,
				    nextA = rootIndexNext + lineIndexA, nextB = rootIndexNext + lineIndexB;
				triIndices.Add( currentA );
				triIndices.Add( nextA );
				triIndices.Add( nextB );
				triIndices.Add( currentA );
				triIndices.Add( nextB );
				triIndices.Add( currentB );
			}
		}
		// Simples assignen von allem
		mesh.SetVertices( verts );
		mesh.SetNormals( normals );
		mesh.SetUVs( 0, uvs0 );
		mesh.SetUVs( 1, uvs1 );
		mesh.SetTriangles( triIndices, 0 );
	}
}