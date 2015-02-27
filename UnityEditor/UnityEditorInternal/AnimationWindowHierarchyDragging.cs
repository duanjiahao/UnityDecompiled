using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyDragging : ITreeViewDragging
	{
		public bool drawRowMarkerAbove
		{
			get;
			set;
		}
		public void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
		}
		public bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem)
		{
			return false;
		}
		public void DragCleanup(bool revertExpanded)
		{
		}
		public int GetDropTargetControlID()
		{
			return 0;
		}
		public int GetRowMarkerControlID()
		{
			return 0;
		}
	}
}
