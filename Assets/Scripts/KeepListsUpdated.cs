
using UnityEngine;
public class KeepListsUpdated : MonoBehaviour
{
    #if UNITY_EDITOR
        [NaughtyAttributes.Button()] private void UpdateAllPreCalculatedLists()
        {
            HexAutoTiling autottiling = FindObjectOfType<HexAutoTiling>().GetComponent<HexAutoTiling>();
            autottiling.FindAllTheHexesTransform();
            autottiling.SetPlayerPositionOnStart();
            
            Highlightmanager highlightmanager = FindObjectOfType<Highlightmanager>().GetComponent<Highlightmanager>();
            highlightmanager.UpdateAllMaterialIndexies();
            highlightmanager.UpdateHexParentsInHighlightObj();
            highlightmanager.UpdateHasOwnGlow();

            ReferenceLibrary refLibarary = FindObjectOfType<ReferenceLibrary>().GetComponent<ReferenceLibrary>();
            refLibarary.FillNullHashsetRefLibary();
            refLibarary.populateList();

            HexUpdater hexUpdater = FindObjectOfType<HexUpdater>().GetComponent<HexUpdater>();
            hexUpdater.updateParticle();
            hexUpdater.updateAudioClips();
            
         FindObjectOfType<HexEffectAudioManager>().GetComponent<HexEffectAudioManager>().getAllTheAudioSourcesBeforeAwake();
         FindObjectOfType<CollectableManager>().GetComponent<CollectableManager>().fillCollectableListsBeforeStart();
         FindObjectOfType<CurveManager>().GetComponent<CurveManager>().UpdateCollider();
         FindObjectOfType<DestroyableManager>().GetComponent<DestroyableManager>().populateDestroyableArrays();
         FindObjectOfType<CameraZoomOut>().GetComponent<CameraZoomOut>().addCashedValues();
         FindObjectOfType<AudioManager>().GetComponent<AudioManager>().SetAllPlayOnAwakeFalse(false);
        FindObjectOfType<UIManager>().GetComponent<UIManager>().SetUICorrect();
        Debug.Log("Everything possible in the Game has been serialized and updated");
        }
    #endif
}

