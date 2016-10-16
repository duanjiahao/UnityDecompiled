using System;
using UnityEditor;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupPropertyNode : TreeViewItem
	{
		public EditorCurveBinding[] curveBindings;

		public AddCurvesPopupPropertyNode(TreeViewItem parent, EditorCurveBinding[] curveBindings) : base(curveBindings[0].GetHashCode(), parent.depth + 1, parent, AnimationWindowUtility.NicifyPropertyGroupName(curveBindings[0].type, AnimationWindowUtility.GetPropertyGroupName(curveBindings[0].propertyName)))
		{
			this.curveBindings = curveBindings;
		}

		public override int CompareTo(TreeViewItem other)
		{
			if (other is AddCurvesPopupPropertyNode)
			{
				if (this.displayName.Contains("Rotation") && other.displayName.Contains("Position"))
				{
					return 1;
				}
				if (this.displayName.Contains("Position") && other.displayName.Contains("Rotation"))
				{
					return -1;
				}
			}
			return base.CompareTo(other);
		}
	}
}
