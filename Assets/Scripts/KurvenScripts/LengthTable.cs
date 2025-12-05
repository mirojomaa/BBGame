using UnityEngine;
public class LengthTable 
{// Dise Klasse fixed die Uvs. Das ist der UVFixed Mode
	private float[] distances; 
	int SmpCount => distances.Length;
	float TotalLength => distances[SmpCount - 1];
	public LengthTable(OrientedCubicBezier3D bezier, int precision = 16) 
	{
		// Längentabelle generieren
		distances = new float[precision];
		Vector3 prevPoint = bezier.points[0];
		distances[0] = 0f;
		for( int i = 1; i < precision; i++ ) 
		{
			float t = i / (precision - 1f);
			Vector3 currentPoint = bezier.GetPoint( t );
			float delta = (prevPoint-currentPoint).magnitude;
			distances[i] = distances[i - 1] + delta;
			prevPoint = currentPoint;
		}
	}
	public float ToPercentage( float t ) 
	{ 	// Umrechnung des t-Werts in Prozent des Abstands entlang der Kurve
		float iFloat = t * (SmpCount-1);
		int idLower = Mathf.FloorToInt(iFloat);
		int idUpper = Mathf.FloorToInt(iFloat + 1);
		if( idUpper >= SmpCount ) idUpper = SmpCount - 1;
		if( idLower < 0 ) idLower = 0;
		return Mathf.Lerp( distances[idLower], distances[idUpper], iFloat - idLower ) / TotalLength;
	}
}