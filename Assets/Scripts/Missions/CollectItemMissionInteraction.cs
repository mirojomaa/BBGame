using UnityEngine;
public class CollectItemMissionInteraction : MonoBehaviour
{
    /*
    #region Inspector
    [SerializeField] float rotation = 18;
    #endregion
    void Update() =>  transform.Rotate(new Vector3(0, rotation * Time.deltaTime, 0));
    
    */
    [SerializeField] GameObject effectParticle;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(ReferenceLibrary.PlayerTag))
        {
            ParticleSystem particle = Instantiate(effectParticle, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            particle.Play();
            ReferenceLibrary.MissionMng.ActiveMissionState.ItemCollected(gameObject);
        }
    }
}