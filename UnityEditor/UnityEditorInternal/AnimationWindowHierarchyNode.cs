using System;
using UnityEditor;

namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyNode : TreeViewItem
	{
		public string path;

		public Type animatableObjectType;

		public string propertyName;

		public EditorCurveBinding? binding;

		public AnimationWindowCurve[] curves;

		public float? topPixel;

		public int indent;

		public AnimationWindowHierarchyNode(int instanceID, int depth, TreeViewItem parent, Type animatableObjectType, string propertyName, string path, string displayName) : base(instanceID, depth, parent, displayName)
		{
			this.displayName = displayName;
			this.animatableObjectType = animatableObjectType;
			this.propertyName = propertyName;
			this.path = path;
		}
	}
}
