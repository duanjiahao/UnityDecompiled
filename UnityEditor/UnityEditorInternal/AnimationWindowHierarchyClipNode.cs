using System;
using UnityEditor;

namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyClipNode : AnimationWindowHierarchyNode
	{
		public AnimationWindowHierarchyClipNode(TreeViewItem parent, int setId, string name) : base(setId, (parent == null) ? -1 : (parent.depth + 1), parent, null, null, null, name)
		{
		}
	}
}
