
// das sind einfach kleine Mathematische berechnungen die ich nicht stöndig in allen klassen haben möchte
// Tau ist z.B. pi*2,    inverslerp ändert die t value des lerps

using System.Runtime.CompilerServices;
using UnityEngine;
public static class MathLibary 
{
	// Gebe a und b rein und du kriegst die Prozente eines lerps // (t value)// v ist in dieser range
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float InverseLerp( float a, float b, float v ) => (v - a) / (b - a);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Remap( float iMin, float iMax, float oMin, float oMax, float v ) 
	{ // Remaps eine komplette Value range zu einer anderen. Wie beim shader. Inverslep und lerp
		float t = InverseLerp(iMin, iMax, v);
		return Mathf.LerpUnclamped( oMin, oMax, t );
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float RemapClamped( float iMin, float iMax, float oMin, float oMax, float v ) 
	{ // positive only
		float t = InverseLerp(iMin, iMax, v);
		return Mathf.Lerp( oMin, oMax, t );
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CalculateDistance(GameObject Player, GameObject OtherObject)
	{
		Vector3 Verbindungsvector = Player.transform.position - OtherObject.transform.position;
		return Mathf.Sqrt(Mathf.Pow(Verbindungsvector.x, 2) + Mathf.Pow(Verbindungsvector.y, 2) + Mathf.Pow(Verbindungsvector.z, 2));
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CalculateDistance(Transform Player, Transform OtherObject)
	{
		Vector3 Verbindungsvector = Player.transform.position - OtherObject.transform.position;
		return Mathf.Sqrt(Mathf.Pow(Verbindungsvector.x, 2) + Mathf.Pow(Verbindungsvector.y, 2) + Mathf.Pow(Verbindungsvector.z, 2));
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CalculateDistanceSquared(GameObject Player, GameObject OtherObject)
	{
		Vector3 Verbindungsvector = Player.transform.position - OtherObject.transform.position;
		return Mathf.Pow(Verbindungsvector.x, 2) + Mathf.Pow(Verbindungsvector.y, 2) + Mathf.Pow(Verbindungsvector.z, 2);
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CalculateDistance(Vector3 Player, Vector3  OtherObject)
	{
		Vector3 Verbindungsvector = Player - OtherObject;
		return Mathf.Sqrt(Mathf.Pow(Verbindungsvector.x, 2) + Mathf.Pow(Verbindungsvector.y, 2) + Mathf.Pow(Verbindungsvector.z, 2));
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CalculateDistanceSquared(Vector3 Player, Vector3  OtherObject)
	{
		Vector3 Verbindungsvector = Player - OtherObject;
	
		return Mathf.Pow(Verbindungsvector.x, 2) + Mathf.Pow(Verbindungsvector.y, 2) + Mathf.Pow(Verbindungsvector.z, 2);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float sqrMagnitudeInlined(Vector3 v) => v.x * v.x + v.y * v.y + v.z * v.z;

	public static void boostDirection(Vector3 pointA, Vector3 pointB, float forceAmount, Rigidbody rb)
	{
		var aToB = pointB - pointA;
		var aToBDir = aToB.normalized;
		aToBDir = new Vector3(aToBDir.x, 0, aToBDir.z);
		rb.AddForce(aToBDir * forceAmount, ForceMode.Impulse);
	}

	public static string TrimToFirstDigitA(float number)//nur bis 9999
	{
		string firstDigit = number.ToString().Remove(1, number.ToString().Length - 1);

		return firstDigit;
	}

	/*
	public static string RemoveDigits(float number, float removeAmount) 
	{
		string digits = "";

		string progress = number.ToString();



		int charAmount = progress.ToCharArray().Length;

		for (int i = 1; i < charAmount-removeAmount+1; i++)
		{
			digits = progress.Remove(progress.Length - 1, 1);
			Debug.Log("Removeing " + i);
		}


		return digits;
	}*/
}
