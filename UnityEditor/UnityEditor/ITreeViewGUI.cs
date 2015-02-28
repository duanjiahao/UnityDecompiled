using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	internal interface ITreeViewGUI
	{
		float halfDropBetweenHeight
		{
			get;
		}
		float topRowMargin
		{
			get;
		}
		float bottomRowMargin
		{
			get;
		}
		Vector2 GetTotalSize(List<TreeViewItem> rows);
		void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible);
		float GetTopPixelOfRow(int row, List<TreeViewItem> rows);
		float GetHeightOfLastRow();
		int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView);
		Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused);
		void BeginRowGUI();
		void EndRowGUI();
		void BeginPingNode(TreeViewItem item, float topPixelOfRow, float availableWidth);
		void EndPingNode();
		bool BeginRename(TreeViewItem item, float delay);
		void EndRename();
		float GetContentIndent(TreeViewItem item);
	}
}
