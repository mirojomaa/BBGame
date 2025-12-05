using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
public class Highlightmanager : MonoBehaviour
{ 
    [BoxGroup("Delay Settings")]  [SerializeField][Range(0, 1)] float SetGlowEnableDelayHex =   0.05f ;
    [BoxGroup("Delay Settings")]  [SerializeField] [Range(0,10)]float SetGlowDisableDelayHex = 2.5f ;
    [BoxGroup("Delay Settings")]  [SerializeField] [Range(0, 1)] float SetGlowEnableDelayObjects =  0.1f ;
    [BoxGroup("Delay Settings")]  [SerializeField] [Range(1, 10)] float SetGlowDisableDelayObjects = 4f;
    public static float GlowEnableDelayObjects, GlowDisableDelayObjects, GlowEnableDelayHex, GlowDisableDelayHex;
    public static Material[] glowMaterialsStatic;
    private static readonly HashSet<Material> hasAllTheUniqueMaterialsHashSet = new HashSet<Material>();
    private static Material[] hasAllTheUniqueMaterials;
    private static  readonly List<GameObject> HexObjects = new List<GameObject>();
    private static  readonly List<GameObject> HexObjectsWithRenderer = new List<GameObject>();
    private static RendererMatIndex[] rendMatIndexStatic;
    public static Material[] HexMaterialsUseStatic;
    [InfoBox("Hier alle Glowmaterials eintragen die man haben möchte für das Level. Einfach in die Glow Material List eintragen. Rend Mat Index und Materials Used bitte nur durch den Button updaten, und das ab und zu.", EInfoBoxType.Warning)]
    [BoxGroup("Glow Materials")]  [SerializeField] public  Material[] GlowMaterialList;
    [BoxGroup("Serialized Lists")] [SerializeField] public RendererMatIndex[] RendMatIndex;
    [BoxGroup("Serialized Lists")] [SerializeField] public Material[] MaterialsUsed;
    [BoxGroup("Serialized Lists")] [SerializeField] public Material[] HexMaterialsUsed;
    [SerializeField] int propIDEmissiveCutoffRef;
    [SerializeField] private int propIDEmissiveIntensityRef;
    public static int propIDEmissiveCutoff;
    public static int propIDEmissiveIntensity;
    private void Awake()
    {
        Array.Resize(ref glowMaterialsStatic,GlowMaterialList.Length);
        glowMaterialsStatic = GlowMaterialList;
        updateStaticValues();
    }
    public static void GlowHighlight(ushort matSwapIndex, byte HighlightType)
    {
        for (int i = 0; i < rendMatIndexStatic[matSwapIndex].rendererList.Length; i++)
            rendMatIndexStatic[matSwapIndex].rendererList[i].material = glowMaterialsStatic[HighlightType];
    }
    public static void DisableGlowHighlight(ushort matSwapIndex)
    {
        
        for (int i = 0; i < rendMatIndexStatic[matSwapIndex].rendererList.Length; i++)
            rendMatIndexStatic[matSwapIndex].rendererList[i].material = hasAllTheUniqueMaterials[rendMatIndexStatic[matSwapIndex].materialIndexNumber[i]];
    }
    
