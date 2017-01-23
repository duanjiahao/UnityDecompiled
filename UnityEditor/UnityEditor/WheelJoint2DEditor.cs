using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(WheelJoint2D))]
	internal class WheelJoint2DEditor : AnchoredJoint2DEditor
	{
		public new void OnSceneGUI()
		{
			WheelJoint2D wheelJoint2D = (WheelJoint2D)base.target;
			if (wheelJoint2D.enabled)
			{
				Vector3 vector = Joint2DEditor.TransformPoint(wheelJoint2D.transform, wheelJoint2D.anchor);
				Vector3 vector2 = vector;
				Vector3 vector3 = vector;
				Vector3 vector4 = Joint2DEditor.RotateVector2(Vector3.right, -wheelJoint2D.suspension.angle - wheelJoint2D.transform.eulerAngles.z);
				Handles.color = Color.green;
				vector4 *= HandleUtility.GetHandleSize(vector) * 0.3f;
				vector2 += vector4;
				vector3 -= vector4;
				Joint2DEditor.DrawAALine(vector2, vector3);
				base.OnSceneGUI();
			}
		}
	}
}
