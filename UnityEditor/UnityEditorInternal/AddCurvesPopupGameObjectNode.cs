using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupGameObjectNode : TreeViewItem
	{
		public AddCurvesPopupGameObjectNode(GameObject gameObject, TreeViewItem parent, string displayName) : base(gameObject.GetInstanceID(), (parent == null) ? -1 : (parent.depth + 1), parent, displayName)
		{
		}
	}
}
