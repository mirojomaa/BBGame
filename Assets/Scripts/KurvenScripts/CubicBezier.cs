using UnityEngine;
// Diese Klasse enthält die Bezier-Auswertungsmathematik
[System.Serializable]
public class OrientedCubicBezier3D 
{
	public Vector3[] points = new Vector3[4]; // Kontrollpunkte der Kurve
	// Aufwärtsvektor für den Anfangs- bzw. Endpunkt.
	// Dies ist für eine korrekte und konsistente Ausrichtung in der Kurve erforderlich.
	public Vector3 upStart, upEnd;
	public OrientedCubicBezier3D( Vector3 upStart, Vector3 upEnd, params Vector3[] points ) 
	{   // Konstruktor
		this.points = points;
		this.upStart = upStart;
		this.upEnd = upEnd;
	}
	// Ermittelt den OrientedPoint anhand eines t-Werts, optional mit einer festgelegten Orientierungserleichterungskurve
	public OrientedPoint GetOrientedPoint( float t, Ease orientationEase = Ease.Linear )
	=>new OrientedPoint( GetPoint(t), GetOrientation( orientationEase.GetEased( t ) ) );
	Quaternion GetOrientation( float t ) 
	{   // Gibt die Orientierung bei einem bestimmten t-Wert entlang der Kurve zurück
		Vector3 forward = GetTangent( t );
		Vector3 up = Vector3.Slerp( upStart, upEnd, t ).normalized;
		return Quaternion.LookRotation( forward, up );
	}
	public float GetArcLength( int precision = 16 ) 
	{   // unterteiltdie Kurve stattdessen in lineare Segmente,
		// und addieren Sie die Länge der einzelnen Segmente
		Vector3[] points = new Vector3[precision];
		for( int i = 0; i < precision; i++ ) 
		{
			float t = i / (precision-1);
			points[i] = GetPoint( t );
		}
		float dist = 0;
		for( int i = 0; i < precision - 1; i++ ) 
		{
			Vector3 a = points[i];
			Vector3 b = points[i+1];
			dist += Vector3.Distance( a, b );
		}
		return dist;
	}
	public Vector3 GetPoint( float t ) 
	{//  Schnelle Auswertung des Punktes bei einem beliebigen t-Wert
		float omt = 1 - t;
		float omt2 = omt * omt;
		float t2 = t * t;
		return
			points[0] * (omt2 * omt) +
			points[1] * (3 * omt2 * t) +
			points[2] * (3 * omt * t2) +
			points[3] * (t2 * t);
	}
	public Vector3 GetTangent( float t ) 
	{//Tangent evaluaten anstelle der Position, was viel schneller ist als die Lerp-Methode.
		float omt = 1f-t;
		float omt2 = omt * omt;
		float t2 = t * t;
		Vector3 tangent =
			points[0] * ( -omt2 ) +
			points[1] * ( 3 * omt2 - 2 * omt ) +
			points[2] * ( -3 * t2 + 2 * t ) +
			points[3] * ( t2 );
		return tangent.normalized;
	}
}