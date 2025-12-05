using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour
{
    public static GameState gameState = GameState.Start;
    [SerializeField] AudioSource myAudioSource;
    bool MainSceneLoadActive = false;
    public enum GameState
    {
        Start,
        Play,
        Pause,
        End
    }
    void Start()
    {
        MainSceneLoadActive = false;
        if (myAudioSource == null) myAudioSource = GetComponent<AudioSource>();
        GameOver = false;
        gameState = GameState.Start;
    }
    void UpdateStartGame()
    {
        if (Input.GetButtonDown("A"))
        {
            myAudioSource.Play();
            gameState = GameState.Play;
            ReferenceLibrary.UIMng.DeactivateStartOfGameUI();
            Vector3 random = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            ReferenceLibrary.PlayerRb.AddForce(random * 5);
        }
    }
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (MainSceneLoadActive) return;
            switch (gameState)
            {
                case GameState.Play: myAudioSource.Play(); PauseGame(); break;
                case GameState.Pause: myAudioSource.Play(); ResumeGame(); break;
                default: break;
            }
        }
        if(gameState == GameState.Start) UpdateStartGame();
        if(gameState == GameState.Pause )
        {
            if (MainSceneLoadActive) return;
            if (Input.GetButtonDown("B"))
            {
                myAudioSource.Play();
                MainSceneLoadActive = true;
                StartCoroutine(AdvancedDelayedSceneMenuSceneLoad());
            }
        }

        if(gameState == GameState.End)
        {
            if (MainSceneLoadActive) return;
            if (Input.GetButtonDown("B"))
            {
                MainSceneLoadActive = true;
                myAudioSource.Play();
                StartCoroutine(DelayedSceneMenuSceneLoad());
            }
        }
    }
    IEnumerator AdvancedDelayedSceneMenuSceneLoad()
    {
        Debug.Log("DelayedLoad1");
        Time.timeScale = 0.005f;
        yield return new WaitForSeconds(0.001f);
        Debug.Log("DelayedLoad2");
        loadMainScreen();
    }
    IEnumerator DelayedSceneMenuSceneLoad()
    {
        yield return new WaitForSeconds(0.3f);
        loadMainScreen();
    }
    void loadMainScreen()
    {
        // AddAllManagers();
        // DestroyAllTheManagers();
        // for (int j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCount; j++)
        // {
        //     foreach (GameObject allParentObjects in UnityEngine.SceneManagement.SceneManager.GetSceneAt(j)
        //         .GetRootGameObjects())
        //      
        //     {  if(allParentObjects != this.gameObject)
        //         Destroy(allParentObjects);
        //       
        //     }
        //     
        // }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // UnsubscribeEvents();
        SceneManager.UnloadSceneAsync((int)SceneIndexes.MAINGAME);
        SceneManager.LoadScene((int)SceneIndexes.MANAGER);
        Time.timeScale = 1;
        //Destroy(this);
    }
    void UnsubscribeEvents()
    {
        Debug.Log("Unsubscirbe Events");
        ScoreManager.OnScoring -= ReferenceLibrary.ScoreMng.UpdateScore;
        ScoreManager.OnScoring -= ReferenceLibrary.UIMng.UpdateUIScore;
        ScoreManager.OnScoring -= ReferenceLibrary.UIMng.PointsStarter;
        
        ScoreManager.OnPermanentMultiplicatorUpdate -= ReferenceLibrary.ScoreMng.UpdateMultiplicator;
        ScoreManager.OnPermanentMultiplicatorUpdate -= ReferenceLibrary.UIMng.UpdateMultiplicatorUI;
        ScoreManager.OnPermanentMultiplicatorUpdate -= ReferenceLibrary.UIMng.PermanentMulitplicatorStarter;

        ScoreManager.OnTemporaryMultiplicatorUpdate -= ReferenceLibrary.ScoreMng.UpdateMultiplicator;
        ScoreManager.OnTemporaryMultiplicatorUpdate -= ReferenceLibrary.UIMng.UpdateMultiplicatorUI;
        ScoreManager.OnTemporaryMultiplicatorUpdate -= ReferenceLibrary.UIMng.UpdateTemporaryMultiplicator;

        CollectableManager.OnRespawnCollectables -= ReferenceLibrary.ColMng.spawnCollectableObjects;
    }
    
    void PauseGame()
   {
        ReferenceLibrary.UIMng.IngameCanvas.SetActive(false);
        ReferenceLibrary.UIMng.PauseCanvas.SetActive(true);
        Time.timeScale = 0;
        gameState = GameState.Pause;
   }
    void ResumeGame()
   {
        ReferenceLibrary.UIMng.IngameCanvas.SetActive(true);
        ReferenceLibrary.UIMng.PauseCanvas.SetActive(false);
        Time.timeScale = 1;
        gameState = GameState.Play;
    }
    #region EndOfGame
    Coroutine GameOverCoroutine;
    [SerializeField] private Dissolve playerDissolve;
    public static bool GameOver = false;
    Vector3 velocityLastFrame, velocitySecondToLastFrame;
    [Space] [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup gameOverGroup;
    public void CheckForEndOfGame()
    {
        if (Mathf.Approximately(ReferenceLibrary.PlayerRb.velocity.x, 0) && Mathf.Approximately(ReferenceLibrary.PlayerRb.velocity.y, 0) && Mathf.Approximately(ReferenceLibrary.PlayerRb.velocity.z, 0))
        {
            if (GameOver) return;
            StopAllCoroutines();
            gameState = GameState.End;
            CalculateEndOfGame();
        }
        else // vergleichen der Velocity des vorherigen frames mit dem der aktuellen;
        {
            if (velocityLastFrame == ReferenceLibrary.PlayerRb.velocity)
            {
                if (velocityLastFrame == velocitySecondToLastFrame) return;
                StartCoroutine(EndGameSavety(velocityLastFrame));
            }
            else StopAllCoroutines();
            velocitySecondToLastFrame = velocityLastFrame;
            velocityLastFrame = ReferenceLibrary.PlayerRb.velocity;
        }
    }
    void CalculateEndOfGame()
    {
        Debug.Log("GameOver");
        ReferenceLibrary.AudMng.PlayGameStateSound(gameOverClip, gameOverGroup);
        GameOver = true;
        if (ReferenceLibrary.ScoreMng.CheckForNewHighscore())
        {
            ReferenceLibrary.ScoreMng.SetNewHighscore();
            if (GameOverCoroutine == null) GameOverCoroutine = StartCoroutine(ReferenceLibrary.UIMng.GameOverNewHighscoreCoroutine());
            Debug.Log("new highscore");
            StartCoroutine(playerDissolve.Coroutine_DisolveShield(1.1f));
        }
        else
        {
            if (GameOverCoroutine == null) GameOverCoroutine = StartCoroutine(ReferenceLibrary.UIMng.GameOverCoroutine());
            else Debug.Log("GameOverCoroutine not null, whyever");
            Debug.Log("no new highscore");
            StartCoroutine(playerDissolve.Coroutine_DisolveShield(1.1f));
        }
    }
    IEnumerator EndGameSavety(Vector3 lastVelocity)
    {
        yield return new WaitForSeconds(3f);
        if (lastVelocity == ReferenceLibrary.PlayerRb.velocity && !GameOver) GameOver = true;
        else yield break;
        yield return new WaitForSeconds(2f);
        gameState = GameState.End;
        CalculateEndOfGame();
        yield return null;
    }
    #endregion
}