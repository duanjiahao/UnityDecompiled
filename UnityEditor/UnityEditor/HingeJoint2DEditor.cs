using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(HingeJoint2D))]
	internal class HingeJoint2DEditor : AnchoredJoint2DEditor
	{
		public new void OnSceneGUI()
		{
			HingeJoint2D hingeJoint2D = (HingeJoint2D)this.target;
			if (!hingeJoint2D.enabled)
			{
				return;
			}
			if (hingeJoint2D.useLimits)
			{
				Vector3 vector = Joint2DEditor.TransformPoint(hingeJoint2D.transform, hingeJoint2D.anchor);
				float num = Mathf.Min(hingeJoint2D.limits.min, hingeJoint2D.limits.max);
				float num2 = Mathf.Max(hingeJoint2D.limits.min, hingeJoint2D.limits.max);
				float angle = num2 - num;
				float num3 = HandleUtility.GetHandleSize(vector) * 0.8f;
				float rotation = hingeJoint2D.GetComponent<Rigidbody2D>().rotation;
				Vector3 vector2 = Joint2DEditor.RotateVector2(Vector3.right, -num2 - rotation);
				Vector3 end = vector + Joint2DEditor.RotateVector2(Vector3.right, -hingeJoint2D.jointAngle - rotation) * num3;
				Handles.color = new Color(0f, 1f, 0f, 0.7f);
				Joint2DEditor.DrawAALine(vector, end);
				Handles.color = new Color(0f, 1f, 0f, 0.03f);
				Handles.DrawSolidArc(vector, Vector3.back, vector2, angle, num3);
				Handles.color = new Color(0f, 1f, 0f, 0.7f);
				Handles.DrawWireArc(vector, Vector3.back, vector2, angle, num3);
				this.DrawTick(vector, num3, 0f, vector2, 1f);
				this.DrawTick(vector, num3, angle, vector2, 1f);
			}
			base.OnSceneGUI();
		}

		private void DrawTick(Vector3 center, float radius, float angle, Vector3 up, float length)
		{
			Vector3 a = Joint2DEditor.RotateVector2(up, angle).normalized;
			Vector3 vector = center + a * radius;
			Vector3 vector2 = vector + (center - vector).normalized * radius * length;
			Handles.DrawAAPolyLine(new Color[]
			{
				new Color(0f, 1f, 0f, 0.7f),
				new Color(0f, 1f, 0f, 0f)
			}, new Vector3[]
			{
				vector,
				vector2
			});
		}
	}
}
