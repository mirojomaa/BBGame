using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;
public class HighlightObjects : MonoBehaviour
{  
    [BoxGroup("|| Shake ||")] [Tooltip("Better to put in here the Lampenarm")] [SerializeField] private GameObject Lampe;
    [BoxGroup("|| Shake ||")] [Tooltip("Number of Shakes the Attached Arm will do. Increase it to let it shake more")] [SerializeField] private int headshakes = 4;
    [BoxGroup("|| GlowHighlight ||")]   [Tooltip("Choose the HighlightMaterial from the HighlightManager. If you choose a too big number it will default to 0)")] 
    [Range(0, 25)] [SerializeField] private Byte highlightType;
    [BoxGroup("|| Debug ||")] [Tooltip("This Property will be modified by the Button in the HighlightManager." +
                                       " It shows where its renderer and Pointer to original Material are at the Index shown here for debug. DO NOT MANUALLY TOUCH IT!!!")]
      public ushort matSwapIndex;
    [BoxGroup("|| Debug ||")] [Tooltip("Rotation bool that resets when the rotation is finished. Only Shown for Debug")][SerializeField] private bool rotationAllowed = true;
    [SerializeField] public Hex hex;
#if UNITY_EDITOR
   [BoxGroup("|| GlowHighlight ||")] [Tooltip("The Name of the Material setup at the Object HighlightManager")]  [SerializeField] private string HighlightName;
   [BoxGroup("|| GlowHighlight ||")]   [Tooltip("How long you want the Highlight to Preview before it switches back?")]  [Range(1, 5)] [SerializeField] private Byte PreviewDuration = 2;
   [BoxGroup("|| Debug ||")] [Tooltip("Only for debug. Do not use it")]  [SerializeField] private List<Material> OldMaterials = new List<Material>();
   private bool previewHighlightinEditor = false;
   private void OnValidate() => changeNamePreview();
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        if (previewHighlightinEditor)
        {
            previewHighlightinEditor = false;
            StartCoroutine(showPreviewWithReset_Coroutine(PreviewDuration));
        }
    }
    IEnumerator showPreviewWithReset_Coroutine(byte previewDur)
    {
        yield return showPreview_Coroutine(previewDur);
        OldMaterials.Clear();
    }
    IEnumerator showPreview_Coroutine(byte previewDur)
    {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>()) OldMaterials.Add(rend.sharedMaterial);
        switchToGlowMaterialPreview();
        yield return new WaitForSeconds(previewDur);
        switchToNormalMaterialPreview(OldMaterials);
        changeNamePreview();
    }
    void changeNamePreview()
    {
        if (Application.isPlaying) return;
        Highlightmanager hmanger = FindObjectOfType<Highlightmanager>();
        if (highlightType >= hmanger.GlowMaterialList?.Length)
            HighlightName = "No Material Setup on this Slot!!!";
        else  if  (highlightType < hmanger.GlowMaterialList.Length)
            HighlightName = hmanger.GlowMaterialList[highlightType].name;
    }
    private  void switchToNormalMaterialPreview(List<Material> oldMats)
    {
        int counter = 0;
       
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            rend.sharedMaterial = oldMats[counter];
            counter++;
        }
    }
    private  void switchToGlowMaterialPreview()
    {
        Highlightmanager hmanger = FindObjectOfType<Highlightmanager>();
        Byte highlightTypeTemp = highlightType;
        if (highlightTypeTemp >= hmanger.GlowMaterialList.Length)
        {
            highlightTypeTemp = 0;
            HighlightName = "Default Material at 0 Index: " + hmanger.GlowMaterialList[highlightTypeTemp].name;
        }
        else HighlightName = "Preview of: " + hmanger.GlowMaterialList[highlightTypeTemp].name;

        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
            rend.sharedMaterial = hmanger.GlowMaterialList[highlightTypeTemp];
    }
    [Button("Preview Highlight in Editor")] private void shortMaterialPreview()
    {
       if(!previewHighlightinEditor) previewHighlightinEditor = true;
    }
    [Button("Oh shit Button!!! Try to reset with Material Swap Index")] private void resetWithMatSwapIndex()
    {
        Highlightmanager hmanger = FindObjectOfType<Highlightmanager>();
        for (int i = 0; i < hmanger.RendMatIndex[matSwapIndex].rendererList.Length; i++)
            hmanger.RendMatIndex[matSwapIndex].rendererList[i].material = hmanger.MaterialsUsed[hmanger.RendMatIndex[matSwapIndex].materialIndexNumber[i]];
       if (GetComponent<Renderer>() != null) GetComponent<Renderer>().sharedMaterial = null;
    }
#endif
    private void Start()
    {
        if (highlightType > Highlightmanager.glowMaterialsStatic.Length) highlightType = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ReferenceLibrary.Player)
        {
            hex.highlightObjects(true,highlightType, matSwapIndex);
            if(Lampe == null) return;
            float playerxzVelocity = Mathf.Abs(ReferenceLibrary.PlayerRb.velocity.x) +
                                     Mathf.Abs(ReferenceLibrary.PlayerRb.velocity.z);
            if (playerxzVelocity > 73 && rotationAllowed)
            {
                int randomDirection = Random.Range(0, 1);
                if (randomDirection == 0) StartCoroutine(Rotate(Lampe,headshakes,0.17f, 27+playerxzVelocity/5, Vector3.up));
                else if (randomDirection == 1) StartCoroutine(Rotate(Lampe,headshakes,0.17f, 27+playerxzVelocity/5, Vector3.down));
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == ReferenceLibrary.Player)  hex.highlightObjects(true,highlightType,matSwapIndex);
    }
    public IEnumerator Rotate(GameObject rotateMe,int headshakes , float duration, float angle, Vector3 firstDirection)
    {
        rotationAllowed = false;
        if (headshakes == 0)
        {
            rotationAllowed = true; yield break;
        }
        Quaternion startRot = rotateMe.transform.rotation;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            rotateMe.transform.rotation = startRot * Quaternion.AngleAxis(t / duration * angle, firstDirection);
            yield return null;
        }
        headshakes--;
        yield return Rotate(rotateMe,headshakes,Random.Range(1.2f,1.4f)*duration,
            Random.Range(angle, angle + 5), -firstDirection);
    }
}