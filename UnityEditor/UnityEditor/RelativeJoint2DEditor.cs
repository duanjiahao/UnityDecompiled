using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(RelativeJoint2D))]
	internal class RelativeJoint2DEditor : Joint2DEditor
	{
		public void OnSceneGUI()
		{
			RelativeJoint2D relativeJoint2D = (RelativeJoint2D)base.target;
			if (relativeJoint2D.enabled)
			{
				Vector3 vector = relativeJoint2D.target;
				Vector3 vector2 = (!relativeJoint2D.connectedBody) ? Vector3.zero : relativeJoint2D.connectedBody.transform.position;
				Handles.color = Color.green;
				Joint2DEditor.DrawAALine(vector, vector2);
				float d = HandleUtility.GetHandleSize(vector2) * 0.16f;
				Vector3 b = Vector3.left * d;
				Vector3 b2 = Vector3.up * d;
				Joint2DEditor.DrawAALine(vector2 - b, vector2 + b);
				Joint2DEditor.DrawAALine(vector2 - b2, vector2 + b2);
				float d2 = HandleUtility.GetHandleSize(vector) * 0.16f;
				Vector3 b3 = Vector3.left * d2;
				Vector3 b4 = Vector3.up * d2;
				Joint2DEditor.DrawAALine(vector - b3, vector + b3);
				Joint2DEditor.DrawAALine(vector - b4, vector + b4);
			}
		}
	}
}
