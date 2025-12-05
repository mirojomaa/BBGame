using System;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.PlayerLoop;
using System.Linq;
using Cinemachine;
using UnityEngine.Jobs;
[ExecuteAlways]
public class Pathfinder : MonoBehaviour
{
	protected CatmullRom spline;
	private CatmullRom.CatmullRomPoint[] waypointsForPlayer;
	private BoxCollider[] m_Collider;
	private const float Tvalue = 50f;
	private bool pathfindingAllowed = true, startPathfindingDisable, noCam, noManager;
	[ReorderableList] public Transform[] controlPoints;
	private TransformAccessArray controlPointsNativeTransforms;
	[SerializeField] private CinemachineVirtualCamera cam = default;
	[SerializeField] [Tooltip("Funktioniert nur wenn gamemanager zugewiesen")]private float disableShakeSec = 1.1f;
	[BoxGroup("Einstellungen")] private bool ClosedLoop = false;
	[InfoBox("Resolution steuert am meisten die Geschwindkeit;  Kann nicht ingame verändert werden, dass muss man in der Scene view machen (wegen performance)")]
	[BoxGroup("Einstellungen")] [Range(2, 100)] [SerializeField] private int Resolution = 30;
	[BoxGroup("Einstellungen")] [Range(1, 3)][SerializeField] private int speedLevel = 1;
	[BoxGroup("Einstellungen")] [Range(2, 15)] [SerializeField] private float secPathDisabled = 5;
	[BoxGroup("Einstellungen")] [Range(5, 200)] [SerializeField] private float forceRedExit = 80;
	[BoxGroup("Einstellungen")] [Range(5, 200)] [SerializeField] private float forceGreenExit = 80;

	[BoxGroup("Debug")][SerializeField]private bool drawNormal;
	[BoxGroup("Debug")][SerializeField]private bool drawTangent;
	[BoxGroup("Debug")][Range(0, 20)] public float normalLength;
	[BoxGroup("Debug")][Range(0, 20)] public float tangentLength;
	#if UNITY_EDITOR
	[BoxGroup("Debug")][Range(1, 20)] [SerializeField] private float colliderSize = 3;
#endif
	private const int playerLayerInt = 11;
	private const int playerNoCollisionLayerInt = 12;

	private void Awake()
	{
		if (!Application.isPlaying) return;
			if (controlPoints != null)
		{
			controlPointsNativeTransforms = new TransformAccessArray(controlPoints, 12);
			controlPoints = null;
		}
		if (cam != null)
		{
			noCam = false;
			cam.gameObject.SetActive(false);
		}
		else if (cam == null)
		{
			cam = CameraZoomOut.vcamera;
			noCam = true;
		}
	}
	private void OnDestroy()
	{
		if (!Application.isPlaying) return;
		controlPointsNativeTransforms.Dispose();
	}

	[Button("Fill List with Controlpoints")] public void AllChildsToList()
	{
		Array.Clear(controlPoints, 0, controlPoints.Length);
		controlPoints = GetComponentsInChildren<Transform>();
			controlPoints = controlPoints.Skip(1).ToArray();
	}
#if UNITY_EDITOR
	[Button("Empty everything from list")] public void RemoveFromList()
	{
		Array.Clear(controlPoints, 0, controlPoints.Length);
		Array.Resize(ref controlPoints, 0);
	}
	[Button("SetCollider")]  private void setCollider()
	{
		if (m_Collider != null)
		{
			Array.Clear(m_Collider,0,m_Collider.Length);
			Array.Resize(ref m_Collider, 0);
		}
		m_Collider = GetComponents<BoxCollider>();
		if (transform.childCount < 1) return;
		m_Collider[0].size = new Vector3(colliderSize, colliderSize, colliderSize);
		m_Collider[0].center = gameObject.transform.GetChild(0).transform.localPosition;
		if (transform.childCount < 2) return;
		m_Collider[1].size = new Vector3(colliderSize, colliderSize, colliderSize);
		m_Collider[1].center = gameObject.transform.GetChild(transform.childCount-1).transform.localPosition;
	}
	[Button] private void resetRotationControlPoints()
	{
		int children = transform.childCount;
		for (int i = 0; i < children; ++i) 
			gameObject.transform.GetChild(i).transform.rotation = new Quaternion(0, 0, 0, 0);
	}
	private void OnDrawGizmosSelected() => CurveManager.updatePathfinder = true;
	private void OnDrawGizmos()
	{
		if (Application.isPlaying || !CurveManager.drawPathfindingInEditMode) return;
	
		//---------------- First Time Update ---------------- //
		if (spline == null)
		{
			spline = new CatmullRom(controlPoints, Resolution, ClosedLoop);
			waypointsForPlayer = spline.GenerateSplinePoints();
		}
		//---------------- Draw Pathfinding ---------------- //
		DrawSplinePoints();

		//---------------- Update only when changed ---------------- //
		if ( !CurveManager.updatePathfinder) return;
		CurveManager.updatePathfinder = false;
		foreach (Transform elem in controlPoints)
			if (elem == null) Debug.Log("No Controlpoints");
		if (transform.childCount < 2 && controlPoints.Length < 2)  Debug.Log("Too little Controlpoints");
		if (spline != null && transform.childCount > 2 && controlPoints.Length > 2)
		{
			spline = new CatmullRom(controlPoints, Resolution, ClosedLoop);
			waypointsForPlayer = spline.GenerateSplinePoints();	
			DrawSplinePoints();
		}
		else if (transform.childCount > 2 && controlPoints.Length > 2)
			spline = new CatmullRom(controlPoints, Resolution, ClosedLoop);
	}

