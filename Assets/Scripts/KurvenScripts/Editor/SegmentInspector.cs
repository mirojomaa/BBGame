using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
// Hier wird ein Custom Inspector generiert mit extra handles für die länge der tangente von einer Segmentkurve
[CustomEditor(typeof(Segment))]
public class  SegmentInspector : Editor 
{
	const string UNDO_STR_ADJUST = "bezier tangent";
	void OnSceneGUI() 
	{
		// Generiert IDs für die UI Controls
		int arrowIDFrw  = GUIUtility.GetControlID( "Arrow vorne".GetHashCode(),  FocusType.Passive ),
		    arrowIDBack = GUIUtility.GetControlID( "Arrow hinten".GetHashCode(), FocusType.Passive );
		// Kriegt daten für die tangent location, direction und origin
		Segment road = target as Segment;
		Vector3 origin = road.GetControlPoint( 0, Space.World ),
			    tangentForward = road.GetControlPoint( 1, Space.World ),
		        tangentBack = origin * 2 - tangentForward, // Mirror tangent point around origin
		         tangentDir = road.transform.forward;
		// Berechnen einer Ebene, gegen die der Mausstrahl beim Ziehen projiziert wird
		Vector3 camUp = SceneView.lastActiveSceneView.camera.transform.up,
			  pNormal = Vector3.Cross( tangentDir, camUp ).normalized;
		Plane draggingPlane = new Plane( pNormal, origin );
		float newDistance = 0;
		bool TangentHandle( int id, Vector3 handlePos, Vector3 direction ) => DrawTangentHandle( id, handlePos, origin, direction, draggingPlane, ref newDistance );
		
		//  Ziehen von Griffen und Ändern der Tangente, wenn Sie sie gezogen haben
		bool changebackForward  = TangentHandle( arrowIDFrw, tangentForward,tangentDir ),
			  changedBack = TangentHandle( arrowIDBack, tangentBack, -tangentDir );

		// Wenn einer der beiden geändert wurde, weisen Sie den neuen Abstand der Tangentenlänge zu!
		if( changebackForward || changedBack ) 
		{
			Undo.RecordObject( target, UNDO_STR_ADJUST );
			road.tangentLength = newDistance;
			road.ChainParent?.UpdateMeshes();
		}
	}
	bool DrawTangentHandle( int id, Vector3 handlePos, Vector3 origin, Vector3 direction, Plane draggingPlane, ref float newDistance ) 
	{
		bool wasChanged = false;
		float size = HandleUtility.GetHandleSize( handlePos ), // Für screen-relative size
		      handleRadius = size * 0.15f,
		      cursorDistancePx = HandleUtility.DistanceToCircle( handlePos, handleRadius * 0.5f );
		// Input states
		Event e = Event.current;
		bool leftMouseButtonDown = e.button == 0,
			 isDraggingThis = GUIUtility.hotControl == id && leftMouseButtonDown,
		     isHoveringThis = HandleUtility.nearestControl == id;
		// DrawTangentHandle wird einmal pro Ereignistyp ausgeführt
		switch( e.type ) 
		{
			case EventType.Layout:
				// Layout wird sehr früh aufgerufen, und hier werden interaktive Steuerelemente eingerichtet
				HandleUtility.AddControl( id, cursorDistancePx ); 
				break;
			case EventType.MouseDown:
				// Focus this control if we clicked it
				if( isHoveringThis && leftMouseButtonDown ) {
					GUIUtility.hotControl = id;
					GUIUtility.keyboardControl = id;
					e.Use();
				}
				break;
			case EventType.MouseDrag:
				if( isDraggingThis ) 
				{
					// Raycast einer plane entlang der Achse
					Ray r = HandleUtility.GUIPointToWorldRay( e.mousePosition );
					if( draggingPlane.Raycast( r, out float dist ) ) 
					{
						Vector3 intersectionPt = r.GetPoint( dist );
						// Projizieren Sie nun den Schnittpunkt in der Ebene auf die Tangentenvektorlinie
						// Dadurch wird auch sichergestellt, dass der Punkt nicht unter 0,5 Meter fällt.
						// Negative Werte werden wild und falsch *nicht tun*
						float projectedDistance = Vector3.Dot( intersectionPt - origin, direction ).AtLeast(0.5f);
						newDistance = projectedDistance;
						wasChanged = true;
					}
					e.Use();
				}
				break; 
			case EventType.MouseUp:
				if( isDraggingThis ) // Defocus control beim realese
				{
					GUIUtility.hotControl = 0; e.Use();
				}
				break;
			case EventType.Repaint:
				// alles zeichnen// Set color/ drag und click
				Color color = GetHandleColor( isHoveringThis, isDraggingThis );
				using( new TemporaryHandleColor( color ) ) {
					Handles.DrawAAPolyLine( origin, handlePos );
					Quaternion rot = Quaternion.LookRotation( direction );
					Handles.SphereHandleCap( id, handlePos, rot, handleRadius, EventType.Repaint );
				}
				break;
		}
		return wasChanged;
	}
	// verschiedene Farben des Griffs basierend auf Hover-Status
	Color GetHandleColor( bool hovering, bool dragging )
	{
		if( dragging ) return Color.magenta;
		 if( hovering ) return Color.Lerp( Color.red, Color.white, 0.6f );
		return Handles.yAxisColor;
	}
	// Sauberere Handhabung der temporären Farbe
	class TemporaryHandleColor : IDisposable 
	{
		static Stack<Color> colorStack = new Stack<Color>();
		public TemporaryHandleColor( Color color ) 
		{
			colorStack.Push( Handles.color );
			Handles.color = color;
		}
		public void Dispose() => Handles.color = colorStack.Pop();
	}
}