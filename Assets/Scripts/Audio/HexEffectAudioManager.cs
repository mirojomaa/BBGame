using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
public class HexEffectAudioManager : MonoBehaviour
{
    public List<StringAudiofileClass> AllHexClips = new List<StringAudiofileClass>();
    [SerializeField] private Dictionary<HexType, AudioClip> AllHexTypesAndClips = new Dictionary<HexType, AudioClip>();
    public List<HexTypeAndMixerGroup> AllOutputGroups = new List<HexTypeAndMixerGroup>();
   [SerializeField] private  Dictionary<HexType, AudioMixerGroup> AllOutputGroupsForHextypes = new Dictionary<HexType, AudioMixerGroup>();
    //int freeAudioSources;
    [System.Serializable]
    public class HexTypeAndMixerGroup
    {
        public HexType type;
        public AudioMixerGroup group;
    }
   [SerializeField]private AudioSource[] AllHexAudioSources;
   
#if UNITY_EDITOR
    [NaughtyAttributes.Button()] public void getAllTheAudioSourcesBeforeAwake() => AllHexAudioSources = GetComponents<AudioSource>();
#endif
    private void Awake()
    {
        // freeAudioSources = AllHexAudioSources.Length;
        AllOutputGroupsForHextypes?.Clear();
        AllHexTypesAndClips?.Clear();
        foreach(HexTypeAndMixerGroup file in AllOutputGroups) AllOutputGroupsForHextypes.Add(file.type, file.group);
        foreach (StringAudiofileClass file in AllHexClips) AllHexTypesAndClips.Add(file.type, file.clip);
    }

    public void PlayHex(HexType type)
    {
        AudioSource mySource = SetAudioSource();
        if (mySource == null) return;
        mySource.outputAudioMixerGroup = AllOutputGroupsForHextypes[type];
        mySource.pitch = Random.Range(0.8f, 1.6f);
        mySource.clip = AllHexTypesAndClips[type];
        mySource.Play();
        // audiosource.play(AllHextypes.type) oder so
    }
    AudioSource SetAudioSource()
    {
        foreach(AudioSource obj in AllHexAudioSources)
        {
            if(obj.isPlaying) continue;
            else return obj;  //freeAudioSources++;
        }
        Debug.Log("Not Enough Hex Audio Sources");
        return null;
    }
}