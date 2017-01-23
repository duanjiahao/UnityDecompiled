using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
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

		public AnimationWindowHierarchyDataSource(TreeViewController treeView, AnimationWindowState animationWindowState) : base(treeView)
		{
			this.state = animationWindowState;
		}

		private void SetupRootNodeSettings()
		{
			base.showRootItem = false;
			base.rootIsCollapsable = false;
			this.SetExpanded(this.m_RootItem, true);
		}

		private AnimationWindowHierarchyNode GetEmptyRootNode()
		{
			return new AnimationWindowHierarchyNode(0, -1, null, null, "", "", "root");
		}

		public override void FetchData()
		{
			this.m_RootItem = this.GetEmptyRootNode();
			this.SetupRootNodeSettings();
			this.m_NeedRefreshRows = true;
			if (this.state.selection.disabled)
			{
				base.root.children = null;
			}
			else
			{
				List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
				if (this.state.allCurves.Count > 0)
				{
					list.Add(new AnimationWindowHierarchyMasterNode
					{
						curves = this.state.allCurves.ToArray()
					});
				}
				list.AddRange(this.CreateTreeFromCurves());
				list.Add(new AnimationWindowHierarchyAddButtonNode());
				TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), base.root);
			}
		}

		public override bool IsRenamingItemAllowed(TreeViewItem item)
		{
			return !(item is AnimationWindowHierarchyAddButtonNode) && !(item is AnimationWindowHierarchyMasterNode) && !(item is AnimationWindowHierarchyClipNode) && (item as AnimationWindowHierarchyNode).path.Length != 0;
		}

		public List<AnimationWindowHierarchyNode> CreateTreeFromCurves()
		{
			List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
			List<AnimationWindowCurve> list2 = new List<AnimationWindowCurve>();
			AnimationWindowSelectionItem[] array = this.state.selection.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				AnimationWindowSelectionItem animationWindowSelectionItem = array[i];
				AnimationWindowCurve[] array2 = animationWindowSelectionItem.curves.ToArray();
				AnimationWindowHierarchyNode parentNode = (AnimationWindowHierarchyNode)this.m_RootItem;
				if (this.state.selection.count > 1)
				{
					AnimationWindowHierarchyNode animationWindowHierarchyNode = this.AddClipNodeToHierarchy(animationWindowSelectionItem, array2, parentNode);
					list.Add(animationWindowHierarchyNode);
					parentNode = animationWindowHierarchyNode;
				}
				for (int j = 0; j < array2.Length; j++)
				{
					AnimationWindowCurve animationWindowCurve = array2[j];
					AnimationWindowCurve animationWindowCurve2 = (j >= array2.Length - 1) ? null : array2[j + 1];
					list2.Add(animationWindowCurve);
					bool flag = animationWindowCurve2 != null && AnimationWindowUtility.GetPropertyGroupName(animationWindowCurve2.propertyName) == AnimationWindowUtility.GetPropertyGroupName(animationWindowCurve.propertyName);
					bool flag2 = animationWindowCurve2 != null && animationWindowCurve.path.Equals(animationWindowCurve2.path) && animationWindowCurve.type == animationWindowCurve2.type;
					if (j == array2.Length - 1 || !flag || !flag2)
					{
						if (list2.Count > 1)
						{
							list.Add(this.AddPropertyGroupToHierarchy(animationWindowSelectionItem, list2.ToArray(), parentNode));
						}
						else
						{
							list.Add(this.AddPropertyToHierarchy(animationWindowSelectionItem, list2[0], parentNode));
						}
						list2.Clear();
					}
				}
			}
			return list;
		}

		private AnimationWindowHierarchyClipNode AddClipNodeToHierarchy(AnimationWindowSelectionItem selectedItem, AnimationWindowCurve[] curves, AnimationWindowHierarchyNode parentNode)
		{
			return new AnimationWindowHierarchyClipNode(parentNode, selectedItem.id, selectedItem.animationClip.name)
			{
				curves = curves
			};
		}

		private AnimationWindowHierarchyPropertyGroupNode AddPropertyGroupToHierarchy(AnimationWindowSelectionItem selectedItem, AnimationWindowCurve[] curves, AnimationWindowHierarchyNode parentNode)
		{
			List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
			Type type = curves[0].type;
			AnimationWindowHierarchyPropertyGroupNode animationWindowHierarchyPropertyGroupNode = new AnimationWindowHierarchyPropertyGroupNode(type, selectedItem.id, AnimationWindowUtility.GetPropertyGroupName(curves[0].propertyName), curves[0].path, parentNode);
			animationWindowHierarchyPropertyGroupNode.icon = this.GetIcon(selectedItem, curves[0].binding);
			animationWindowHierarchyPropertyGroupNode.indent = curves[0].depth;
			animationWindowHierarchyPropertyGroupNode.curves = curves;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve curve = curves[i];
				AnimationWindowHierarchyPropertyNode animationWindowHierarchyPropertyNode = this.AddPropertyToHierarchy(selectedItem, curve, animationWindowHierarchyPropertyGroupNode);
				animationWindowHierarchyPropertyNode.displayName = AnimationWindowUtility.GetPropertyDisplayName(animationWindowHierarchyPropertyNode.propertyName);
				list.Add(animationWindowHierarchyPropertyNode);
			}
			TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), animationWindowHierarchyPropertyGroupNode);
			return animationWindowHierarchyPropertyGroupNode;
		}

		private AnimationWindowHierarchyPropertyNode AddPropertyToHierarchy(AnimationWindowSelectionItem selectedItem, AnimationWindowCurve curve, AnimationWindowHierarchyNode parentNode)
		{
			AnimationWindowHierarchyPropertyNode animationWindowHierarchyPropertyNode = new AnimationWindowHierarchyPropertyNode(curve.type, selectedItem.id, curve.propertyName, curve.path, parentNode, curve.binding, curve.isPPtrCurve);
			if (parentNode.icon != null)
			{
				animationWindowHierarchyPropertyNode.icon = parentNode.icon;
			}
			else
			{
				animationWindowHierarchyPropertyNode.icon = this.GetIcon(selectedItem, curve.binding);
			}
			animationWindowHierarchyPropertyNode.indent = curve.depth;
			animationWindowHierarchyPropertyNode.curves = new AnimationWindowCurve[]
			{
				curve
			};
			return animationWindowHierarchyPropertyNode;
		}

		public Texture2D GetIcon(AnimationWindowSelectionItem selectedItem, EditorCurveBinding curveBinding)
		{
			Texture2D result;
			if (selectedItem.rootGameObject != null)
			{
				UnityEngine.Object animatedObject = AnimationUtility.GetAnimatedObject(selectedItem.rootGameObject, curveBinding);
				if (animatedObject != null)
				{
					result = AssetPreview.GetMiniThumbnail(animatedObject);
					return result;
				}
			}
			result = AssetPreview.GetMiniTypeThumbnail(curveBinding.type);
			return result;
		}

		public void UpdateData()
		{
			this.m_TreeView.ReloadData();
		}
	}
}
