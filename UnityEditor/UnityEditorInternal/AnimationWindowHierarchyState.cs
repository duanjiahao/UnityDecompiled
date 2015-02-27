using System;
using System.Collections.Generic;
using UnityEditor;
namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimationWindowHierarchyState : TreeViewState
	{
		public List<int> m_TallInstanceIDs = new List<int>();
		public bool getTallMode(AnimationWindowHierarchyNode node)
		{
			return this.m_TallInstanceIDs.Contains(node.id);
		}
		public void setTallMode(AnimationWindowHierarchyNode node, bool tallMode)
		{
			if (tallMode)
			{
				this.m_TallInstanceIDs.Add(node.id);
			}
			else
			{
				this.m_TallInstanceIDs.Remove(node.id);
			}
		}
	}
}
