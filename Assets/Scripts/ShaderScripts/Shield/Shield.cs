using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    /*Renderer renderer;
    [SerializeField] AnimationCurve DisplacementCurve;
    [SerializeField] float DisplacementMagnitude;
    [SerializeField] float LerpSpeed;
    [SerializeField] float DisolveSpeed;
    bool shieldOn;
    Coroutine disolveCoroutine;

    // private Hitpoints _hitpoints;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        // _hitpoints = FindObjectOfType<Hitpoints>();
    }

    // Update is called once per frame
    void Update()
    {

        /*if (Input.GetMouseButtonDown(0))
        {
            //  Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(_hitpoints.pointOut, this.transform.position, out hit))
            {
                HitShield(hit.point);
            }

            // }
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenCloseShield();
            }

        }
    }#1#
    /*void OnCollisionEnter(Collision other)
{
    if( other.gameObject.layer == 6){
    // Print how many points are colliding with this transform
    Debug.Log("Points colliding: " + other.contacts.Length);

    // Print the normal of the first point in the collision.
    Debug.Log("Normal of the first point: " + other.contacts[0].normal);
    Debug.Log("test: " + other.contacts[0].point);

    // Draw a different colored ray for every normal in the collision
    foreach (var item in other.contacts)
    {
        Debug.DrawRay(item.point, item.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
    }
    }#1#
    //}

    // private void OnCollisionEnter(Collision collision) 
    // {
    //     ContactPoint[] contacts = new ContactPoint[1];
    //     collision.GetContacts(contacts);
    //     RaycastHit hit;
    //     Physics.Raycast(contacts[0].point, this.gameObject.transform.position, out hit);
    //     Debug.DrawLine(start:contacts[0].point, this.gameObject.transform.position, Color.red);
    //     HitShield(hit.point);
    //     
    // }
    
    /*
    public void HitShield(Vector3 hitPos)
    {
        renderer.material.SetVector("_HitPos", hitPos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
    }

    public void OpenCloseShield()
    {
        float target = 1;
        if (shieldOn)
        {
            target = 0;
        }
        shieldOn = !shieldOn;
        if (disolveCoroutine != null)
        {
            StopCoroutine(disolveCoroutine);
        }
        disolveCoroutine = StartCoroutine(Coroutine_DisolveShield(target));
    }
    #1#

    IEnumerator Coroutine_HitDisplacement()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            renderer.material.SetFloat("_DisplacementStrength", DisplacementCurve.Evaluate(lerp) * DisplacementMagnitude);
            lerp += Time.deltaTime*LerpSpeed;
            yield return null;
        }
    }

    IEnumerator Coroutine_DisolveShield(float target)
    {
        float start = renderer.material.GetFloat("_Disolve");
        float lerp = 0;
        while (lerp < 1)
        {
            renderer.material.SetFloat("_Disolve", Mathf.Lerp(start,target,lerp));
            lerp += Time.deltaTime * DisolveSpeed;
            yield return null;
        }
    }*/
}