	void DrawSplinePoints()
	{
		spline.DrawSpline(Color.white, waypointsForPlayer);
		if (drawNormal) spline.DrawNormals(normalLength, Color.yellow, waypointsForPlayer);
		if (drawTangent) spline.DrawTangents(tangentLength, Color.green, waypointsForPlayer);
	}
#endif
	IEnumerator CamShakeDisableAterPush_Coroutine(float sec)
	{
		GameManager.bridgePause = true;
		yield return new WaitForSeconds(sec);
		GameManager.bridgePause = false;
	}
	IEnumerator movePath(Collider other)
	{
		for (int i = 0 ; i <waypointsForPlayer.Length-1 ; i++)
		{
			if (i == waypointsForPlayer.Length-2)
			{
				if(!noCam)cam.gameObject.SetActive(false);
				ReferenceLibrary.GameMng.AllowMovement = true;
				StopCoroutine(waitUntilNextTrigger());
				StartCoroutine(waitUntilNextTrigger());
				MathLibary.boostDirection(waypointsForPlayer[waypointsForPlayer.Length - 2].position,
					waypointsForPlayer[waypointsForPlayer.Length - 1].position, forceRedExit, other.attachedRigidbody);
				StopCoroutine(CamShakeDisableAterPush_Coroutine(disableShakeSec));
					StartCoroutine(CamShakeDisableAterPush_Coroutine(disableShakeSec));
			}
			if (i < waypointsForPlayer.Length - 1 && i >1)
			{
				other.transform.position = new Vector3(
					Mathf.Lerp(other.transform.position.x, waypointsForPlayer[i].position.x,
						Tvalue*Time.deltaTime),
					Mathf.Lerp(other.transform.position.y, waypointsForPlayer[i].position.y,
						Tvalue*Time.deltaTime),
					Mathf.Lerp(other.transform.position.z, waypointsForPlayer[i].position.z,
						Tvalue*Time.deltaTime));
				if(speedLevel==1) yield return new TimeUpdate.WaitForLastPresentationAndUpdateTime();
				if(speedLevel==2) yield return new WaitForEndOfFrame();
				if (speedLevel==3) yield return new WaitForFixedUpdate();
			}
		}
		ReferenceLibrary.Player.layer = playerLayerInt;
	}
	IEnumerator movePathReverse(Collider other)
	{
		for (int i = waypointsForPlayer.Length; i > -1 ; i--)
		{
			if (i == 2)
			{
				if(!noCam)cam.gameObject.SetActive(false);
				ReferenceLibrary.GameMng.AllowMovement = true;
				StopCoroutine(waitUntilNextTrigger());
				StartCoroutine(waitUntilNextTrigger());
				MathLibary.boostDirection(waypointsForPlayer[2].position,
					waypointsForPlayer[1].position, forceGreenExit, other.attachedRigidbody);
				StopCoroutine(CamShakeDisableAterPush_Coroutine(disableShakeSec));
				StartCoroutine(CamShakeDisableAterPush_Coroutine(disableShakeSec));
			}
			if (i < waypointsForPlayer.Length - 1 && i >2)
			{
				other.transform.position = new Vector3(
					Mathf.Lerp(other.transform.position.x, waypointsForPlayer[i - 1].position.x,
						Tvalue*Time.deltaTime),
					Mathf.Lerp(other.transform.position.y, waypointsForPlayer[i - 1].position.y,
						Tvalue*Time.deltaTime),
					Mathf.Lerp(other.transform.position.z, waypointsForPlayer[i - 1].position.z,
						Tvalue*Time.deltaTime));
				if(speedLevel==1) yield return new TimeUpdate.WaitForLastPresentationAndUpdateTime();
				if(speedLevel==2) yield return new WaitForEndOfFrame();
				if (speedLevel==3) yield return new WaitForFixedUpdate();
			}
		}
		  
			ReferenceLibrary.Player.layer = playerLayerInt;
	}
	IEnumerator waitUntilNextTrigger()
	{
		GameManager.StartMovingGhostLayer = false;
		ReferenceLibrary.PlayerMov.DisableGravity = false;
		GameManager.LerpCameraBack = true;
		yield return new WaitForSeconds(secPathDisabled);
		pathfindingAllowed = true;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			spline = new CatmullRom(controlPointsNativeTransforms, Resolution, ClosedLoop);
			waypointsForPlayer = spline.GenerateSplinePoints();
			float distanceStart = MathLibary.CalculateDistanceSquared(other.transform.position,
				waypointsForPlayer[0].position);
			float distanceEnd = MathLibary.CalculateDistanceSquared(other.transform.position,
				waypointsForPlayer[waypointsForPlayer.Length-1].position);
			if (pathfindingAllowed)
			{
				GameManager.StartMovingGhostLayer = true;
				pathfindingAllowed = false;
				if(!noCam) cam.gameObject.SetActive(true);
				ReferenceLibrary.GameMng.AllowMovement = false;
				ReferenceLibrary.PlayerMov.DisableGravity = true;
				ReferenceLibrary.Player.layer = playerNoCollisionLayerInt;
				if (distanceStart > distanceEnd)
				{
					StopCoroutine(movePathReverse(other)); 
					StartCoroutine(movePathReverse(other));
					return;
				}
				
					StopCoroutine(movePath(other));
					StartCoroutine(movePath(other));
			}
		}
	}
}