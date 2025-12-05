using UnityEditor;
using UnityEngine;
public class WayPointGizmos
{
    private static readonly float Width = 3f, Halfwidth = Width / 2, Quarterwidth = Width / 3,
                                  ArrowHeadAngle = 20, ArrowLength = 5f;
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable, typeof(Waypoint))]
    public static void DrawSceneGizmos(Waypoint waypoint, GizmoType gizmoType)
    {
        if (!CurveManager.drawPathfindingInEditMode) return;
        DrawStartPoint(waypoint);
        DrawEndPoint(waypoint);
        DrawMidPoint(waypoint);
        DrawWalkLine(waypoint);
        DrawOrientation(waypoint);
    }

    private static void DrawWalkLine(Waypoint waypoint)
    {
        if (!waypoint.NextPoint) return;
        Gizmos.color = Color.white;
        var waypointPosition = waypoint.transform.position;
        var nextWaypointPoition = waypoint.NextPoint.transform.position;
        var direction = (nextWaypointPoition - waypointPosition).normalized;
        if (direction == Vector3.zero) return;
        var lookRotation = Quaternion.LookRotation(direction, Vector3.forward);
        var rightArrowHead = lookRotation * Quaternion.Euler(0, 180 + ArrowHeadAngle, -60) * Vector3.forward;
        var leftArrowHead = lookRotation * Quaternion.Euler(0, 180 - ArrowHeadAngle, -60) * Vector3.forward;
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(nextWaypointPoition,rightArrowHead *ArrowLength);
        Gizmos.DrawRay(nextWaypointPoition,leftArrowHead *ArrowLength);
    }
    private static void DrawStartPoint(Waypoint waypoint)
    {
        if (waypoint.PreviousPoint) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(waypoint.transform.position,Halfwidth);
    }
    private static void DrawEndPoint(Waypoint waypoint)
    {
        if (waypoint.NextPoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(waypoint.transform.position,Halfwidth);
    }
    private static void DrawMidPoint(Waypoint waypoint)
    {
        if (!waypoint.NextPoint || !waypoint.PreviousPoint) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(waypoint.transform.position,Quarterwidth);
    }
    private static void DrawOrientation(Waypoint waypoint)
    {
        Gizmos.color = Color.white;
        var transform = waypoint.transform;
        var offset = transform.right * Halfwidth;
        var startPosition = transform.position;
        Gizmos.DrawLine(startPosition - offset, startPosition + offset);
    }
}