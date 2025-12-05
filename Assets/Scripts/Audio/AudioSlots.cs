/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSlots : MonoBehaviour
{
    #region Singleton
    public static AudioSlots Instance;
    private void Awake()
    {
        if(Instance != null) 
        { Destroy(gameObject); }
        else 
        { Instance = this; }
    }
    #endregion
    public static bool slotA = true, slotB = true , slotC = true, slotD = true, needSlot = true;
    public void playSoundSlots(AudioSource audioSource, AudioClip audioClip, float volume)
    {
        if (!slotA && !slotB && !slotC && !slotD) return;
        needSlot = true;
        if (slotA && needSlot) StartCoroutine(Coroutine_slotA(audioClip.length, audioSource,audioClip, volume));
        if (slotB && needSlot) StartCoroutine(Coroutine_slotB(audioClip.length, audioSource,audioClip, volume));
        if (slotC && needSlot) StartCoroutine(Coroutine_slotC(audioClip.length, audioSource,audioClip, volume));
        if (slotD && needSlot) StartCoroutine(Coroutine_slotD(audioClip.length, audioSource,audioClip, volume));
    }
    
    IEnumerator Coroutine_slotA(float sec, AudioSource audioSource, AudioClip audioClip, float volume)
    {
        needSlot = false; slotA = false;
        if(!audioSource.isPlaying) audioSource.PlayOneShot(audioClip, volume);
        yield return new WaitForSeconds(sec);
        slotA = true;
    }
    IEnumerator Coroutine_slotB(float sec, AudioSource audioSource, AudioClip audioClip, float volume)
    {    
        needSlot = false;
        slotB = false;
        if(audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(audioClip, volume);
        }
        yield return new WaitForSeconds(sec);
        slotB = true;
    }
    IEnumerator Coroutine_slotC(float sec, AudioSource audioSource, AudioClip audioClip, float volume)
    { 
        needSlot = false;
        slotC = false;
        if(audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(audioClip, volume);
        }
        yield return new WaitForSeconds(sec);
        slotC = true;
    }
    IEnumerator Coroutine_slotD(float sec, AudioSource audioSource, AudioClip audioClip, float volume)
    {    
        needSlot = false;
        slotD = false;
        if(audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(audioClip, volume);
        }
        yield return new  WaitForSeconds(sec);
        slotD = true;
    }
}
*/
