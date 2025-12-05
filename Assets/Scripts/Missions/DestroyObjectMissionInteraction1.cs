using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectMissionInteraction1 : MonoBehaviour
{
    public ScriptableLevelObject settings;
    [Space]
    public AudioSource myAudioSource;
    [SerializeField] AudioClip collisionClip;
    [SerializeField] AudioClip destructionClip;

    PlayerSuperDash superDash;
    GameObject player;
    Collider col;


    void Start()
    {
        myAudioSource = this.GetComponent<AudioSource>();
        col = this.GetComponent<Collider>();

        superDash = ReferenceLibrary.SuperDash;
        player = ReferenceLibrary.Player;
    }


    private void FixedUpdate()
    {
        /*
        if (superDash.isDestroying == true)
        {
            col.isTrigger = true;
            TriggerResetted = true;
        }
        else if (TriggerResetted == false)
        {
            TriggerResetted = true;
            col.isTrigger = false;
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != player) return;


        if (superDash.isDestroying == true)
        {
            //Sound
            //Effekte

            col.enabled = false;

            ReferenceLibrary.PlayerRb.velocity *= -1;


            if (MissionManager.CurrentMission.missionType == MissionInformation.MissionType.DestroyObjs)
            {
                Debug.Log("A");
                ScoreManager.OnScoring?.Invoke(settings.value * 2);
                MissionStateActiveMission.ObjDestroyed();
            }
            else
            {
                Debug.Log("B");
                ScoreManager.OnScoring?.Invoke(settings.value);
            }

            if (myAudioSource.isPlaying == false)
            {
                myAudioSource.clip = destructionClip;
                myAudioSource.Play();
            }

            //Destroy(this.gameObject); //oder Set active + respawn

            StartCoroutine(PlayParticleAndDestroyObj());


        }
        else
        {

            if (myAudioSource.isPlaying == false)
            {
                myAudioSource.clip = collisionClip;
                myAudioSource.Play();
            }
        }

    }

    IEnumerator PlayParticleAndDestroyObj()
    {
        //play stuff
        yield return new WaitForSeconds(2f);

        this.gameObject.SetActive(false);
    }
}
