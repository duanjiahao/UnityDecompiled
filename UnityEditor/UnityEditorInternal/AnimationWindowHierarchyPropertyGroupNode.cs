using System;
using UnityEditor;
namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyPropertyGroupNode : AnimationWindowHierarchyNode
	{
		public AnimationWindowHierarchyPropertyGroupNode(Type animatableObjectType, string propertyName, string path, TreeViewItem parent) : base(AnimationWindowUtility.GetPropertyNodeID(path, animatableObjectType, propertyName), (parent == null) ? -1 : (parent.depth + 1), parent, animatableObjectType, AnimationWindowUtility.GetPropertyGroupName(propertyName), path, AnimationWindowUtility.GetNicePropertyGroupDisplayName(animatableObjectType, propertyName))
		{
		}
	}
}
