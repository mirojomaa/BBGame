/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCurves : MonoBehaviour
{
    public bool OnCurve = false;

    public bool StartZoneFound = false;
    public GameObject StartZone;
    public GameObject EndZone;
    [SerializeField] bool listForward = true;
    [Space]
    [SerializeField] float distanceToCurve;
    [SerializeField] float distance = 3f;

    List<GameObject> CurvesUnderPlayer = new List<GameObject>();

    GameObject currentCurve;
    MovementWay movementWay;

    Vector3 curveDirection;

    Rigidbody rb;

    [SerializeField] float speed = 30;


    bool curveFound = false;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CurvesTrigger")
        {
            if (StartZone == null)
            {
                StartZone = other.gameObject;
                EndZone = other.GetComponent<TriggerZones>().PartnerZone;
                MovementCurveStarter();
            }

            /*
            if(other.gameObject == EndZone)
            {
                StopAllCoroutines();
                //lastWaypointReached == true
                rb.AddForce(direction * 50, ForceMode.Impulse);

                OnCurve = false;
                waypointCoroutine = null;
                lastWaypointReached = false;

                StartZone = null;
                EndZone = null;
            }#1#
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == EndZone)
        {
            OnCurve = false;
            waypointCoroutine = null;
            lastWaypointReached = false;

            StartZone = null;
            EndZone = null;
        }
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        /*
        if (collision.gameObject.tag == "Curve")
        {
            if (OnCurve == false)
            {

                OnCurve = true;

                currentCurve = collision.gameObject.transform.parent.gameObject;
                movementWay = currentCurve.GetComponentInChildren<MovementWay>();

                //FindStartpoint (and Direction?)

                if (waypointCoroutine == null)
                    waypointCoroutine = StartCoroutine(MoveToWaypoint(0)); //Startpoint reingeben
            }



        }
        #1#

    }
    private void OnCollisionExit(Collision collision)
    {
        /*
        if (collision.gameObject.tag == "Curve")
        {
            if (lastWaypointReached == true)
            {
                StartCoroutine(WaitForLeaveCurve());
                rb.AddForce(direction * 50, ForceMode.Impulse);
            }
        }
        #1#

    }

    void MovementCurveStarter()
    {
        OnCurve = true;
        currentCurve = StartZone.transform.parent.gameObject;
        movementWay = currentCurve.GetComponentInChildren<MovementWay>();

        int startPoint;
        if (StartZone.GetComponent<TriggerZones>().ZoneNumber == 0)
        {
            listForward = true;
            startPoint = 0;
        }
        else
        {
            listForward = false;
            startPoint = movementWay.Waypoints.Count - 1;
        }
            


        if (waypointCoroutine == null)
            waypointCoroutine = StartCoroutine(MoveToWaypoint(startPoint)); //Startpoint reingeben
    }

   

    IEnumerator WaitForLeaveCurve()
    {
        RaycastHit hit = new RaycastHit();
        while (OnCurve == true)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hit, 20, LayerMask.GetMask("Level")))
            {
                if (hit.collider.tag == "Curve")
                {
                    OnCurve = true;
                }
                else
                    OnCurve = false;

            }
            yield return null;
        }

        OnCurve = false;
        waypointCoroutine = null;
        lastWaypointReached = false;
        StopAllCoroutines();

    }


    Coroutine waypointCoroutine;
    public bool lastWaypointReached = false;
    [SerializeField] SphereCollider myCollider;
    Vector3 direction;

    IEnumerator MoveToWaypoint(int point)
    {
        //Debug.Log(point);
        bool waypointReached = false;


        while (waypointReached == false)
        {
            GameObject waypoint = movementWay.Waypoints[point];
            
            direction = new Vector3(waypoint.transform.position.x - this.transform.position.x, waypoint.transform.position.y - this.transform.position.y, waypoint.transform.position.z - this.transform.position.z).normalized;
            Vector3 movement = direction * Time.deltaTime * speed;
            Vector3 lerpedMovement = new Vector3(Mathf.Lerp(this.transform.position.x, this.transform.position.x + movement.x, 10), Mathf.Lerp(this.transform.position.y, this.transform.position.y + movement.y, 10), Mathf.Lerp(this.transform.position.z, this.transform.position.z + movement.z, 10));

            rb.MovePosition(lerpedMovement);

            Collider[] waypointCollider;
            waypointCollider = Physics.OverlapSphere(this.transform.position, myCollider.radius + 1, LayerMask.GetMask("Waypoints")); //LayerMask.GetMask("World")
            if (waypointCollider != null)
            {
                for (int i = 0; i < waypointCollider.Length; i++)
                {
                    if (waypointCollider[i].gameObject == movementWay.Waypoints[point])
                    {
                        waypointReached = true;
                        //Debug.Log("waypoint reached");
                    }
                }

            }

            yield return null;
        }


        if (listForward == true)
        {
            Debug.Log("List forward");
            if (point >= movementWay.Waypoints.Count - 1)
            {
                lastWaypointReached = true;
                rb.AddForce(direction * 50, ForceMode.Impulse);

            }
            else
            {
                StartCoroutine(MoveToWaypoint(point + 1));
            }
        }

        if (listForward == false)
        {
            Debug.Log("List backwards");

            if (point <= 0)
            {
                lastWaypointReached = true;
                rb.AddForce(direction * 50, ForceMode.Impulse);
            }
            else
            {
                StartCoroutine(MoveToWaypoint(point - 1));
            }
        }

        yield return null;
    }

}*/