using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchy
	{
		private AnimationWindowState m_State;

		private TreeViewController m_TreeView;

		public AnimationWindowHierarchy(AnimationWindowState state, EditorWindow owner, Rect position)
		{
			this.m_State = state;
			this.Init(owner, position);
		}

		public Vector2 GetContentSize()
		{
			return this.m_TreeView.GetContentSize();
		}

		public Rect GetTotalRect()
		{
			return this.m_TreeView.GetTotalRect();
		}

		public void OnGUI(Rect position)
		{
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(position, GUIUtility.GetControlID(FocusType.Keyboard));
		}

		public void Init(EditorWindow owner, Rect rect)
		{
			if (this.m_State.hierarchyState == null)
			{
				this.m_State.hierarchyState = new AnimationWindowHierarchyState();
			}
			this.m_TreeView = new TreeViewController(owner, this.m_State.hierarchyState);
			this.m_State.hierarchyData = new AnimationWindowHierarchyDataSource(this.m_TreeView, this.m_State);
			this.m_TreeView.Init(rect, this.m_State.hierarchyData, new AnimationWindowHierarchyGUI(this.m_TreeView, this.m_State), null);
			this.m_TreeView.deselectOnUnhandledMouseDown = true;
			TreeViewController expr_8F = this.m_TreeView;
			expr_8F.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_8F.selectionChangedCallback, new Action<int[]>(this.m_State.OnHierarchySelectionChanged));
			this.m_TreeView.ReloadData();
		}

		internal virtual bool IsRenamingNodeAllowed(TreeViewItem node)
		{
			return true;
		}

		public bool IsIDVisible(int id)
		{
			bool result;
			if (this.m_TreeView == null)
			{
				result = false;
			}
			else
			{
				IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
				result = (TreeViewController.GetIndexOfID(rows, id) >= 0);
			}
			return result;
		}

		public void EndNameEditing(bool acceptChanges)
		{
			this.m_TreeView.EndNameEditing(acceptChanges);
		}
	}
}
