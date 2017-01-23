using System;
using System.Collections.Generic;

namespace UnityEditor.IMGUI.Controls
{
	internal class TreeViewItemAlphaNumericSort : IComparer<TreeViewItem>
	{
		public int Compare(TreeViewItem lhs, TreeViewItem rhs)
		{
			int result;
			if (lhs == rhs)
			{
				result = 0;
			}
			else if (lhs == null)
			{
				result = -1;
			}
			else if (rhs == null)
			{
				result = 1;
			}
			else
			{
				result = EditorUtility.NaturalCompare(lhs.displayName, rhs.displayName);
			}
			return result;
		}
	}
}
