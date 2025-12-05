///old Scritp nicht löschen!!!
///
///
///
/// Ich brauche davon noch teile später
/*
using System.Collections.Generic;
using UnityEngine;
public class GlowHighlight : MonoBehaviour
{
    #region Dictonarys
    Dictionary<Renderer, Material[]> originalMaterialDictionaryHexes = new Dictionary<Renderer, Material[]>();
    Dictionary<Renderer, Material[]> originalMaterialDictionaryProps = new Dictionary<Renderer, Material[]>();
    Dictionary<Color, Material> cachedGlowMaterials = new Dictionary<Color, Material>();
    #endregion
    
    #region Inspector
     Material glowMaterial;
    private bool isGlowing;
    private Color validSpaceColor = Color.green;
    private Color originalGlowColor;
    #endregion
    
    #region MaterialSwap
    private void Start() =>PrepareMaterialDictionaries();
    
    private void PrepareMaterialDictionaries()
    {
        if (transform.childCount > 0)
        {
            foreach (Renderer renderer in transform.GetChild(0).GetComponentsInChildren<Renderer>())
          {
              Material[] originalMaterials = renderer.materials;
                originalMaterialDictionaryHexes.Add(renderer, originalMaterials);
          }
         }
        foreach (Renderer renderer in transform.GetChild(1).GetComponentsInChildren<Renderer>())
            {
                Material[] originalMaterials = renderer.materials;
                originalMaterialDictionaryProps.Add(renderer, originalMaterials);
            }
    }
    #endregion

    #region PathHighlight
    
    /*   WIRD SPÄTER WIEDER BENUTZT
    internal void HighlightValidPath(bool isProp)
    {
        if (isGlowing == false) return;
        if (!isProp)
        {
            foreach (Renderer renderer in glowMaterialDictionaryHexes.Keys)
            {
                foreach (Material item in glowMaterialDictionaryHexes[renderer])
                {
                    if (item == null) continue;
                    item.SetColor("_GlowColor", validSpaceColor);
                }
            }
        }
        if (isProp)
        {
            foreach (Renderer renderer in glowMaterialDictionaryProps.Keys)
            {
                foreach (Material item in glowMaterialDictionaryProps[renderer])
                { 
                    if (item == null) continue;
                    item.SetColor("_GlowColor", validSpaceColor);
                }
            }
        }
    }#1#
    /*internal void ResetGlowHighlight(bool isProp)
    {
        // if (!isProp)
        // {
        //     foreach (Renderer renderer in glowMaterialDictionaryHexes.Keys)
        //     {
        //         foreach (Material item in glowMaterialDictionaryHexes[renderer])
        //         {
        //               if (item == null) continue;
        //             item.SetColor("_GlowColor", originalGlowColor);
        //         }
        //     }
        //  }
        // if (isProp)
        // {
        //     foreach (Renderer renderer in glowMaterialDictionaryProps.Keys)
        // {
        //     foreach (Material item in glowMaterialDictionaryProps[renderer])
        //     {
        //         if (item == null) continue;
        //         item.SetColor("_GlowColor", originalGlowColor);
        //     }
        // }
        // }
    }#1#
    public void ToggleGlow(bool isProp, int HighlightType)
    {
        if (!isProp)
        {
            if (isGlowing == false)
            {
            //    ResetGlowHighlight(isProp);
                foreach (Renderer renderer in originalMaterialDictionaryHexes.Keys)
                {
                    if (renderer == null) continue;
                    for (int i = 0; i < renderer.materials.Length; i++)
                        renderer.material = Highlightmanager.GlowMaterials[HighlightType];
                }
            }
        else
        {
            foreach (Renderer renderer in originalMaterialDictionaryHexes.Keys)
            {
                if (renderer == null) continue;
                renderer.materials = originalMaterialDictionaryHexes[renderer];
            }
        }
        isGlowing = !isGlowing;
        }
        if (isProp)
        {
            if (isGlowing == false)
            {
            //    ResetGlowHighlight(isProp);
                foreach (Renderer renderer in originalMaterialDictionaryProps.Keys)
                {
                    if (renderer == null) continue;
                    for (int i = 0; i < renderer.materials.Length; i++)
                        renderer.material = Highlightmanager.GlowMaterials[HighlightType];
                }
            }
            else
            {
                foreach (Renderer renderer in originalMaterialDictionaryProps.Keys)
                {
                    if (renderer == null) continue;
                    renderer.materials = originalMaterialDictionaryProps[renderer];
                }
            }
            isGlowing = !isGlowing;
        }
    }
    public void ToggleGlow(bool state, bool isProp, int HighlightType)
    {
        if (isGlowing == state) return;
        isGlowing = !state;
        ToggleGlow(isProp, HighlightType);
    }
    #endregion
}*/