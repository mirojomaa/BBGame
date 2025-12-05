using UnityEngine;
public class ButtonSound : MonoBehaviour
{
   [SerializeField] AudioSource source;
   public void PlayClickSound(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
}