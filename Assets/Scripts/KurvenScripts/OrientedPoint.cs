using UnityEngine;
public struct OrientedPoint 
{
	public Vector3 pos;
	public Quaternion rot;
	public OrientedPoint( Vector3 pos, Quaternion rot ) 
	{
		this.pos = pos;
		this.rot = rot;
	}
	public Vector3 LocalToWorldPos( Vector3 localSpacePos ) =>pos + rot * localSpacePos;
	public Vector3 LocalToWorldVec( Vector3 localSpacePos ) => rot * localSpacePos;
}
