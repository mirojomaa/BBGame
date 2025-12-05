using UnityEngine;
using NaughtyAttributes;
public class Waypoint : MonoBehaviour
{
    [InfoBox("Rebuild nur falls was schief läuft. Ist auch im Parent und wird bei instanziieren gemacht", EInfoBoxType.Normal)]
    public Waypoint NextPoint, PreviousPoint;
    
    
    #if UNITY_EDITOR
    [Button()] void RebuildList() =>transform.parent.GetComponent<Pathfinder>().AllChildsToList();
    private void OnDrawGizmosSelected() => CurveManager.updatePathfinder = true;
#endif
}