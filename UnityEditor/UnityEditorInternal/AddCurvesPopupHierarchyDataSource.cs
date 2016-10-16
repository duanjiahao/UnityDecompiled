using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupHierarchyDataSource : TreeViewDataSource
	{
		private IAnimationRecordingState state
		{
			get;
			set;
		}

		public static bool showEntireHierarchy
		{
			get;
			set;
		}

		public AddCurvesPopupHierarchyDataSource(TreeView treeView, IAnimationRecordingState animationWindowState) : base(treeView)
		{
			base.showRootItem = false;
			base.rootIsCollapsable = false;
			this.state = animationWindowState;
		}

		private void SetupRootNodeSettings()
		{
			base.showRootItem = false;
			this.SetExpanded(this.root, true);
		}

		public override void FetchData()
		{
			if (AddCurvesPopup.gameObject == null)
			{
				return;
			}
			this.AddGameObjectToHierarchy(AddCurvesPopup.gameObject, null);
			this.SetupRootNodeSettings();
			this.m_NeedRefreshVisibleFolders = true;
		}

		private TreeViewItem AddGameObjectToHierarchy(GameObject gameObject, TreeViewItem parent)
		{
			string path = AnimationUtility.CalculateTransformPath(gameObject.transform, this.state.activeRootGameObject.transform);
			TreeViewItem treeViewItem = new AddCurvesPopupGameObjectNode(gameObject, parent, gameObject.name);
			List<TreeViewItem> list = new List<TreeViewItem>();
			if (parent == null)
			{
				this.m_RootItem = treeViewItem;
			}
			EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, this.state.activeRootGameObject);
			List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
			for (int i = 0; i < animatableBindings.Length; i++)
			{
				EditorCurveBinding editorCurveBinding = animatableBindings[i];
				list2.Add(editorCurveBinding);
				if (editorCurveBinding.propertyName == "m_IsActive")
				{
					if (editorCurveBinding.path != string.Empty)
					{
						TreeViewItem treeViewItem2 = this.CreateNode(list2.ToArray(), treeViewItem);
						if (treeViewItem2 != null)
						{
							list.Add(treeViewItem2);
						}
						list2.Clear();
					}
					else
					{
						list2.Clear();
					}
				}
				else
				{
					bool flag = i == animatableBindings.Length - 1;
					bool flag2 = false;
					if (!flag)
					{
						flag2 = (animatableBindings[i + 1].type != editorCurveBinding.type);
					}
					if (AnimationWindowUtility.IsCurveCreated(this.state.activeAnimationClip, editorCurveBinding))
					{
						list2.Remove(editorCurveBinding);
					}
					if (editorCurveBinding.type == typeof(Animator) && editorCurveBinding.propertyName == "m_Enabled")
					{
						list2.Remove(editorCurveBinding);
					}
					if ((flag || flag2) && list2.Count > 0)
					{
						list.Add(this.AddAnimatableObjectToHierarchy(this.state.activeRootGameObject, list2.ToArray(), treeViewItem, path));
						list2.Clear();
					}
				}
			}
			if (AddCurvesPopupHierarchyDataSource.showEntireHierarchy)
			{
				for (int j = 0; j < gameObject.transform.childCount; j++)
				{
					Transform child = gameObject.transform.GetChild(j);
					TreeViewItem treeViewItem3 = this.AddGameObjectToHierarchy(child.gameObject, treeViewItem);
					if (treeViewItem3 != null)
					{
						list.Add(treeViewItem3);
					}
				}
			}
			TreeViewUtility.SetChildParentReferences(list, treeViewItem);
			return treeViewItem;
		}

		private static string GetClassName(GameObject root, EditorCurveBinding binding)
		{
			UnityEngine.Object animatedObject = AnimationUtility.GetAnimatedObject(root, binding);
			if (animatedObject)
			{
				return ObjectNames.GetInspectorTitle(animatedObject);
			}
			return binding.type.Name;
		}

		private TreeViewItem AddAnimatableObjectToHierarchy(GameObject root, EditorCurveBinding[] curveBindings, TreeViewItem parentNode, string path)
		{
			TreeViewItem treeViewItem = new AddCurvesPopupObjectNode(parentNode, path, AddCurvesPopupHierarchyDataSource.GetClassName(root, curveBindings[0]));
			treeViewItem.icon = AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(root, curveBindings[0]));
			List<TreeViewItem> list = new List<TreeViewItem>();
			List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
			for (int i = 0; i < curveBindings.Length; i++)
			{
				EditorCurveBinding item = curveBindings[i];
				list2.Add(item);
				if (i == curveBindings.Length - 1 || AnimationWindowUtility.GetPropertyGroupName(curveBindings[i + 1].propertyName) != AnimationWindowUtility.GetPropertyGroupName(item.propertyName))
				{
					TreeViewItem treeViewItem2 = this.CreateNode(list2.ToArray(), treeViewItem);
					if (treeViewItem2 != null)
					{
						list.Add(treeViewItem2);
					}
					list2.Clear();
				}
			}
			list.Sort();
			TreeViewUtility.SetChildParentReferences(list, treeViewItem);
			return treeViewItem;
		}

		private TreeViewItem CreateNode(EditorCurveBinding[] curveBindings, TreeViewItem parentNode)
		{
			AddCurvesPopupPropertyNode addCurvesPopupPropertyNode = new AddCurvesPopupPropertyNode(parentNode, curveBindings);
			if (AnimationWindowUtility.IsRectTransformPosition(addCurvesPopupPropertyNode.curveBindings[0]))
			{
				addCurvesPopupPropertyNode.curveBindings = new EditorCurveBinding[]
				{
					addCurvesPopupPropertyNode.curveBindings[2]
				};
			}
			addCurvesPopupPropertyNode.icon = parentNode.icon;
			return addCurvesPopupPropertyNode;
		}

		public void UpdateData()
		{
			this.m_TreeView.ReloadData();
		}
	}
}
