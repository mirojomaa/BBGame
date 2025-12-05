using UnityEngine;
public class PlayerParticle : MonoBehaviour
{
    public void ToggleParticle(ParticleSystem particle)
    {
        if (particle.isPlaying) particle.Stop();
        else particle.Play();
    }
}
