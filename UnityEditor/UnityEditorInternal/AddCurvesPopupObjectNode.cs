using System;
using UnityEditor.IMGUI.Controls;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupObjectNode : TreeViewItem
	{
		public AddCurvesPopupObjectNode(TreeViewItem parent, string path, string className) : base((path + className).GetHashCode(), (parent == null) ? -1 : (parent.depth + 1), parent, className)
		{
		}
	}
}
