using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(DistanceJoint2D))]
	internal class DistanceJoint2DEditor : AnchoredJoint2DEditor
	{
		public new void OnSceneGUI()
		{
			DistanceJoint2D distanceJoint2D = (DistanceJoint2D)this.target;
			if (!distanceJoint2D.enabled)
			{
				return;
			}
			Vector3 anchor = Joint2DEditor.TransformPoint(distanceJoint2D.transform, distanceJoint2D.anchor);
			Vector3 vector = distanceJoint2D.connectedAnchor;
			if (distanceJoint2D.connectedBody)
			{
				vector = Joint2DEditor.TransformPoint(distanceJoint2D.connectedBody.transform, vector);
			}
			Joint2DEditor.DrawDistanceGizmo(anchor, vector, distanceJoint2D.distance);
			base.OnSceneGUI();
		}
	}
}
