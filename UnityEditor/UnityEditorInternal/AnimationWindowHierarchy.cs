using System;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchy
	{
		private TreeView m_TreeView;
		public AnimationWindowState state
		{
			get;
			set;
		}
		public AnimationWindowHierarchy(AnimationWindowState state, EditorWindow owner, Rect position)
		{
			this.state = state;
			this.Init(owner, position);
		}
		public void OnGUI(Rect position, EditorWindow owner)
		{
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(position, GUIUtility.GetControlID(FocusType.Keyboard));
		}
		public void Init(EditorWindow owner, Rect rect)
		{
			if (this.state.m_hierarchyState == null)
			{
				this.state.m_hierarchyState = new AnimationWindowHierarchyState();
			}
			this.m_TreeView = new TreeView(owner, this.state.m_hierarchyState);
			this.state.m_HierarchyData = new AnimationWindowHierarchyDataSource(this.m_TreeView, this.state);
			this.m_TreeView.Init(rect, this.state.m_HierarchyData, new AnimationWindowHierarchyGUI(this.m_TreeView, this.state), new AnimationWindowHierarchyDragging());
			this.m_TreeView.deselectOnUnhandledMouseDown = true;
			TreeView expr_92 = this.m_TreeView;
			expr_92.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_92.selectionChangedCallback, new Action<int[]>(this.state.OnHierarchySelectionChanged));
			this.m_TreeView.ReloadData();
		}
		internal virtual bool IsRenamingNodeAllowed(TreeViewItem node)
		{
			return true;
		}
		public bool IsIDVisible(int id)
		{
			return this.m_TreeView != null && this.m_TreeView.IsVisible(id);
		}
	}
}
