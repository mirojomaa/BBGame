using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class Shuffler : EditorWindow
{
    private GameObject pivot;
    private bool pivotHasMoved,
        rightAvailable = true, leftAvailable = true, 
        lastPressedLeft, lastPressedRight, 
        topAvailable = true, bottomAvailable = true,
        lastPressedTop, lastPressedBottom,
        inverseRT = true, inverseLB = true,
        lastPressedInverseRT, lastPressedInverseLB;
    
    List<GameObject> hasAllTheHexes = new List<GameObject>();
    
    [MenuItem("HaMiLeJa/ Shuffler")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Shuffler));
    }
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Shuffler", EditorStyles.boldLabel);
        pivot = EditorGUILayout.ObjectField("Pivot Object: ", pivot, typeof(GameObject), true) as GameObject;
        GUILayout.Space(5);
        GUILayout.Label("First Chose a Pivot and make a List. Doing it once is enough.", EditorStyles.helpBox);
        GUILayout.Space(5);
        
        if (GUILayout.Button("Make List")) safeHexesToList();
        
        GUILayout.Space(5);
        GUILayout.Label(" Now chose how you want to shuffle. Just one Categorie [left<>right] [top<>bottom] [inverseMapRT<>InverseMaplB].", EditorStyles.helpBox);
        GUILayout.Space(5);
        GUILayout.Label("Don't mix Categories and remember to shuffle stuff back once you are done.", EditorStyles.helpBox);
        GUILayout.Space(15);

        GUI.enabled = rightAvailable;
        if (GUILayout.Button("Shuffle Right"))
        {
            moveToTheRight();
            leftAvailable = true; rightAvailable = false; 
            lastPressedRight = !lastPressedRight;
        }

        GUI.enabled = true; GUI.enabled = leftAvailable; GUILayout.Space(5);
        if (GUILayout.Button("Shuffle Left"))
        {
            moveToTheLeft();
            leftAvailable = false; rightAvailable = true;
            lastPressedLeft = !lastPressedLeft;
        }

        GUI.enabled = true; GUI.enabled = topAvailable; GUILayout.Space(18);
        if (GUILayout.Button("Top"))
        {
            moveToTheTop();
            topAvailable = false; bottomAvailable = true;
            lastPressedTop = !lastPressedTop;
        }
        
        GUI.enabled = true; GUI.enabled = bottomAvailable;
        if (GUILayout.Button("Bottom"))
        {
            moveToTheBottom();
            topAvailable = true; bottomAvailable = false;
            lastPressedBottom = !lastPressedBottom;
        }
        
        GUI.enabled = true; GUI.enabled = inverseRT;  GUILayout.Space(18);
        if (GUILayout.Button("Inverse Map RT"))
        {
            moveToTheRight(); moveToTheTop();
            inverseRT = false; inverseLB = true;
            lastPressedInverseRT = !lastPressedInverseRT;
        }
        
        GUI.enabled = true; GUI.enabled = inverseLB ;
        if (GUILayout.Button("Inverse Map LB"))
        {
           moveToTheLeft(); moveToTheBottom();
           inverseLB = false; inverseRT = true;
           lastPressedInverseLB = !lastPressedInverseLB;
        }
     
        GUI.enabled = true; GUILayout.Space(18);
        if (GUILayout.Button("Release Button Pressed")) restAllButtons();
        
        void safeHexesToList()
        {
            if (pivot == null)
            {
                Debug.Log("Choose a Pivot first"); return;
            }
            hasAllTheHexes.Clear();
            Debug.Log("List cleared");
            hasAllTheHexes.AddRange(GameObject.FindGameObjectsWithTag("Hex"));
            Debug.Log("Made new List");
        }
        
        void nullcheck()
        {
            if(hasAllTheHexes == null) 
                safeHexesToList();
        }
      
        void moveToTheTop()
        {
            nullcheck();
            foreach (GameObject hex in hasAllTheHexes)
            {
                if (hex.transform.position.z < pivot.transform.position.z)
                    hex.transform.position = new Vector3(hex.transform.position.x,
                        hex.transform.position.y, hex.transform.position.z + HexAutoTiling.zTilingDistance);
                if (!pivotHasMoved) pivotHasMoved = true;
            }

            if (pivotHasMoved)
            {
                pivot.transform.position =
                    new Vector3(pivot.transform.position.x,
                        pivot.transform.position.y,
                        pivot.transform.position.z + (HexAutoTiling.zTilingDistance / 2));
                pivotHasMoved = false;
            }
        }

        void restAllButtons()
        {
            leftAvailable = true; rightAvailable = true;
            topAvailable = true; bottomAvailable = true;
            inverseLB = true; inverseRT = true;
        }
        
        void moveToTheBottom()
        {
            nullcheck();
            foreach (GameObject hex in hasAllTheHexes)
            {
                if (hex.transform.position.z > pivot.transform.position.z)
                    hex.transform.position = new Vector3(hex.transform.position.x,
                        hex.transform.position.y, hex.transform.position.z - HexAutoTiling.zTilingDistance);
                if (!pivotHasMoved) pivotHasMoved = true;
            }
            if (pivotHasMoved)
            {
                pivot.transform.position =
                    new Vector3(pivot.transform.position.x,
                        pivot.transform.position.y,
                        pivot.transform.position.z - (HexAutoTiling.zTilingDistance / 2));
                pivotHasMoved = false;
            }
        }
         void moveToTheLeft()
        {
            nullcheck();
            foreach (GameObject hex in hasAllTheHexes)
            {
                if (hex.transform.position.x > pivot.transform.position.x)
                    hex.transform.position = new Vector3(hex.transform.position.x - HexAutoTiling.xTilingDistance,
                        hex.transform.position.y, hex.transform.position.z);
                if (!pivotHasMoved) pivotHasMoved = true;
            }

            if (pivotHasMoved)
            {
                pivot.transform.position =
                    new Vector3(pivot.transform.position.x - (HexAutoTiling.xTilingDistance / 2),
                        pivot.transform.position.y, pivot.transform.position.z);
                pivotHasMoved = false;
            }
        }
        void moveToTheRight()
        {
            nullcheck();
            foreach (GameObject hex in hasAllTheHexes)
            {

                if (hex.transform.position.x < pivot.transform.position.x)
                    hex.transform.position = new Vector3(hex.transform.position.x + HexAutoTiling.xTilingDistance,
                        hex.transform.position.y, hex.transform.position.z);
                if (!pivotHasMoved) pivotHasMoved = true;
            }
            if (pivotHasMoved)
            {
                pivot.transform.position = new Vector3(pivot.transform.position.x + (HexAutoTiling.xTilingDistance/2),
                    pivot.transform.position.y, pivot.transform.position.z);
                pivotHasMoved = false;
            }
        }
    }
}