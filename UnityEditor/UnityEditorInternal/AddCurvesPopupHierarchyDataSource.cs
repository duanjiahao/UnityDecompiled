using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupHierarchyDataSource : TreeViewDataSource
	{
		public static bool showEntireHierarchy
		{
			get;
			set;
		}

		public AddCurvesPopupHierarchyDataSource(TreeViewController treeView) : base(treeView)
		{
			base.showRootItem = false;
			base.rootIsCollapsable = false;
		}

		private void SetupRootNodeSettings()
		{
			base.showRootItem = false;
			this.SetExpanded(base.root, true);
		}

		public override void FetchData()
		{
			if (AddCurvesPopup.selection != null)
			{
				AnimationWindowSelectionItem[] array = AddCurvesPopup.selection.ToArray();
				if (array.Length > 1)
				{
					this.m_RootItem = new AddCurvesPopupObjectNode(null, "", "");
				}
				AnimationWindowSelectionItem[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					AnimationWindowSelectionItem animationWindowSelectionItem = array2[i];
					if (animationWindowSelectionItem.canAddCurves)
					{
						if (animationWindowSelectionItem.rootGameObject != null)
						{
							this.AddGameObjectToHierarchy(animationWindowSelectionItem.rootGameObject, animationWindowSelectionItem, this.m_RootItem);
						}
						else if (animationWindowSelectionItem.scriptableObject != null)
						{
							this.AddScriptableObjectToHierarchy(animationWindowSelectionItem.scriptableObject, animationWindowSelectionItem, this.m_RootItem);
						}
					}
				}
				this.SetupRootNodeSettings();
				this.m_NeedRefreshRows = true;
			}
		}

		private TreeViewItem AddGameObjectToHierarchy(GameObject gameObject, AnimationWindowSelectionItem selectionItem, TreeViewItem parent)
		{
			string path = AnimationUtility.CalculateTransformPath(gameObject.transform, selectionItem.rootGameObject.transform);
			TreeViewItem treeViewItem = new AddCurvesPopupGameObjectNode(gameObject, parent, gameObject.name);
			List<TreeViewItem> list = new List<TreeViewItem>();
			if (this.m_RootItem == null)
			{
				this.m_RootItem = treeViewItem;
			}
			EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, selectionItem.rootGameObject);
			List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
			for (int i = 0; i < animatableBindings.Length; i++)
			{
				EditorCurveBinding editorCurveBinding = animatableBindings[i];
				list2.Add(editorCurveBinding);
				if (editorCurveBinding.propertyName == "m_IsActive")
				{
					if (editorCurveBinding.path != "")
					{
						TreeViewItem treeViewItem2 = this.CreateNode(selectionItem, list2.ToArray(), treeViewItem);
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
					if (AnimationWindowUtility.IsCurveCreated(selectionItem.animationClip, editorCurveBinding))
					{
						list2.Remove(editorCurveBinding);
					}
					if (editorCurveBinding.type == typeof(Animator) && editorCurveBinding.propertyName == "m_Enabled")
					{
						list2.Remove(editorCurveBinding);
					}
					if ((flag || flag2) && list2.Count > 0)
					{
						list.Add(this.AddAnimatableObjectToHierarchy(selectionItem, list2.ToArray(), treeViewItem, path));
						list2.Clear();
					}
				}
			}
			if (AddCurvesPopupHierarchyDataSource.showEntireHierarchy)
			{
				for (int j = 0; j < gameObject.transform.childCount; j++)
				{
					Transform child = gameObject.transform.GetChild(j);
					TreeViewItem treeViewItem3 = this.AddGameObjectToHierarchy(child.gameObject, selectionItem, treeViewItem);
					if (treeViewItem3 != null)
					{
						list.Add(treeViewItem3);
					}
				}
			}
			TreeViewUtility.SetChildParentReferences(list, treeViewItem);
			return treeViewItem;
		}

		private TreeViewItem AddScriptableObjectToHierarchy(ScriptableObject scriptableObject, AnimationWindowSelectionItem selectionItem, TreeViewItem parent)
		{
			EditorCurveBinding[] scriptableObjectAnimatableBindings = AnimationUtility.GetScriptableObjectAnimatableBindings(scriptableObject);
			EditorCurveBinding[] array = (from c in scriptableObjectAnimatableBindings
			where !AnimationWindowUtility.IsCurveCreated(selectionItem.animationClip, c)
			select c).ToArray<EditorCurveBinding>();
			TreeViewItem treeViewItem;
			if (array.Length > 0)
			{
				treeViewItem = this.AddAnimatableObjectToHierarchy(selectionItem, array, parent, "");
			}
			else
			{
				treeViewItem = new AddCurvesPopupObjectNode(parent, "", scriptableObject.name);
			}
			if (this.m_RootItem == null)
			{
				this.m_RootItem = treeViewItem;
			}
			return treeViewItem;
		}

		private static string GetClassName(AnimationWindowSelectionItem selectionItem, EditorCurveBinding binding)
		{
			string result;
			if (selectionItem.rootGameObject != null)
			{
				UnityEngine.Object animatedObject = AnimationUtility.GetAnimatedObject(selectionItem.rootGameObject, binding);
				if (animatedObject)
				{
					result = ObjectNames.GetInspectorTitle(animatedObject);
					return result;
				}
			}
			result = binding.type.Name;
			return result;
		}

		private static Texture2D GetIcon(AnimationWindowSelectionItem selectionItem, EditorCurveBinding binding)
		{
			Texture2D result;
			if (selectionItem.rootGameObject != null)
			{
				result = AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(selectionItem.rootGameObject, binding));
			}
			else if (selectionItem.scriptableObject != null)
			{
				result = AssetPreview.GetMiniThumbnail(selectionItem.scriptableObject);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private TreeViewItem AddAnimatableObjectToHierarchy(AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] curveBindings, TreeViewItem parentNode, string path)
		{
			TreeViewItem treeViewItem = new AddCurvesPopupObjectNode(parentNode, path, AddCurvesPopupHierarchyDataSource.GetClassName(selectionItem, curveBindings[0]));
			treeViewItem.icon = AddCurvesPopupHierarchyDataSource.GetIcon(selectionItem, curveBindings[0]);
			List<TreeViewItem> list = new List<TreeViewItem>();
			List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
			for (int i = 0; i < curveBindings.Length; i++)
			{
				EditorCurveBinding item = curveBindings[i];
				list2.Add(item);
				if (i == curveBindings.Length - 1 || AnimationWindowUtility.GetPropertyGroupName(curveBindings[i + 1].propertyName) != AnimationWindowUtility.GetPropertyGroupName(item.propertyName))
				{
					TreeViewItem treeViewItem2 = this.CreateNode(selectionItem, list2.ToArray(), treeViewItem);
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

		private TreeViewItem CreateNode(AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] curveBindings, TreeViewItem parentNode)
		{
			AddCurvesPopupPropertyNode addCurvesPopupPropertyNode = new AddCurvesPopupPropertyNode(parentNode, selectionItem, curveBindings);
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
