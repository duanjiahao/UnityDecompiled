using System;
using UnityEditor;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupObjectNode : TreeViewItem
	{
		public AddCurvesPopupObjectNode(TreeViewItem parent, string path, string className) : base((path + className).GetHashCode(), parent.depth + 1, parent, className)
		{
		}
	}
}
