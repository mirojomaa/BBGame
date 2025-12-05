using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
     public HexEffectAudioManager HexAudMng;
    [SerializeField] private GameObject MissionAudio;
    [SerializeField] private AudioSource[] AllMissionAudioSources;
    private string mastervolume = "masterVolume";
    private void Start() => AudioListener.volume = PlayerPrefs.GetFloat(mastervolume);

    public void PlayMissionSound(AudioClip clip, AudioMixerGroup group)
    {
        AudioSource source = FindFreeMissionAudioSource();
        if (source == null) return;
        source.outputAudioMixerGroup = group;
        source.clip = clip;
       if(source.enabled) source.Play();
    }
    AudioSource FindFreeMissionAudioSource()
    {
        foreach (AudioSource source in AllMissionAudioSources)
        {
            if (source.isPlaying ) continue;
            else return source;
        }
        return null;
    }
    [Space] [SerializeField] AudioSource gameStateSource;
    public void PlayGameStateSound(AudioClip clip, AudioMixerGroup group)
    {
        gameStateSource.outputAudioMixerGroup = group;
        gameStateSource.clip = clip;
        gameStateSource.Play();
    }

    #if UNITY_EDITOR
    [NaughtyAttributes.Button("Sets all |Play OnAwake| = false and updates arrays")]  public void SetAllPlayOnAwakeFalse(bool showDebug = true)
    {
        HexAudMng = GetComponentInChildren<HexEffectAudioManager>();
        AllMissionAudioSources = MissionAudio.GetComponents<AudioSource>();
        
        
        int counterAudioSources = 0;
        int counterAudioSPlayOnAwakeEnabled = -1;
        GameObject obj = GameObject.Find("Baseline");
        for (ushort j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCount; j++)
        {
            foreach (GameObject allParentObjects in UnityEngine.SceneManagement.SceneManager.GetSceneAt(j)
                .GetRootGameObjects())
            {
                foreach (AudioSource audios in allParentObjects.GetComponentsInChildren<AudioSource>())
                {
                    counterAudioSources++;
                    if (audios.playOnAwake)
                    {
                        audios.playOnAwake = false;
                        counterAudioSPlayOnAwakeEnabled++;
                    }
                }
            }
        }
      
        obj.GetComponent<AudioSource>().playOnAwake = true;
        if (showDebug)
        {
            Debug.Log("Audiosources found: " + counterAudioSources);
            Debug.Log("Audiosources  with play on Awake set to false: " + counterAudioSPlayOnAwakeEnabled);
            if(obj.GetComponent<AudioSource>().playOnAwake)  Debug.Log("Baseline wurde aktiviert");
        }
    }
#endif
}