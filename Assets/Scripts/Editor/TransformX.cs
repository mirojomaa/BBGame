/*using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TransformX
{
    private const string UNDO_STR_SNAP = "transform Objects";

    [MenuItem("Edit/Transform predefined amount%&X", isValidateFunction: true)]
    public static bool TransformSValidate()
    {
        return Selection.gameObjects.Length > 0;
    }
    
    [MenuItem("Edit/Transform predefined amount %&X")]
    public static void TransformTheThings()
    {
        foreach (GameObject go in Selection.gameObjects)
            //expensive to use 
        {Undo.RecordObject(go.transform, UNDO_STR_SNAP);
            go.transform.position =
                new Vector3(go.transform.position.x+2, go.transform.position.y, go.transform.position.z);

        }

    }
    
 
}
*/
