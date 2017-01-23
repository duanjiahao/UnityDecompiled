using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyNode : TreeViewItem
	{
		public string path;

		public Type animatableObjectType;

		public string propertyName;

		public EditorCurveBinding? binding;

		public AnimationWindowCurve[] curves;

		public float? topPixel = null;

		public int indent = 0;

		public AnimationWindowHierarchyNode(int instanceID, int depth, TreeViewItem parent, Type animatableObjectType, string propertyName, string path, string displayName) : base(instanceID, depth, parent, displayName)
		{
			this.displayName = displayName;
			this.animatableObjectType = animatableObjectType;
			this.propertyName = propertyName;
			this.path = path;
		}
	}
}
