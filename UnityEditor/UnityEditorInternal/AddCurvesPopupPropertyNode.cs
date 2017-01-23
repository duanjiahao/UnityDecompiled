using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupPropertyNode : TreeViewItem
	{
		public AnimationWindowSelectionItem selectionItem;

		public EditorCurveBinding[] curveBindings;

		public AddCurvesPopupPropertyNode(TreeViewItem parent, AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] curveBindings) : base(curveBindings[0].GetHashCode(), parent.depth + 1, parent, AnimationWindowUtility.NicifyPropertyGroupName(curveBindings[0].type, AnimationWindowUtility.GetPropertyGroupName(curveBindings[0].propertyName)))
		{
			this.selectionItem = selectionItem;
			this.curveBindings = curveBindings;
		}

		public override int CompareTo(TreeViewItem other)
		{
			AddCurvesPopupPropertyNode addCurvesPopupPropertyNode = other as AddCurvesPopupPropertyNode;
			int result;
			if (addCurvesPopupPropertyNode != null)
			{
				if (this.displayName.Contains("Rotation") && addCurvesPopupPropertyNode.displayName.Contains("Position"))
				{
					result = 1;
					return result;
				}
				if (this.displayName.Contains("Position") && addCurvesPopupPropertyNode.displayName.Contains("Rotation"))
				{
					result = -1;
					return result;
				}
			}
			result = base.CompareTo(other);
			return result;
		}
	}
}
