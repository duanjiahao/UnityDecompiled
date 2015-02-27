using System;
using UnityEditor;
namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyPropertyNode : AnimationWindowHierarchyNode
	{
		public bool isPptrNode;
		public AnimationWindowHierarchyPropertyNode(Type animatableObjectType, string propertyName, string path, TreeViewItem parent, EditorCurveBinding binding, bool isPptrNode) : base(AnimationWindowUtility.GetPropertyNodeID(path, animatableObjectType, propertyName), (parent == null) ? -1 : (parent.depth + 1), parent, animatableObjectType, propertyName, path, AnimationWindowUtility.GetNicePropertyDisplayName(animatableObjectType, propertyName))
		{
			this.binding = new EditorCurveBinding?(binding);
			this.isPptrNode = isPptrNode;
		}
	}
}
