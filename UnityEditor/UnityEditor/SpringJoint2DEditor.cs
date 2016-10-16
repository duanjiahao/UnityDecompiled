using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SpringJoint2D))]
	internal class SpringJoint2DEditor : AnchoredJoint2DEditor
	{
		public new void OnSceneGUI()
		{
			SpringJoint2D springJoint2D = (SpringJoint2D)this.target;
			if (!springJoint2D.enabled)
			{
				return;
			}
			Vector3 anchor = Joint2DEditor.TransformPoint(springJoint2D.transform, springJoint2D.anchor);
			Vector3 vector = springJoint2D.connectedAnchor;
			if (springJoint2D.connectedBody)
			{
				vector = Joint2DEditor.TransformPoint(springJoint2D.connectedBody.transform, vector);
			}
			Joint2DEditor.DrawDistanceGizmo(anchor, vector, springJoint2D.distance);
			base.OnSceneGUI();
		}
	}
}
