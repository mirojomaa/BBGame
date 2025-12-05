using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class DestroyableManager : MonoBehaviour
{
    #if UNITY_EDITOR
[HideInInspector][SerializeField] private List<Destroyables> destroyableList = new List<Destroyables>();
  [NaughtyAttributes.Button()]  public void populateDestroyableArrays()
  {
      destroyableList?.Clear();
      foreach (Destroyables destroyables in FindObjectsOfType<Destroyables>()) destroyableList.Add(destroyables);
      foreach (Destroyables destroyables in destroyableList)
      {
          if (destroyables.settings.ChangeMaterial)
          {
              HashSet<Material> uniqueMaterials = new HashSet<Material>();
              
              if(destroyables.settings.Material01 !=null)   uniqueMaterials .Add(destroyables.settings.Material01); 
              if(destroyables.settings.Material01 !=null)   uniqueMaterials .Add(destroyables.settings.Material02);
              if(destroyables.settings.Material01 !=null)   uniqueMaterials .Add(destroyables.settings.Material03);
              if(destroyables.settings.Material01 !=null)   uniqueMaterials .Add(destroyables.settings.Material04);
              List<Material> uniqueMaterialsList = uniqueMaterials.ToList();
              
              SerializedObject serializedDestroyable = new SerializedObject(destroyables);
              serializedDestroyable.FindProperty("myRenderer").objectReferenceValue = destroyables.GetComponent<Renderer>();
              
              serializedDestroyable.FindProperty("AllMaterials").ClearArray();
              serializedDestroyable.FindProperty("AllMaterials").arraySize = uniqueMaterialsList.Count ;
              
              if (uniqueMaterialsList.Count > 0)
              {
                  for (int i = 0; i < uniqueMaterialsList.Count; i++)
                      serializedDestroyable.FindProperty("AllMaterials").GetArrayElementAtIndex(i).objectReferenceValue = uniqueMaterialsList[i];
              }
              serializedDestroyable.ApplyModifiedPropertiesWithoutUndo();
          }
      }
      destroyableList?.Clear();
  }
#endif
}