    void updateStaticValues()
    {
        HexMaterialsUseStatic = HexMaterialsUsed;
        HexMaterialsUsed = null;
        hasAllTheUniqueMaterials = MaterialsUsed;
        rendMatIndexStatic = RendMatIndex;
        MaterialsUsed = null; RendMatIndex = null; 
        GlowEnableDelayObjects = SetGlowEnableDelayObjects;
        GlowDisableDelayObjects = SetGlowDisableDelayObjects;
        GlowEnableDelayHex = SetGlowEnableDelayHex;
        GlowDisableDelayHex = SetGlowDisableDelayHex;
        propIDEmissiveIntensity = propIDEmissiveIntensityRef;
        propIDEmissiveCutoff = propIDEmissiveCutoffRef;
    }
#if UNITY_EDITOR
    [Button]
    public void UpdateHasOwnGlow()
    {
        propIDEmissiveCutoffRef = Shader.PropertyToID("_emissiveCutoff");
        propIDEmissiveIntensityRef = Shader.PropertyToID("_emissionIntensity");
        
      
        HexAutoTiling hexAutoTiling = FindObjectOfType<HexAutoTiling>();
        foreach (Transform obj in hexAutoTiling.hasAllTheHexGameObjectsTransformsBeforeStart)
        {
            foreach (HighlightObjectsOwnGlow hex in obj.GetComponentsInChildren<HighlightObjectsOwnGlow >())
            {
                 SerializedObject serHex = new SerializedObject(hex);
                  serHex.FindProperty("cashedEmissiveIntensity").floatValue = hex.gameObject.GetComponent<Renderer>().sharedMaterial.GetFloat(propIDEmissiveIntensityRef);
                serHex.FindProperty("cashedEmissiveCutoff").floatValue = hex.gameObject.GetComponent<Renderer>().sharedMaterial.GetFloat(propIDEmissiveCutoffRef);
                serHex.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
    [Button] public void UpdateAllMaterialIndexies()
    {
        if (Application.isPlaying) return;
        clearAllContainer();
        populateHexObjectLists();
        setHexMaterials();
        pupulateUniqueMaterials();
        setArraySizes();
        populateSerializedPropertys();
    }

    [Button()] public void UpdateHexParentsInHighlightObj()
    {
        HexAutoTiling hexAutoTiling = FindObjectOfType<HexAutoTiling>();
        foreach (Transform obj in hexAutoTiling.hasAllTheHexGameObjectsTransformsBeforeStart)
        {
            foreach (HighlightObjects hex in obj.GetComponentsInChildren<HighlightObjects>())
            {
                SerializedObject serHex = new SerializedObject(hex);
                serHex.FindProperty("hex").objectReferenceValue = obj.gameObject.GetComponent<Hex>();
                serHex.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
    private void setHexMaterials()
    {       HashSet<Material> hasAllTheUniqueHexMaterialsHashSet = new HashSet<Material>();
        foreach (GameObject hex in HexObjects)
        {
            if(hex.GetComponent<Renderer>().sharedMaterial !=null) hasAllTheUniqueHexMaterialsHashSet.Add(hex.GetComponent<Renderer>().sharedMaterial);
        }

        HexMaterialsUsed = hasAllTheUniqueHexMaterialsHashSet.ToArray();
        hasAllTheUniqueHexMaterialsHashSet = null;
    }
    private void clearAllContainer()
    {
        hasAllTheUniqueMaterialsHashSet.Clear();
        HexObjects.Clear();
        HexObjectsWithRenderer.Clear();
       if(RendMatIndex != null) Array.Clear(RendMatIndex,0,RendMatIndex.Length);
    }
    private void populateHexObjectLists()
    {
        foreach (GameObject hex in GameObject.FindGameObjectsWithTag("Hex")) HexObjects.Add(hex);
        foreach (GameObject hex in HexObjects)
        {   
            int rendererCount = hex.transform.GetComponentsInChildren<Renderer>().Length;
            int isValidCount = hex.transform.GetComponentsInChildren<HighlightObjects>().Length;
            if (rendererCount > 0 && isValidCount > 0) HexObjectsWithRenderer.Add(hex);
        }
    }
    private void pupulateUniqueMaterials()
    {
        foreach (GameObject hex in HexObjects)
        {
            for (int i = 0; i < hex.transform.childCount; i++)
                if (hex.transform.GetChild(i).GetComponent<HighlightObjects>() != null)
                {
                    foreach (Renderer rend in hex.transform.GetComponentsInChildren<Renderer>())
                    {
                        if (rend.sharedMaterial != null )
                        {
                            hasAllTheUniqueMaterialsHashSet.Add(rend.sharedMaterial);
                        }
                    }
                }
        }


        hasAllTheUniqueMaterials = hasAllTheUniqueMaterialsHashSet.ToArray();
        
        // for (var index = 0; index < HexMaterialsUsed.Length; index++)
        // {
        //     Material material = HexMaterialsUsed[index];
        //     foreach (Material mat in hasAllTheUniqueMaterials)
        //     {
        //         if (material == mat) HexMaterialsUsed[index] = null;
        //     }
        // }
        MaterialsUsed = hasAllTheUniqueMaterials;
    }
    private void setArraySizes()
    {
        Array.Resize(ref RendMatIndex, HexObjectsWithRenderer.Count+1);
        int rendMatIndexNumber = 1;
        foreach (GameObject hex in HexObjectsWithRenderer)
        {
            int arrayLength = hex.transform.GetComponentsInChildren<Renderer>().Length;
            foreach (Renderer renderer in hex.transform.GetComponentsInChildren<Renderer>())
            {
                if (renderer.sharedMaterial == null) arrayLength--;
            }
            Array.Resize(ref RendMatIndex[rendMatIndexNumber].materialIndexNumber, arrayLength);
            Array.Resize(ref RendMatIndex[rendMatIndexNumber].rendererList, arrayLength);
            rendMatIndexNumber++;
        }
    }
    private void populateSerializedPropertys()
    {
        SerializedObject SerializedHexObject;
        SerializedObject SerializedObject = new SerializedObject(this );
        SerializedProperty rendMatIndexProp = SerializedObject.FindProperty("RendMatIndex");
        int rendererIndexNumber = 1;
        foreach (GameObject hex in HexObjectsWithRenderer)
        {
            foreach (HighlightObjects hiOBJ in hex.transform.GetComponentsInChildren<HighlightObjects>())
            {
                SerializedHexObject = new SerializedObject(hiOBJ);
                SerializedHexObject.FindProperty("matSwapIndex").intValue = rendererIndexNumber;
                SerializedHexObject.ApplyModifiedPropertiesWithoutUndo();
            }
            int countIndexInnerArrays = 0;
            for (ushort i = 0; i < hasAllTheUniqueMaterials.Length; i++)
                foreach (Renderer rend in hex.transform.GetComponentsInChildren<Renderer>())
                {
                    if (rend.gameObject.GetComponent<HighlightObjectsOwnGlow>() != null) continue;
                    if (hasAllTheUniqueMaterials[i] == rend.sharedMaterial)
                    {
                        rendMatIndexProp.GetArrayElementAtIndex(rendererIndexNumber)
                            .FindPropertyRelative("materialIndexNumber").GetArrayElementAtIndex(countIndexInnerArrays).intValue = i;
                         rendMatIndexProp .GetArrayElementAtIndex(rendererIndexNumber)
                           .FindPropertyRelative("rendererList").GetArrayElementAtIndex(countIndexInnerArrays).objectReferenceValue = rend;
                        countIndexInnerArrays++;
                    }
                }
            rendererIndexNumber++;
        }
        SerializedObject.ApplyModifiedPropertiesWithoutUndo();


        SerializedObject serializedHexScript;
        foreach (GameObject hex in HexObjects)
        {
            Renderer hexrender = hex.GetComponent<Renderer>();
            int checkMat = 0;
            for (int i = 0; i < HexMaterialsUsed.Length; i++)
            {
                if (hexrender.sharedMaterial == HexMaterialsUsed[i])
                    checkMat = i;
            }
            serializedHexScript = new SerializedObject(hex.GetComponent<Hex>());
            serializedHexScript.FindProperty("hexSwapIndex").intValue = checkMat;
            serializedHexScript.ApplyModifiedPropertiesWithoutUndo();
        }
    }
#endif
}