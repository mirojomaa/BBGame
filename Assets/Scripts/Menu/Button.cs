using System.Collections;
using UnityEngine;
public class Button : MonoBehaviour
{
    private GameSceneManager _gameSceneManager;
    [SerializeField] AudioSource MenuBaseLineAudioSource;
    void Awake() =>_gameSceneManager = FindObjectOfType<GameSceneManager>();
    public void ButtonLoadGame() => StartCoroutine(DecreaseMusicVolume());   //_gameSceneManager.LoadGame();
    IEnumerator DecreaseMusicVolume()
    {
        float startVolume = MenuBaseLineAudioSource.volume, timer = 0;
        while(timer <= 0.7f)
        {
            timer += Time.deltaTime;
            MenuBaseLineAudioSource.volume = MenuBaseLineAudioSource.volume * 0.8f;
            //Debug.Log("Decreasing");
            yield return new WaitForFixedUpdate();
        }
        _gameSceneManager.LoadGame();
        yield return null;
    }
}
