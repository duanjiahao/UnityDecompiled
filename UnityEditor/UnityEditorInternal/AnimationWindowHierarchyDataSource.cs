using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyDataSource : TreeViewDataSource
	{
		private AnimationWindowState state
		{
			get;
			set;
		}
		public bool showAll
		{
			get;
			set;
		}
		public AnimationWindowHierarchyDataSource(TreeView treeView, AnimationWindowState animationWindowState) : base(treeView)
		{
			this.state = animationWindowState;
		}
		private void SetupRootNodeSettings()
		{
			base.showRootNode = false;
			base.rootIsCollapsable = false;
			this.SetExpanded(this.m_RootItem, true);
		}
		private AnimationWindowHierarchyNode GetEmptyRootNode()
		{
			return new AnimationWindowHierarchyNode(0, -1, null, null, string.Empty, string.Empty, "root");
		}
		public override void FetchData()
		{
			this.m_RootItem = this.GetEmptyRootNode();
			this.SetupRootNodeSettings();
			if (this.state.m_ActiveGameObject == null || this.state.m_RootGameObject == null)
			{
				return;
			}
			List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
			if (this.state.allCurves.Count > 0)
			{
				list.Add(new AnimationWindowHierarchyMasterNode());
			}
			list.AddRange(this.CreateTreeFromCurves());
			list.Add(new AnimationWindowHierarchyAddButtonNode());
			TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), this.root);
			this.m_NeedRefreshVisibleFolders = true;
		}
		public override bool IsRenamingItemAllowed(TreeViewItem item)
		{
			return !(item is AnimationWindowHierarchyAddButtonNode) && !(item is AnimationWindowHierarchyMasterNode) && (item as AnimationWindowHierarchyNode).path.Length != 0;
		}
		public List<AnimationWindowHierarchyNode> CreateTreeFromCurves()
		{
			List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
			List<AnimationWindowCurve> list2 = new List<AnimationWindowCurve>();
			AnimationWindowCurve[] array = this.state.allCurves.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = array[i];
				AnimationWindowCurve animationWindowCurve2 = (i >= array.Length - 1) ? null : array[i + 1];
				list2.Add(animationWindowCurve);
				bool flag = animationWindowCurve2 != null && AnimationWindowUtility.GetPropertyGroupName(animationWindowCurve2.propertyName) == AnimationWindowUtility.GetPropertyGroupName(animationWindowCurve.propertyName);
				bool flag2 = animationWindowCurve2 != null && animationWindowCurve.path.Equals(animationWindowCurve2.path) && animationWindowCurve.type == animationWindowCurve2.type;
				if (i == array.Length - 1 || !flag || !flag2)
				{
					if (list2.Count > 1)
					{
						list.Add(this.AddPropertyGroupToHierarchy(list2.ToArray(), (AnimationWindowHierarchyNode)this.m_RootItem));
					}
					else
					{
						list.Add(this.AddPropertyToHierarchy(list2[0], (AnimationWindowHierarchyNode)this.m_RootItem));
					}
					list2.Clear();
				}
			}
			return list;
		}
		private AnimationWindowHierarchyPropertyGroupNode AddPropertyGroupToHierarchy(AnimationWindowCurve[] curves, AnimationWindowHierarchyNode parentNode)
		{
			List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
			Type type = curves[0].type;
			AnimationWindowHierarchyPropertyGroupNode animationWindowHierarchyPropertyGroupNode = new AnimationWindowHierarchyPropertyGroupNode(type, AnimationWindowUtility.GetPropertyGroupName(curves[0].propertyName), curves[0].path, parentNode);
			animationWindowHierarchyPropertyGroupNode.icon = this.GetIcon(curves[0].binding);
			animationWindowHierarchyPropertyGroupNode.indent = curves[0].depth;
			animationWindowHierarchyPropertyGroupNode.curves = curves;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve curve = curves[i];
				AnimationWindowHierarchyPropertyNode animationWindowHierarchyPropertyNode = this.AddPropertyToHierarchy(curve, animationWindowHierarchyPropertyGroupNode);
				animationWindowHierarchyPropertyNode.displayName = AnimationWindowUtility.GetPropertyDisplayName(animationWindowHierarchyPropertyNode.propertyName);
				list.Add(animationWindowHierarchyPropertyNode);
			}
			TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), animationWindowHierarchyPropertyGroupNode);
			return animationWindowHierarchyPropertyGroupNode;
		}
		private AnimationWindowHierarchyPropertyNode AddPropertyToHierarchy(AnimationWindowCurve curve, AnimationWindowHierarchyNode parentNode)
		{
			AnimationWindowHierarchyPropertyNode animationWindowHierarchyPropertyNode = new AnimationWindowHierarchyPropertyNode(curve.type, curve.propertyName, curve.path, parentNode, curve.binding, curve.isPPtrCurve);
			if (parentNode.icon != null)
			{
				animationWindowHierarchyPropertyNode.icon = parentNode.icon;
			}
			else
			{
				animationWindowHierarchyPropertyNode.icon = this.GetIcon(curve.binding);
			}
			animationWindowHierarchyPropertyNode.indent = curve.depth;
			animationWindowHierarchyPropertyNode.curves = new AnimationWindowCurve[]
			{
				curve
			};
			return animationWindowHierarchyPropertyNode;
		}
		public Texture2D GetIcon(EditorCurveBinding curveBinding)
		{
			if (this.state.m_RootGameObject != null)
			{
				UnityEngine.Object animatedObject = AnimationUtility.GetAnimatedObject(this.state.m_RootGameObject, curveBinding);
				if (animatedObject != null)
				{
					return AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(this.state.m_RootGameObject, curveBinding));
				}
			}
			return AssetPreview.GetMiniTypeThumbnail(curveBinding.type);
		}
		public void UpdateData()
		{
			this.m_TreeView.ReloadData();
		}
	}
}
