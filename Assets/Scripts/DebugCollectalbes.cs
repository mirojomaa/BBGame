using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugCollectalbes : MonoBehaviour
{

    public Collectable[] AllCollectables;
   public  bool done = false;

    void Update()
    {
        if(done == false)
        {
            AllCollectables = null;
            AllCollectables = GetComponentsInChildren<Collectable>();
            done = true;
        }
    }
}
