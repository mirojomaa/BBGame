using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Destroyables : MonoBehaviour
{
    public DestroyableScriptableObject settings;
    [Space]
    public AudioSource myAudioSource;

    public Material[] AllMaterials;
    
   [SerializeField] private Collider[] col;
   private GameObject brokenInstance;
   // bool TriggerResetted = false;
    [SerializeField] public Renderer myRenderer;
    int collisionCounter = 0;
    
    public void Explode()
    {
        foreach(Collider turnMeOff in col) turnMeOff.enabled = false;
        myRenderer.enabled = false;
        if (settings.DestructionClip != null)
       {
            if (myAudioSource.isPlaying == false)
            {
                myAudioSource.clip = settings.DestructionClip;
                myAudioSource.outputAudioMixerGroup = settings.DestroyGroup;
                myAudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                myAudioSource.Play();
            }
       }
       GameObject brokenPrefabCopy = settings.BrokenPrefab;
       brokenInstance = Instantiate(brokenPrefabCopy, transform.position + 
                                                      settings.DestroyedCubeAdditionalDistance, transform.rotation);
       DestroyedPrefabData brokenInstanceComponent = brokenInstance.GetComponent<DestroyedPrefabData>();
        foreach (Rigidbody body in brokenInstanceComponent.rigidbodys)
         body.AddExplosionForce(settings.ExplosiveForce, transform.position, settings.ExplosiveRadius);
        StartCoroutine(FadeOutRigidBodies(brokenInstanceComponent.rigidbodys, brokenInstanceComponent.renderers));
    }
    private IEnumerator FadeOutRigidBodies(Rigidbody[] Rigidbodies, Renderer[] renderers)
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
        foreach(Rigidbody body in Rigidbodies)
        {
            Destroy(body.GetComponent<Collider>());
            Destroy(body);
        }
        while(time < 1)
        {
            float step = Time.deltaTime * settings.FadeSpeed; 
            foreach (Renderer renderer in renderers) 
                renderer.transform.Translate(Vector3.down * 
                (step / renderer.bounds.size.y), Space.World);
            time += step;
            yield return null;
        }
        foreach (Renderer renderer in renderers) Destroy(renderer.gameObject);
        if (settings.Respawn)
        {
            StopCoroutine(Coroutine_ResetObject());
            StartCoroutine(Coroutine_ResetObject());
        }
        if(!settings.Respawn)  Destroy(gameObject);
    }
    
    IEnumerator Coroutine_ResetObject()
    {
        yield return new WaitForSeconds(settings.resetTimer);
        if(settings.ChangeMaterial) //Change Material
            myRenderer.material = AllMaterials[UnityEngine.Random.Range(0, 4)];
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
    }
    int DestroyCounter;
    int hitCounter;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag)) return;
        if (settings.AllowAutomatedDestruction) collisionCounter++;
        if (ReferenceLibrary.SuperDash.isDestroying || ReferenceLibrary.DownDashPl.isDestroying || collisionCounter >= settings.HitAmount)
        {
            foreach(Collider collide in col) collide.enabled = false;
            ReferenceLibrary.PlayerRb.velocity *= -1.2f;
            Explode();
            if (DestroyCounter >= 15)
            {
                float points15 = settings.DestroyValue / 15;
                if (points15 < 1) points15 = 1;
                ScoreManager.OnScoring?.Invoke(points15);
            }
            else
            {
                float points = settings.DestroyValue - DestroyCounter;
                if (points <= 1) points = 1;
                ScoreManager.OnScoring?.Invoke(points);
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
            if (myAudioSource.isPlaying == false)
            {
                myAudioSource.clip = settings.CollisionClip;
                myAudioSource.outputAudioMixerGroup = settings.CollisionGroup;
                myAudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.6f);
                myAudioSource.Play();
            }
        }
    }
}
