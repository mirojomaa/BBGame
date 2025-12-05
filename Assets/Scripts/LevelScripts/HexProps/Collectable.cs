using UnityEngine;
public class Collectable : MonoBehaviour
{
    #region Inspector
    [SerializeField] public int CollectableIndexID;
    public ScriptableLevelObject settings;
   // public CollectableReferences colRef;
   #endregion
   private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(ReferenceLibrary.PlayerTag))
        {
            ScoreManager.OnScoring?.Invoke(settings.value);
            ReferenceLibrary.ColMng.CollectableCollected(settings.secondValue,CollectableIndexID); //hier drin sind auch sounds;
        }
    }

}