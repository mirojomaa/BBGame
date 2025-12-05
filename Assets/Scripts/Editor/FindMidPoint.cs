using UnityEditor;
using UnityEngine;
public class FindMidPoint 
{
    [MenuItem("HaMiLeJa/Find MidPoint t%&W", isValidateFunction: true)]
    public static bool FindTheMidPointValidate() => Selection.gameObjects.Length > 0;
    
    [MenuItem("HaMiLeJa/Find MidPoint %&W")]
    public static void FindTheMidPoint()
    {
       float xPos = 0, yPos = 0, zPos = 0, objCounter = 0;
       foreach (GameObject Object in Selection.gameObjects)
       {
            xPos += Object.transform.position.x;
            yPos += Object.transform.position.y;
            zPos += Object.transform.position.z;
            objCounter++;
        }
       xPos = xPos/ objCounter; yPos = yPos/ objCounter; zPos = zPos/ objCounter;
        Debug.Log("-------------------");
        Debug.Log("X Mid: [" + xPos + "]");
        Debug.Log("Y Mid: [" + yPos + "]");
        Debug.Log("Z Mid: [" + zPos + "]");
        Debug.Log("-------------------");
    }
}



    
