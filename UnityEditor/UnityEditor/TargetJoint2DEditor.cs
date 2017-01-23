using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(TargetJoint2D))]
	internal class TargetJoint2DEditor : Joint2DEditor
	{
		public void OnSceneGUI()
		{
			TargetJoint2D targetJoint2D = (TargetJoint2D)base.target;
			if (targetJoint2D.enabled)
			{
				Vector3 vector = Joint2DEditor.TransformPoint(targetJoint2D.transform, targetJoint2D.anchor);
				Vector3 vector2 = targetJoint2D.target;
				Handles.color = Color.green;
				Handles.DrawDottedLine(vector, vector2, 5f);
				if (base.HandleAnchor(ref vector, false))
				{
					Undo.RecordObject(targetJoint2D, "Move Anchor");
					targetJoint2D.anchor = Joint2DEditor.InverseTransformPoint(targetJoint2D.transform, vector);
				}
				float d = HandleUtility.GetHandleSize(vector2) * 0.3f;
				Vector3 b = Vector3.left * d;
				Vector3 b2 = Vector3.up * d;
				Joint2DEditor.DrawAALine(vector2 - b, vector2 + b);
				Joint2DEditor.DrawAALine(vector2 - b2, vector2 + b2);
				if (base.HandleAnchor(ref vector2, true))
				{
					Undo.RecordObject(targetJoint2D, "Move Target");
					targetJoint2D.target = vector2;
				}
			}
		}
	}
}
