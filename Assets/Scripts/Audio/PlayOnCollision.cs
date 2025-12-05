using UnityEngine;
public class PlayOnCollision : MonoBehaviour
{
    AudioSource myAudioSource;
    private void Start() =>myAudioSource = GetComponent<AudioSource>();
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag) && !myAudioSource.isPlaying)myAudioSource.Play();
    }
}