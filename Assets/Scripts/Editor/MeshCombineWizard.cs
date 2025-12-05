using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class MeshCombiner : ScriptableWizard
{
	public GameObject combineParent;
	public bool is32bit = true;
	[MenuItem("HaMiLeJa/ MeshCombiner")]
	static void CreateWizard()
	{
		var wizard = DisplayWizard<MeshCombiner>("Mesh Combine Wizard");
		var selectionObjects = Selection.objects;
		if (selectionObjects != null && selectionObjects.Length == 1) 
		{
			var firstSelection = selectionObjects[0] as GameObject;
			if (firstSelection != null) wizard.combineParent = firstSelection;
		}
	}
	void OnWizardCreate()
	{
		if (combineParent == null) 
		{
			Debug.LogError("Assign ein Parent zuerst");
			return;
		}
		Vector3 originalPosition = combineParent.transform.position;
		combineParent.transform.position = Vector3.zero;
		Dictionary<Material, List<MeshFilter>> hasAllThematerialToMeshFilter = new Dictionary<Material, List<MeshFilter>>();
		List<GameObject> combinedObjects = new List<GameObject>();
		MeshFilter[] meshFilters = combineParent.GetComponentsInChildren<MeshFilter>();
		foreach (var meshFilter in meshFilters) 
		{
			var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
			if (meshRenderer == null) 
			{
				Debug.LogWarning("Der Mesh Filter von  " + meshFilter.name + " hat keinen Mesh renderer.");
				continue;
			}
			var materials = meshRenderer.sharedMaterials;
			if (materials == null) 
			{
				Debug.LogWarning("Der Mesh Renderer von " + meshFilter.name + "hat kein Material");
				continue;
			}
			if (materials.Length > 1) 
			{
				combineParent.transform.position = originalPosition;
				Debug.LogError("Objekte mit mehreren Materials pro Mesh werden aktuell nicht supportet. Abgebrochen.");
				return;
			}
			var material = materials[0];
			if (hasAllThematerialToMeshFilter.ContainsKey(material)) hasAllThematerialToMeshFilter[material].Add(meshFilter);
			else hasAllThematerialToMeshFilter.Add(material, new List<MeshFilter>() { meshFilter });
		}
		foreach (var entry in hasAllThematerialToMeshFilter) 
		{
			List<MeshFilter> meshesWithSameMaterial = entry.Value;
			string materialName = entry.Key.ToString().Split(' ')[0];

			CombineInstance[] combine = new CombineInstance[meshesWithSameMaterial.Count];
			for (int i = 0; i < meshesWithSameMaterial.Count; i++) 
			{
				combine[i].mesh = meshesWithSameMaterial[i].sharedMesh;
				combine[i].transform = meshesWithSameMaterial[i].transform.localToWorldMatrix;
			}
			var format = is32bit? IndexFormat.UInt32 : IndexFormat.UInt16;
			Mesh combinedMesh = new Mesh { indexFormat = format };
			combinedMesh.CombineMeshes(combine);
			materialName += "_" + combinedMesh.GetInstanceID();
			AssetDatabase.CreateAsset(combinedMesh, "Assets/CombinedMeshes_" + materialName + ".asset");
			string goName = (hasAllThematerialToMeshFilter.Count > 1)? "CombinedMeshes_" + materialName : "CombinedMeshes_" + combineParent.name;
			GameObject combinedObject = new GameObject(goName);
			var filter = combinedObject.AddComponent<MeshFilter>();
			filter.sharedMesh = combinedMesh;
			var renderer = combinedObject.AddComponent<MeshRenderer>();
			renderer.sharedMaterial = entry.Key;
			combinedObjects.Add(combinedObject);
		}
		GameObject resultGO = null;
		if (combinedObjects.Count > 1) 
		{
			resultGO = new GameObject("CombinedMeshes_" + combineParent.name);
			foreach (var combinedObject in combinedObjects) combinedObject.transform.parent = resultGO.transform;
		}
		else resultGO = combinedObjects[0];
		
#pragma warning disable 618
		Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + resultGO.name + ".prefab");
		PrefabUtility.ReplacePrefab(resultGO, prefab, ReplacePrefabOptions.ConnectToPrefab);
#pragma warning restore 618
		combineParent.SetActive(false);
		combineParent.transform.position = originalPosition;
		resultGO.transform.position = originalPosition;
	}
}