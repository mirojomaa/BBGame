using System.Collections;
using UnityEngine;
public class FadeOutAudio : MonoBehaviour
{
    public IEnumerator DecreaseMusicVolume(AudioSource source)
    {
        float startVolume = source.volume, timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime;
            //MenuBaseLineAudioSource.volume = Mathf.Lerp(MenuBaseLineAudioSource.volume, 0, 0.1f);
            source.volume = source.volume * 0.9f;
            Debug.Log("Decreasing");
            yield return new WaitForFixedUpdate();
        }
        //_gameSceneManager.LoadGame();
        yield return null;
    }
}