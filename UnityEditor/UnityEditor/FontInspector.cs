using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Font))]
	internal class FontInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				UnityEngine.Object @object = targets[i];
				if (@object.hideFlags == HideFlags.NotEditable)
				{
					return;
				}
			}
			base.DrawDefaultInspector();
		}
	}
}
