using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Bootstrapper
{
    /*
   private const string SceneName = "PersistantScene";
   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
   public static void Execute()
   {
      //check if its loaded already
      for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; ++sceneIndex)
      {
         Scene candidate = SceneManager.GetSceneAt(sceneIndex);
         if (candidate.name == SceneName)
            return;
      }
      //add bootstrapscene
      SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
   } */
}
public class GameSceneManager : MonoBehaviour
{
   public static GameSceneManager instance; 
   private List<AsyncOperation>  hasAllTheScenesLoading = new List<AsyncOperation>();
   [SerializeField] private GameObject loadingScreen;
   public string[] tips, funnyMessages;
 public TextMeshProUGUI tipsText,funnyMessagesText, loadingText;
 public Image progressbar;
 [HideInInspector]public int tipCount, funnyMessagesCount;
 private float target;
   private void Awake()
   {
      if (instance = null)
      {
         instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else if (instance !=null) Destroy(gameObject);
      SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
   }
   public void LoadGame()
   {
      target = 0; progressbar.fillAmount = 0;
      loadingScreen.gameObject.SetActive(true);
      StartCoroutine(GenerateTips());
      hasAllTheScenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
      hasAllTheScenesLoading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.MAINGAME, LoadSceneMode.Additive));
      
      StartCoroutine(GetSceneLoadProgress());
      StartCoroutine (WaitForSceneLoad (SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MAINGAME )));
   }
   public IEnumerator WaitForSceneLoad(Scene scene)
{
   while(!scene.isLoaded) yield return null;
   Debug.Log("Setting active scene..");
   SceneManager.SetActiveScene (scene);
   //unloads the persistant scene
   hasAllTheScenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MANAGER));
}
   private float totalSceneProgress;
   public IEnumerator GetSceneLoadProgress()
   {
      for (int i = 0; i < hasAllTheScenesLoading.Count; i++)
      {
         while (!hasAllTheScenesLoading[i].isDone)
         {
            totalSceneProgress = 0;
            foreach (AsyncOperation operation in hasAllTheScenesLoading)
               totalSceneProgress += operation.progress;
            // *99 instead of *100  because a loading bar at 99% looks better 
            totalSceneProgress = Mathf.Clamp01((totalSceneProgress / hasAllTheScenesLoading.Count) / 0.9f) * 99f;
          target = Mathf.RoundToInt(totalSceneProgress);
         loadingText.text = (Mathf.RoundToInt(totalSceneProgress)).ToString();
            yield return null;
         }
      }
      loadingScreen.gameObject.SetActive(false);
   }
   void Update() => progressbar.fillAmount = Mathf.MoveTowards(
      progressbar.fillAmount, target, 10 * Time.deltaTime);
   public IEnumerator GenerateTips()
   {
      tipCount = Random.Range(0, tips.Length);
      tipsText.text = tips[tipCount];
      funnyMessagesCount = Random.Range(0, funnyMessages.Length);
      funnyMessagesText.text = funnyMessages[funnyMessagesCount];
      while (loadingScreen.activeInHierarchy)
      {
         yield return new WaitForSeconds(2f);
         tipCount++; funnyMessagesCount++;
         if (funnyMessagesCount >= funnyMessages.Length) funnyMessagesCount = 0;
         if (tipCount >= tips.Length) tipCount = 0;
         tipsText.text = tips[tipCount];
         funnyMessagesText.text = funnyMessages[funnyMessagesCount];
      }
   }
}