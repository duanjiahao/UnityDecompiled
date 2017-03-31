using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	internal class TreeViewStateWithColumns : TreeViewState
	{
		[SerializeField]
		public float[] columnWidths;
	}
}
