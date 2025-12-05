using UnityEngine;
public class TriggerZones : MonoBehaviour
{
    public GameObject PartnerZone;
    [Tooltip ("0: Zone at beginning, 1: Zone at End")] 
    public int ZoneNumber;
    /*
    [SerializeField] GameObject player;
    MovementCurves curvesMov;
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        curvesMov = player.GetComponent<MovementCurves>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != player) return;

        if(curvesMov.Sta)
        curvesMov.StartZoneFound = true;
        curvesMov.StartZone = this.gameObject;
        curvesMov.EndZone = partnerZone;
    }
    */
}
