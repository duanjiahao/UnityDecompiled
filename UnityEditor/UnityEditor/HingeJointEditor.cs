using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(HingeJoint))]
	internal class HingeJointEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();
			string text = string.Empty;
			JointLimits limits = ((HingeJoint)this.target).limits;
			if (limits.min < -180f || limits.min > 180f)
			{
				text += "Min Limit needs to be within [-180,180].";
			}
			if (limits.max < -180f || limits.max > 180f)
			{
				text = text + ((!string.IsNullOrEmpty(text)) ? "\n" : string.Empty) + "Max Limit needs to be within [-180,180].";
			}
			if (limits.max < limits.min)
			{
				text = text + ((!string.IsNullOrEmpty(text)) ? "\n" : string.Empty) + "Max Limit needs to be larger or equal to the Min Limit.";
			}
			if (!string.IsNullOrEmpty(text))
			{
				EditorGUILayout.HelpBox(text, MessageType.Warning);
			}
		}
	}
}
