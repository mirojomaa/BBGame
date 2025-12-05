using System.Collections;
using UnityEngine;
public class DownDash : MonoBehaviour
{
    #region Inspector
    //[SerializeField] float boostCost = 1;
    [Space]
    [SerializeField] private bool buttonPressedInLastFrame = false;
    [SerializeField] private float speed = 8;
    [SerializeField] private bool boostingDown = false;
    [HideInInspector] public bool isDestroying = false;
    // [SerializeField] private GameObject SlamParent;
    // [SerializeField]  private GameObject PlayerParticleParent;
    [SerializeField] private ParticleSystem smashParticle;
    [SerializeField] private AudioSource audioSource;
    private Vector3 direction;
    private float timer, boostDuration = 0.1f;
    #endregion
    void FixedUpdate()
    {
        if (GameStateManager.gameState == GameStateManager.GameState.Start) return;
        if (ReferenceLibrary.GameMng.AllowMovement == false) return;

        if (Input.GetButtonDown("A"))
        {
            if (ReferenceLibrary.GameMng.AllowMovement == false) return;
            if (ReferenceLibrary.PlayerMov.OnGround == false && buttonPressedInLastFrame == false)
            {
                ReferenceLibrary.GameMng.InputMade();
                boostingDown = true;
                buttonPressedInLastFrame = true;
                direction = ReferenceLibrary.PlayerRb.velocity.normalized;
                StartCoroutine(ReferenceLibrary.EnergyMng.ModifyEnergy(-ReferenceLibrary.GameMng.DownDashCosts));

            }
        }
        // else
        // {
        //     //buttonPressedInLastFrame = false;
        // }
        if (boostingDown)
        {
            timer += Time.deltaTime;
            if (timer < boostDuration)
            {
                /*
                if (particleCoroutineStarted == false)
                {
                    particleCoroutineStarted = true;
                    StartCoroutine(PlayParticle());
                }
                */
                ReferenceLibrary.GameMng.AllowMovement = false;
                ReferenceLibrary.PlayerRb.AddForce((ReferenceLibrary.PlayerRb.velocity.normalized/2 + Vector3.down) * speed * 100  *Time.deltaTime, ForceMode.Impulse);
                isDestroying = true;
            }
            if(ReferenceLibrary.PlayerMov.OnGround)
            {    //Stoppbewegung         //je nach winkel stopp oder rollen
                ReferenceLibrary.PlayerRb.velocity = new Vector3 (0, 0, 0); //(rb.velocity.x, 0, rb.velocity.z)
                boostingDown = false;
                StartCoroutine(DisableMovement());
                timer = 0;
                smashParticle.Play();
                if (audioSource.isPlaying == false)
                {
                    audioSource.pitch = Random.Range(0.8f, 1.6f);
                    audioSource.Play();
                }
                //touchedGround = true;
            }
        }
    }
    IEnumerator DisableMovement()
    {
        isDestroying = false;
        Vector3 pos = transform.position;
        //gameMng.AllowMovement = false;
        ReferenceLibrary.GameMng.AllowHexEffects = false;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            transform.position = pos;
            yield return null;
        }
        ReferenceLibrary.PlayerRb.AddForce(new Vector3(direction.x, 0, direction.y) * 15);
        ReferenceLibrary.GameMng.AllowMovement = true;
        ReferenceLibrary.GameMng.AllowHexEffects = true;
        buttonPressedInLastFrame = false;
        yield return null;
    }
    /*IEnumerator PlayParticle()
    {
        while (ReferenceLibrary.PlayerMov.OnGround == false)
        {
            yield return null;
        }
        Debug.Log("Coroutine");
        GameObject slam = Instantiate(SlamParent, this.transform.position, this.transform.rotation, PlayerParticleParent.transform);
       // slam.SetActive(true);
       slam.GetComponent<SlamParentScript>().Detach();
        yield return null;
    }*/
}