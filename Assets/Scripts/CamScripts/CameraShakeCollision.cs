using UnityEngine;
public class CameraShakeCollision : MonoBehaviour
{
    [HideInInspector] public bool camShakeActivated;
    [Header("Active on Everything")]
    [Space]
    [SerializeField] private bool ActivateOnEverything;
    [Space]
    [Header("Cam shake is active on the following tags")]
    [Space]
    [SerializeField] private bool Destroyable;
    [SerializeField] private bool MissionItem;
    [SerializeField] private bool Wall;
    [SerializeField] private bool Collectable;
    [SerializeField] private bool Defense; 
    //[SerializeField] private bool ItemPos;
    [SerializeField] private bool Rebound; 
    //[SerializeField] private bool Hex;
    [SerializeField] private bool HexEffectObj;
    [SerializeField] private bool Portal; 
    //[SerializeField] private bool CurvesTrigger;
    [SerializeField] private bool Trampolin;
    [SerializeField] private bool NPC;
    [SerializeField] private bool CookingPot;
    [SerializeField] private bool Critter;
    [SerializeField] private bool ChickInLantern; 
    //[SerializeField] private bool SpawnLocation;
    //[SerializeField] private bool Camerahelper;
    private void OnCollisionEnter(Collision collision)
    {
        if (Destroyable && collision.gameObject.CompareTag("Hex")) return;
        if (ActivateOnEverything && collision.gameObject.layer == 7)
            camShakeActivated = true;
        if (!ActivateOnEverything)
        {
            if(Destroyable && collision.gameObject.CompareTag("Destroyable")) camShakeActivated = true;
            if(MissionItem && collision.gameObject.CompareTag("MissionItem") ) camShakeActivated = true;
            if(Wall && collision.gameObject.CompareTag("Wall")) camShakeActivated = true;
            if(Collectable && collision.gameObject.CompareTag("Collectable")) camShakeActivated = true;
            if(Defense && collision.gameObject.CompareTag("Defense")) camShakeActivated = true;
            if(Rebound && collision.gameObject.CompareTag("Rebound")) camShakeActivated = true;
            if(HexEffectObj && collision.gameObject.CompareTag("HexEffectObj")) camShakeActivated = true;
            if(Portal && collision.gameObject.CompareTag("Portal")) camShakeActivated = true;
            if(Trampolin && collision.gameObject.CompareTag("Trampolin")) camShakeActivated = true;
            if(NPC && collision.gameObject.CompareTag("NPC")) camShakeActivated = true;
            if(CookingPot && collision.gameObject.CompareTag("CookingPot")) camShakeActivated = true;
            if(Critter && collision.gameObject.CompareTag("Critter")) camShakeActivated = true;
            if(ChickInLantern && collision.gameObject.CompareTag("ChickInLantern")) camShakeActivated = true;
        }
    }
}
