using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DestroyObjectMissionInteraction : MonoBehaviour
{
    public DestroyableScriptableObject settings;
    [Space]
   [SerializeField] Collider col;

    private Rigidbody Rigidbody;
    [SerializeField]public AudioSource myAudioSource;
    private GameObject brokenInstance;
//    bool TriggerResetted = false;
    Renderer myRenderer;
    List<Material> AllMaterials = new List<Material>();
    int collisionCounter = 0;
    void Start()
    {
        if (settings.ChangeMaterial)
        {
            myRenderer = GetComponent<Renderer>();
            AllMaterials.Add(settings.Material01);
            AllMaterials.Add(settings.Material02);
            AllMaterials.Add(settings.Material03);
            AllMaterials.Add(settings.Material04);
        }
        collisionCounter = 0;
    }
    public void Explode()
    {
        Destroy(Rigidbody);
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        if (settings.DestructionClip != null)
        {
            if (!myAudioSource.isPlaying)
            {
                myAudioSource.clip = settings.DestructionClip;
                myAudioSource.outputAudioMixerGroup = settings.DestroyGroup;
                myAudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                myAudioSource.Play();
            }
        }
        GameObject brokenPrefabCopy = settings.BrokenPrefab;
        brokenInstance = Instantiate(brokenPrefabCopy, transform.position + settings.DestroyedCubeAdditionalDistance, transform.rotation);

        Rigidbody[] rigidbodies = brokenInstance.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in rigidbodies)
        {
            if (Rigidbody != null) body.velocity = Rigidbody.velocity;
            body.AddExplosionForce(settings.ExplosiveForce, transform.position, settings.ExplosiveRadius);
        }
        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }
    private IEnumerator FadeOutRigidBodies(Rigidbody[] Rigidbodies)
    {
        WaitForSeconds Wait = new WaitForSeconds(settings.SleepCheckDelay);
        float activeRigidbodies = Rigidbodies.Length;

        while (activeRigidbodies > 0)
        {
            yield return Wait;
            foreach (Rigidbody rigidbody in Rigidbodies)
            {
                if (rigidbody.IsSleeping()) activeRigidbodies--;
            }
        }
        yield return new WaitForSeconds(settings.DestroyDelay);
        float time = 0;
        Renderer[] renderers = Array.ConvertAll(Rigidbodies, GetRendererFromRigidbody);
        foreach (Rigidbody body in Rigidbodies)
        {
            Destroy(body.GetComponent<Collider>());
            Destroy(body);
        }
        while (time < 1)
        {
            float step = Time.deltaTime * settings.FadeSpeed;
            foreach (Renderer renderer in renderers)
            {
                renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
            }
            time += step;
            yield return null;
        }
        foreach (Renderer renderer in renderers) Destroy(renderer.gameObject);
        if (settings.Respawn)
        {
            StopCoroutine(Coroutine_ResetObject());
            StartCoroutine(Coroutine_ResetObject());
        }
        if (!settings.Respawn) Destroy(gameObject);
    }
    private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody) =>Rigidbody.GetComponent<Renderer>();
    IEnumerator Coroutine_ResetObject()
    {
        yield return new WaitForSeconds(settings.resetTimer);
        //Change Material
        if (settings.ChangeMaterial) myRenderer.material = AllMaterials[UnityEngine.Random.Range(0, 4)];
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
    }
    int DestroyCounter;
    int hitCounter;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag)) return;
        if (settings.AllowAutomatedDestruction) collisionCounter++;
        ReferenceLibrary.PlayerRb.velocity = new Vector3(ReferenceLibrary.PlayerRb.velocity.x, ReferenceLibrary.PlayerRb.velocity.y / 4, ReferenceLibrary.PlayerRb.velocity.z); //Damit der Player nicht so sehr hoch fliegt bei Collision

        if (ReferenceLibrary.SuperDash.isDestroying || ReferenceLibrary.DownDashPl.isDestroying|| collisionCounter >= settings.HitAmount)
        {
            float missionscoreMultiplicator = 1;

            if (MissionManager.CurrentMission != null)
            {
                if (MissionManager.CurrentMission.missionType == MissionInformation.MissionType.DestroyObjs)
                {
                    missionscoreMultiplicator = 3;
                    MissionStateActiveMission.ObjDestroyed();
                }
                else missionscoreMultiplicator = 1;
            }
            col.enabled = false;
            ReferenceLibrary.PlayerRb.velocity *= -1.2f;
            Explode();
            if (DestroyCounter >= 15)
            {
                float points15 = settings.DestroyValue / 15;
                if (points15 < 1) points15 = 1;
                ScoreManager.OnScoring?.Invoke(points15 * missionscoreMultiplicator);
            }
            else
            {
                float scoreValue = ((DestroyCounter * 0.05f)) * settings.DestroyValue;
                float points = settings.DestroyValue - DestroyCounter;
                if (points <= 1) points = 1;
                ScoreManager.OnScoring?.Invoke(points * missionscoreMultiplicator);
            }
            DestroyCounter++;
            collisionCounter = 0;
        }
        else
        {
            if (hitCounter >= 15)
            {
                float points = settings.CollisionValue / 15;
                if (points < 1) points = 1;
                ScoreManager.OnScoring?.Invoke(points);
            }
            else
            {
                float scoreValue = ((hitCounter * 0.05f)) * settings.CollisionValue;
                float points = settings.CollisionValue - scoreValue;
                if (points < 1) points = 1;
                ScoreManager.OnScoring?.Invoke(points);
            }
            hitCounter++;
            if (!myAudioSource.isPlaying)
            {
                myAudioSource.clip = settings.CollisionClip;
                myAudioSource.outputAudioMixerGroup = settings.CollisionGroup;
                myAudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                myAudioSource.Play();
            }
        }
    }
}