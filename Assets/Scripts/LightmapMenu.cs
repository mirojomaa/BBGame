#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;
public class LightmapMenu : MonoBehaviour
{
#if UNITY_EDITOR
    private static float boostEmissive = 35;
    [MenuItem ("HaMiLeJa/Bake")]
    static void Bake ()
    {
        List<GameObject> hasAllTheEmmisiveObjects = new List<GameObject>();
        Dictionary<Material, float> hasAllTheOrginalEmmissiveIntensity = new Dictionary<Material, float>();
        List<Material> hasAlltheModifiedGameObjects= new List<Material>();
        String emissiveProperty = "_emssiveIntesnity";
        hasAllTheEmmisiveObjects.Clear();
        hasAllTheOrginalEmmissiveIntensity.Clear();
        hasAllTheEmmisiveObjects.AddRange(GameObject.FindGameObjectsWithTag("Emissve_baked"));
        
        foreach (GameObject tmpObj in hasAllTheEmmisiveObjects)
        {
            Material tmpMaterial = tmpObj.GetComponent<Renderer> ().sharedMaterial;
            tmpMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            float originalEmissiveIntensity = 0;
            try
            {
                if(!hasAllTheOrginalEmmissiveIntensity.ContainsKey(tmpMaterial))
                {   originalEmissiveIntensity = tmpMaterial.GetFloat(emissiveProperty);
                    hasAllTheOrginalEmmissiveIntensity.Add(tmpMaterial, originalEmissiveIntensity);
                    hasAlltheModifiedGameObjects.Add(tmpMaterial);
                    tmpMaterial.SetFloat(emissiveProperty, originalEmissiveIntensity+boostEmissive);
                }
            } 
            catch (NullReferenceException) { }
        }
        Lightmapping.Bake ();
        foreach (Material tmpMaterial in hasAlltheModifiedGameObjects)
        {
            float value = 0;
            if (hasAllTheOrginalEmmissiveIntensity.TryGetValue(tmpMaterial , out value))
                tmpMaterial.SetFloat(emissiveProperty, value);
        }     
    }
#endif
}

