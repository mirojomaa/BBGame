#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using Random = UnityEngine.Random;
#endif
using UnityEngine;
public class HexManager: EditorWindow
{
#if UNITY_EDITOR
    
    public  static Dictionary<int, Material> hasAllTheHexMaterials = new Dictionary<int, Material>();
    public  static Dictionary<int, int> hasAllTheAngles= new Dictionary<int, int>();
    private string hexTag = "Hex";

    private const int leftAngle = -60;

    private const int rightAngle = 60;

    private int 
        startAtMaterial = 1, stopAtMaterial;

    private const int maxMaterials = 4;

    [MenuItem("HaMiLeJa/ Mange Hex")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HexManager));
    }
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Rotate Hex", EditorStyles.boldLabel);
        GUILayout.Label("Unbedingt Hex[x] parent bei allem auswählen, nicht in die Childs gehen. Kein Undo. Nutze die andere Richtung",
            EditorStyles.helpBox);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Alles =>")) RotateRightEverything();
        GUILayout.Space(4);
        if (GUILayout.Button("<= Alles")) RotateLeftEverything();
        GUILayout.Space(8);
        if (GUILayout.Button("Boden =>")) RotateRightChild();
        GUILayout.Space(4);
        if (GUILayout.Button("<= Boden")) RotateLeftChild();
        GUILayout.Space(8);
        if (GUILayout.Button("Props =>")) RotateRightProps();
        GUILayout.Space(4);
        if (GUILayout.Button("<= Props")) RotateLeftProps();
        GUILayout.Space(15);
        GUILayout.Label("Materials", EditorStyles.boldLabel);
        for (int i = 1; i <= maxMaterials; i++)
        {
            GUILayout.Space(6);
            if (GUILayout.Button("Set Material " + i))
                SetMaterial(i);
        }
        GUILayout.Space(15);
        GUILayout.Label("Randomize Material", EditorStyles.boldLabel);
        GUILayout.Label("Hier kannst du zwischen allen Materials randomisieren", EditorStyles.helpBox);
        if (GUILayout.Button("> Randomize Materials <")) RandomizeMaterials(1, maxMaterials, Selection.gameObjects);
        GUILayout.Space(5);
        GUILayout.Label("Bei dieser option kannst du ein start und End Material wählen", EditorStyles.helpBox);
        startAtMaterial = EditorGUILayout.IntSlider("Start at Material", startAtMaterial, 1, maxMaterials-1);
        stopAtMaterial = EditorGUILayout.IntSlider("Stop at Material", stopAtMaterial, 2, maxMaterials);
        if (GUILayout.Button("> Randomize between<"))
        {
            if (stopAtMaterial < startAtMaterial)
            {
                Debug.Log("StopValue kann nicht kleiner sein als StartValue"); 
                return;
            }
            RandomizeMaterials(startAtMaterial, stopAtMaterial, Selection.gameObjects);
        }
        GUILayout.Space(8);
        if (GUILayout.Button("Randomize Ground Rotation")) RandomizeGround(Selection.gameObjects);
        if (GUILayout.Button("Randomize ALL Hex Materials Between "))  RandomizeMaterials(startAtMaterial, stopAtMaterial, GameObject.FindGameObjectsWithTag("Hex"));
        if (GUILayout.Button("Randomize ALL Hex Ground Rotations "))  RandomizeGround(GameObject.FindGameObjectsWithTag("Hex"));
    }
    private void RandomizeGround(GameObject[] gameObjects)
    {   //randomized the HexObject Grounds by given Degree angles
        int counter = 0;
        foreach (GameObject rotateMe in gameObjects)
        { 
            if(hasAllTheAngles.Count == 0) AddDic();
           if (rotateMe.CompareTag(hexTag))
           {
               if (rotateMe.GetComponent<Hex>().hexType == HexType.BoostInDirection)
               {
                   counter++;
                   rotateMe.transform.eulerAngles = new Vector3(0, 0, 0);
                   continue;
               }
               
               int randomAngle = Random.Range(1, 6), value = hasAllTheAngles[randomAngle];
               List<GameObject> childList = new List<GameObject>();
               for (int i = 0; i < rotateMe.transform.childCount; i++)
               {
                   childList.Add(rotateMe.transform.GetChild(i).gameObject);
                   rotateMe.transform.GetChild(i).parent = null;
               }
               Rotator(rotateMe, value);
               foreach (GameObject child in childList) child.transform.parent = rotateMe.transform;
           }
       }
        if(counter > 0) Debug.Log("BoostInDirection hexes werden immer auf zero rotation gesetzt. Für " + counter + " Hex(es) wurde es gerade getan");
    }
    private void RotateRightChild()
    {
        foreach (GameObject rotateMe in Selection.gameObjects)
        {
            if (rotateMe.CompareTag(hexTag))
            {
                for (int i = 0; i < rotateMe.transform.childCount; i++)
                    Rotator(rotateMe.transform.GetChild(i).gameObject, rightAngle);
            }
        }
    }
    private void RotateLeftChild()
    {
        foreach (GameObject rotateMe in Selection.gameObjects)
        {
            if (rotateMe.CompareTag(hexTag))
            {
                for (int i = 0; i < rotateMe.transform.childCount; i++)
                Rotator(rotateMe.transform.GetChild(i).gameObject, leftAngle );
            }
        }
    }
    private void RotateRightEverything()
    {
        foreach (GameObject rotateMe in Selection.gameObjects)
        {
            if (rotateMe.GetComponent<Hex>().hexType == HexType.BoostInDirection)
            {
                Debug.Log("BoostInDirection Hexes erhalten aktuell immer eine 0 rotation: " + rotateMe.name);
                rotateMe.transform.eulerAngles = new Vector3(0, 0, 0);
                continue;
            }
             
            if(rotateMe.CompareTag(hexTag))
            Rotator(rotateMe , rightAngle);
        }
    }
    private void RotateLeftEverything()
    {
        foreach (GameObject rotateMe in Selection.gameObjects)
        {
            if (rotateMe.GetComponent<Hex>().hexType == HexType.BoostInDirection)
            {
                Debug.Log("BoostInDirection Hexes erhalten aktuell immer eine 0 rotation: " + rotateMe.name);
                rotateMe.transform.eulerAngles = new Vector3(0, 0, 0);
                continue;
            }
            if(rotateMe.CompareTag(hexTag))
            Rotator(rotateMe, leftAngle );
        }
    }
    private void RotateRightProps()
    {
        foreach (GameObject rotateMe in Selection.gameObjects)
        {
            if (rotateMe.CompareTag(hexTag))
            {
                for (int i = 0; i < rotateMe.transform.childCount; i++)
                Rotator(rotateMe.transform.GetChild(i).gameObject, rightAngle);
            }
        }
    }
    private void RotateLeftProps()
    {
        foreach (GameObject rotateMe in Selection.gameObjects)
        {
            if (rotateMe.CompareTag(hexTag))
            {
                for (int i = 0; i < rotateMe.transform.childCount; i++)
                Rotator(rotateMe.transform.GetChild(i).gameObject, leftAngle );
            }
        }
    }
    private void Rotator(GameObject gameObjectToRotate, int angle)
    {
        gameObjectToRotate.transform.Rotate(0,angle,0);
    }
    private void SetMaterial(int materialID)
    {     if(hasAllTheHexMaterials.Count == 0) AddDic();
        foreach (GameObject replaceMyMat in Selection.gameObjects)
            replaceMyMat.GetComponent<MeshRenderer>().sharedMaterial= hasAllTheHexMaterials[materialID];
    }
    private void AddDic()
    {
        hasAllTheHexMaterials.Clear();
        hasAllTheAngles.Clear();
        for (int i = 1; i <= maxMaterials; i++)
        {
            Material newMat = Resources.Load("HexMaterial " + i, typeof(Material)) as Material;
            hasAllTheHexMaterials.Add(i,newMat);
        }
        Debug.Log("Es wurden [" + maxMaterials + "] HexMaterials geladen");
        for (int i = 1; i <= 7; i++)
            hasAllTheAngles.Add(i, leftAngle*i);
    }
    private void RandomizeMaterials(int minMatIndex, int MaxMatIndex, GameObject[] gameObjects)
    {
        if(hasAllTheHexMaterials.Count == 0) AddDic();
        foreach (GameObject replaceMyMat in gameObjects)
        {
            int materialID = Random.Range(minMatIndex, MaxMatIndex+1);
           replaceMyMat.GetComponent<MeshRenderer>().sharedMaterial = hasAllTheHexMaterials[materialID];
        }
    }
#endif
}
