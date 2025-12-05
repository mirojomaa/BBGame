using UnityEditor;
using UnityEngine;
public class WaypointCreator : EditorWindow
{
#if UNITY_EDITOR
 [MenuItem("HaMiLeJa/Waypoint Creator")]
 private static void ShowWindow()
 {
  var window = GetWindow<WaypointCreator>();
  window.titleContent = new GUIContent("Waypoint Creator");
  window.Show();
 }
 public Transform WaypointParent;
 private void OnGUI()
 {
  var serializedEditorWindow = new SerializedObject(this);
  EditorGUILayout.PropertyField(serializedEditorWindow.FindProperty(nameof(WaypointParent)));
  if (!WaypointParent) EditorGUILayout.HelpBox("Zieh erstmal ein Pathfinding parent rein", MessageType.Info);
  else DrawButtons();
  serializedEditorWindow.ApplyModifiedProperties();
 }
 private void DrawButtons()
 {
  if (GUILayout.Button("Rename all Points")) RenameAllWaypoints();
  if (GUILayout.Button("Create Point")) CreateWaypoint();
  var selectedWaypoint = GetSelectedWaypoint();
  GUI.enabled = selectedWaypoint;
  var waypointName = selectedWaypoint ? $"{selectedWaypoint.name}" : string.Empty;
  if (GUILayout.Button($"[...] <- {waypointName}")) CreateWaypointBefore(selectedWaypoint);
  if (GUILayout.Button($"{waypointName} -> [...]")) CreateWaypointAfter(selectedWaypoint);
  if (GUILayout.Button($"Delete{waypointName}")) DeleteWaypoint(selectedWaypoint);
 }
private void CreateWaypointBefore(Waypoint selectedWaypoint)
  {
   var group = Undo.GetCurrentGroup();
   Undo.RegisterCreatedObjectUndo(selectedWaypoint, "Change Point");
   if (selectedWaypoint.PreviousPoint != null) Undo.RegisterCreatedObjectUndo(selectedWaypoint.PreviousPoint, "Change Point");
   var beforeWaypoint = CreateNewWayPoint();
   beforeWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
   beforeWaypoint.PreviousPoint = selectedWaypoint.PreviousPoint;
   if (beforeWaypoint.PreviousPoint) beforeWaypoint.PreviousPoint.NextPoint = beforeWaypoint;
   beforeWaypoint.NextPoint = selectedWaypoint;
   beforeWaypoint.NextPoint.PreviousPoint = beforeWaypoint;
   OrientWaypoint(beforeWaypoint, selectedWaypoint);
   Selection.activeGameObject = beforeWaypoint.gameObject;
   RenameAllWaypoints();
   Undo.SetCurrentGroupName($"Create Point before {selectedWaypoint.name}");
   Undo.CollapseUndoOperations(group);
  }
  private void CreateWaypointAfter(Waypoint selectedWaypoint)
  {
   var group = Undo.GetCurrentGroup();
   Undo.RegisterCreatedObjectUndo(selectedWaypoint, "Change Point");
   if (selectedWaypoint.NextPoint != null) Undo.RegisterCreatedObjectUndo(selectedWaypoint.NextPoint, "Change Point");
   var afterWaypoint = CreateNewWayPoint();
   afterWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex()+1);
   afterWaypoint.NextPoint = selectedWaypoint.NextPoint;
   if (afterWaypoint.NextPoint) afterWaypoint.NextPoint.PreviousPoint = afterWaypoint;
   afterWaypoint.PreviousPoint = selectedWaypoint;
   afterWaypoint.PreviousPoint.NextPoint = afterWaypoint;
   OrientWaypoint(afterWaypoint, selectedWaypoint);
   Selection.activeObject = afterWaypoint.gameObject;
   RenameAllWaypoints();
   Undo.SetCurrentGroupName($"Create Point before {selectedWaypoint.name}");
   Undo.CollapseUndoOperations(group);
  }
  private void DeleteWaypoint(Waypoint selectedWaypoint)
  {
   Undo.SetCurrentGroupName($"Delete Waypoint {selectedWaypoint.name}");
   if (selectedWaypoint.PreviousPoint)
   {
    Undo.RecordObject(selectedWaypoint.PreviousPoint, "Change prev Point");
    selectedWaypoint.PreviousPoint.NextPoint = selectedWaypoint.NextPoint;
   }
   if (selectedWaypoint.NextPoint)
   {
    Undo.RecordObject(selectedWaypoint.PreviousPoint, "Change next Point");
    selectedWaypoint.NextPoint.PreviousPoint = selectedWaypoint.PreviousPoint;
   }
   Undo.DestroyObjectImmediate(selectedWaypoint.gameObject);
   RenameAllWaypoints();
  }
  private Waypoint GetSelectedWaypoint()
 {
  return Selection.activeGameObject ? Selection.activeGameObject.GetComponent<Waypoint>() : null;
 }
 private void RenameAllWaypoints()
 {
  Undo.IncrementCurrentGroup();
  Undo.SetCurrentGroupName("Rename all Points");
  for (var i = 0; i < WaypointParent.childCount; i++)
  {
   var child = WaypointParent.GetChild(i);
   Undo.RecordObject(child.gameObject, "Rename Points");
   child.name = $"Point {i+1}";
  }
  WaypointParent.GetComponent<Pathfinder>().AllChildsToList();
 }
 private void CreateWaypoint()
 {
  var waypoint = CreateNewWayPoint();
  waypoint.transform.parent.GetComponent<Pathfinder>().AllChildsToList();
  if (WaypointParent.childCount > 1)
  {
   waypoint.PreviousPoint =
    WaypointParent.GetChild(waypoint.transform.GetSiblingIndex() - 1).GetComponent<Waypoint>();
   waypoint.PreviousPoint.NextPoint = waypoint;
   OrientWaypoint(waypoint, waypoint.PreviousPoint);
  }
  Selection.activeObject = waypoint.gameObject;
 }
 private void OrientWaypoint(Waypoint waypoint, Waypoint reference)
 {
  if (!reference) return;
  var waypointTransform = waypoint.transform;
  var referenceTransform = reference.transform;
  Undo.IncrementCurrentGroup();
  Undo.RecordObject(waypointTransform, $"Change orientation and position of {waypoint}");
  waypointTransform.SetPositionAndRotation(referenceTransform.position, referenceTransform.rotation);
 }
 private Waypoint CreateNewWayPoint()
 {
  var waypointGameObject = new GameObject(
   $"Point {WaypointParent.childCount +1}",
   typeof(Waypoint));
  waypointGameObject.transform.SetParent(WaypointParent.transform, false);
  Undo.RegisterCreatedObjectUndo(waypointGameObject, "Created new Waypoint");
  return waypointGameObject.GetComponent<Waypoint>();
 }
#endif
}
