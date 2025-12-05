using System.Collections;
using UnityEngine;
public class CameraShake : MonoBehaviour 
{ 
	[System.Serializable] public class Einstellungen 
	{
		public float angle; public float strength; public float maxSpeed;
		public float minSpeed; public float duration;
		[Range(0,1)] public float noise;
		[Range(0,1)] public float damping;
		[Range(0,1)] public float rotation;
		public Einstellungen (float angle, float strength, float speed, float duration, float noise, float damping, float rotation)
		{
			this.angle = angle; this.strength = strength; maxSpeed = speed; this.duration = duration; this.noise = Mathf.Clamp01(noise);
			this.damping = Mathf.Clamp01(damping); this.rotation = Mathf.Clamp01(rotation);
		}
	}
	const float maxAngle = 10f;
	float CalcDamping(float x, float damping) 
	{
		x = Mathf.Clamp01 (x);
		float a = Mathf.Lerp (2, .25f, damping);
		float b = 1 - Mathf.Pow (x, a);
		return b * b * b;
	}
	IEnumerator currentShakeCoroutine;
	public void StartShake(Einstellungen einstellungen) 
	{
		if (currentShakeCoroutine != null)
			StopCoroutine (currentShakeCoroutine);
		currentShakeCoroutine = Shake (einstellungen);
		StartCoroutine (currentShakeCoroutine);
	}
	IEnumerator Shake(Einstellungen einstellungen) 
	{
		float last = 0; float go = 0;
		float angleR = einstellungen.angle * Mathf.Deg2Rad - Mathf.PI;
		Vector3 b = Vector3.zero;
		Vector3 a = Vector3.zero;
		float moveDistance = 0; float speed = 0;
		Quaternion targetRotation = Quaternion.identity;
		Quaternion previousRotation = Quaternion.identity;

		do {
			if (go >= 1 || last == 0) 
			{
				float dampingFactor = CalcDamping (last, einstellungen.damping);
				float noiseAngle = (Random.value - .5f) * Mathf.PI;
				angleR += Mathf.PI + noiseAngle * einstellungen.noise;
				a = new Vector3 (Mathf.Cos (angleR), Mathf.Sin (angleR)) * einstellungen.strength * dampingFactor;
				b = transform.localPosition;
				moveDistance = Vector3.Distance (a, b);
				targetRotation = Quaternion.Euler (new Vector3 (a.y, a.x).normalized * einstellungen.rotation * dampingFactor * maxAngle);
				previousRotation = transform.localRotation;
				speed = Mathf.Lerp(einstellungen.minSpeed,einstellungen.maxSpeed,dampingFactor);
				go = 0;
			}
			last += Time.deltaTime / einstellungen.duration;
			go += Time.deltaTime / moveDistance * speed;
			transform.localPosition = Vector3.Lerp (b, a, go);
			transform.localRotation = Quaternion.Slerp (previousRotation, targetRotation, go);
			yield return null;
		} while (moveDistance > 0);
	}
}