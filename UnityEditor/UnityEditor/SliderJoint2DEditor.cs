using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SliderJoint2D))]
	internal class SliderJoint2DEditor : AnchoredJoint2DEditor
	{
		public new void OnSceneGUI()
		{
			SliderJoint2D sliderJoint2D = (SliderJoint2D)this.target;
			if (!sliderJoint2D.enabled)
			{
				return;
			}
			Vector3 vector = Joint2DEditor.TransformPoint(sliderJoint2D.transform, sliderJoint2D.anchor);
			Vector3 vector2 = vector;
			Vector3 vector3 = vector;
			Vector3 vector4 = Joint2DEditor.RotateVector2(Vector3.right, -sliderJoint2D.angle - sliderJoint2D.transform.eulerAngles.z);
			Handles.color = Color.green;
			if (sliderJoint2D.useLimits)
			{
				vector2 = vector + vector4 * sliderJoint2D.limits.max;
				vector3 = vector + vector4 * sliderJoint2D.limits.min;
				Vector3 a = Vector3.Cross(vector4, Vector3.forward);
				float d = HandleUtility.GetHandleSize(vector2) * 0.16f;
				float d2 = HandleUtility.GetHandleSize(vector3) * 0.16f;
				Joint2DEditor.DrawAALine(vector2 + a * d, vector2 - a * d);
				Joint2DEditor.DrawAALine(vector3 + a * d2, vector3 - a * d2);
			}
			else
			{
				vector4 *= HandleUtility.GetHandleSize(vector) * 0.3f;
				vector2 += vector4;
				vector3 -= vector4;
			}
			Joint2DEditor.DrawAALine(vector2, vector3);
			base.OnSceneGUI();
		}
	}
}
