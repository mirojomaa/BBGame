using UnityEngine;
public class PlayOnTrigger : MonoBehaviour
{
    [SerializeField] AudioSource myAudioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(ReferenceLibrary.PlayerTag) && !myAudioSource.isPlaying)myAudioSource.Play();
    }
}