using UnityEngine;
public class EnergyGenerator : MonoBehaviour
{
    [Tooltip("How much energy this generatior currently has")]
    [SerializeField] float generatedEnergy = 0f;
    //[SerializeField] GameObject player;
    [SerializeField] ScriptableEnergyGenerator settings;
    [SerializeField] AudioSource mySource;
    float timer;
    private void Start() =>mySource.clip = settings.clip;
    private void Update()
    {
        if (generatedEnergy >= settings.maxEnergy) return;

        timer += Time.deltaTime;

        if (timer > settings.energyGenerationRate)
        {
            generatedEnergy += settings.energyGenerationAmount;
            timer = 0;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == ReferenceLibrary.Player)
        {
            EnergyManager.energyGotHigher = true;
            StartCoroutine(ReferenceLibrary.EnergyMng.ModifyEnergy(generatedEnergy));
            generatedEnergy = 0;

            if(mySource.isPlaying == false)
                mySource.Play();
        }
    }
}