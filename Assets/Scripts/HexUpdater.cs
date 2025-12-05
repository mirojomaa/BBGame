    #if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class HexUpdater : MonoBehaviour
{
#if UNITY_EDITOR
    [NaughtyAttributes.Button("Update Particles For SlowDown Hex")]  public void updateParticle()
    {
        SerializedObject serializedHex;
        foreach (GameObject hex in GameObject.FindGameObjectsWithTag("Hex"))
        {
            if(hex.GetComponent<Hex>().hexType == HexType.SlowDown) 
            {
                serializedHex = new SerializedObject(hex.GetComponent<Hex>());
                serializedHex.FindProperty("EffectParticle").objectReferenceValue  = hex.GetComponentInChildren<ParticleSystem>();
                serializedHex.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
    
    [NaughtyAttributes.Button("Update AudioClips for Wall")]  public void updateAudioClips()
    {
        SerializedObject serializedHex;
        AudioClip fAudioClip;
        foreach (GameObject hex in GameObject.FindGameObjectsWithTag("Hex"))
        {
            foreach (Wall wall in hex.GetComponentsInChildren<Wall>())
            {
                if (wall.gameObject.GetComponent<AudioSource>() == null || wall.settings.Clip == null) continue;
                
                serializedHex = new SerializedObject(wall.gameObject.GetComponent<AudioSource>());
                 fAudioClip = wall.settings.Clip;
                 wall.gameObject.GetComponent<AudioSource>().clip = fAudioClip;
                 serializedHex.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
#endif
}


