using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal abstract class TreeViewGUI : ITreeViewGUI
	{
		internal class Styles
		{
			public GUIStyle foldout = "IN Foldout";
			public GUIStyle insertion = "PR Insertion";
			public GUIStyle insertionAbove = "PR Insertion Above";
			public GUIStyle ping = new GUIStyle("PR Ping");
			public GUIStyle toolbarButton = "ToolbarButton";
			public GUIStyle lineStyle = new GUIStyle("PR Label");
			public GUIStyle lineBoldStyle = new GUIStyle("PR Label");
			public GUIContent content = new GUIContent(EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName));
			public Styles()
			{
				this.lineStyle.alignment = TextAnchor.MiddleLeft;
				this.lineBoldStyle.alignment = TextAnchor.MiddleLeft;
				this.lineBoldStyle.font = EditorStyles.boldLabel.font;
				this.lineBoldStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
				this.ping.padding.left = 16;
				this.ping.padding.right = 16;
				this.ping.fixedHeight = 16f;
			}
		}
		protected TreeView m_TreeView;
		protected PingData m_Ping = new PingData();
		protected Rect m_DraggingInsertionMarkerRect;
		protected bool m_UseHorizontalScroll;
		protected float k_LineHeight = 16f;
		protected float k_BaseIndent = 2f;
		protected float k_IndentWidth = 14f;
		protected float k_FoldoutWidth = 12f;
		protected float k_IconWidth = 16f;
		protected float k_SpaceBetweenIconAndText = 2f;
		protected float k_TopRowMargin;
		protected float k_BottomRowMargin;
		protected float k_HalfDropBetweenHeight = 4f;
		protected static TreeViewGUI.Styles s_Styles;
		public float iconLeftPadding
		{
			get;
			set;
		}
		public float iconRightPadding
		{
			get;
			set;
		}
		public float iconTotalPadding
		{
			get
			{
				return this.iconLeftPadding + this.iconRightPadding;
			}
		}
		public Action<TreeViewItem, Rect> iconOverlayGUI
		{
			get;
			set;
		}
		protected float indentWidth
		{
			get
			{
				return this.k_IndentWidth + this.iconTotalPadding;
			}
		}
		public float halfDropBetweenHeight
		{
			get
			{
				return this.k_HalfDropBetweenHeight;
			}
		}
		public virtual float topRowMargin
		{
			get
			{
				return this.k_TopRowMargin;
			}
		}
		public virtual float bottomRowMargin
		{
			get
			{
				return this.k_BottomRowMargin;
			}
		}
		public TreeViewGUI(TreeView treeView)
		{
			this.m_TreeView = treeView;
		}
		public TreeViewGUI(TreeView treeView, bool useHorizontalScroll)
		{
			this.m_TreeView = treeView;
			this.m_UseHorizontalScroll = useHorizontalScroll;
		}
		protected virtual void InitStyles()
		{
			if (TreeViewGUI.s_Styles == null)
			{
				TreeViewGUI.s_Styles = new TreeViewGUI.Styles();
			}
		}
		protected abstract Texture GetIconForNode(TreeViewItem item);
		public virtual Vector2 GetTotalSize(List<TreeViewItem> rows)
		{
			this.InitStyles();
			float x = 1f;
			if (this.m_UseHorizontalScroll)
			{
				x = this.GetMaxWidth(rows);
			}
			return new Vector2(x, (float)rows.Count * this.k_LineHeight + this.topRowMargin + this.bottomRowMargin);
		}
		protected float GetMaxWidth(List<TreeViewItem> rows)
		{
			this.InitStyles();
			float num = 1f;
			foreach (TreeViewItem current in rows)
			{
				float num2 = 0f;
				num2 += this.GetContentIndent(current);
				if (current.icon != null)
				{
					num2 += this.k_IconWidth;
				}
				float num3;
				float num4;
				TreeViewGUI.s_Styles.lineStyle.CalcMinMaxWidth(GUIContent.Temp(current.displayName), out num3, out num4);
				num2 += num4;
				num2 += this.k_BaseIndent;
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}
		public virtual float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
		{
			return (float)row * this.k_LineHeight + this.topRowMargin;
		}
		public virtual float GetHeightOfLastRow()
		{
			return this.k_LineHeight;
		}
		public virtual int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
		{
			return (int)Mathf.Floor(heightOfTreeView / this.k_LineHeight);
		}
		public virtual void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
		{
			firstRowVisible = (int)Mathf.Floor(topPixel / this.k_LineHeight);
			lastRowVisible = firstRowVisible + (int)Mathf.Ceil(heightInPixels / this.k_LineHeight);
			firstRowVisible = Mathf.Max(firstRowVisible, 0);
			lastRowVisible = Mathf.Min(lastRowVisible, rows.Count - 1);
		}
		public virtual void BeginRowGUI()
		{
			this.InitStyles();
			this.m_DraggingInsertionMarkerRect.x = -1f;
			this.SyncFakeItem();
			if (Event.current.type != EventType.Repaint)
			{
				this.DoRenameOverlay();
			}
		}
		public virtual void EndRowGUI()
		{
			if (this.m_DraggingInsertionMarkerRect.x >= 0f && Event.current.type == EventType.Repaint)
			{
				if (this.m_TreeView.dragging.drawRowMarkerAbove)
				{
					TreeViewGUI.s_Styles.insertionAbove.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
				}
				else
				{
					TreeViewGUI.s_Styles.insertion.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
				}
			}
			if (Event.current.type == EventType.Repaint)
			{
				this.DoRenameOverlay();
			}
			this.HandlePing();
		}
		public virtual Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
		{
			Rect rect = new Rect(0f, this.GetTopPixelOfRow(row, this.m_TreeView.data.GetVisibleRows()), rowWidth, this.k_LineHeight);
			this.DoNodeGUI(rect, item, selected, focused, false);
			return rect;
		}
		protected virtual void DoNodeGUI(Rect rect, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
		{
			EditorGUIUtility.SetIconSize(new Vector2(this.k_IconWidth, this.k_IconWidth));
			float foldoutIndent = this.GetFoldoutIndent(item);
			Rect position = rect;
			int itemControlID = TreeView.GetItemControlID(item);
			bool flag = false;
			if (this.m_TreeView.dragging != null)
			{
				flag = (this.m_TreeView.dragging.GetDropTargetControlID() == itemControlID && this.m_TreeView.data.CanBeParent(item));
			}
			bool flag2 = this.IsRenaming(item.id);
			bool flag3 = this.m_TreeView.data.IsExpandable(item);
			if (flag2 && Event.current.type == EventType.Repaint)
			{
				float num = foldoutIndent + this.k_FoldoutWidth + this.k_IconWidth + this.iconTotalPadding - 1f;
				this.GetRenameOverlay().editFieldRect = new Rect(rect.x + num, rect.y, rect.width - num, rect.height);
			}
			if (Event.current.type == EventType.Repaint)
			{
				string label = item.displayName;
				if (flag2)
				{
					selected = false;
					label = string.Empty;
				}
				if (selected)
				{
					TreeViewGUI.s_Styles.lineStyle.Draw(rect, false, false, true, focused);
				}
				if (flag)
				{
					TreeViewGUI.s_Styles.lineStyle.Draw(rect, GUIContent.none, true, true, false, false);
				}
				this.DrawIconAndLabel(rect, item, label, selected, focused, useBoldFont, false);
				if (this.m_TreeView.dragging != null && this.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID)
				{
					this.m_DraggingInsertionMarkerRect = new Rect(rect.x + foldoutIndent + this.k_FoldoutWidth, rect.y, rect.width - foldoutIndent, rect.height);
				}
			}
			if (flag3)
			{
				position.x = foldoutIndent;
				position.width = this.k_FoldoutWidth;
				EditorGUI.BeginChangeCheck();
				bool flag4 = GUI.Toggle(position, this.m_TreeView.data.IsExpanded(item), GUIContent.none, TreeViewGUI.s_Styles.foldout);
				if (EditorGUI.EndChangeCheck())
				{
					if (Event.current.alt)
					{
						this.m_TreeView.data.SetExpandedWithChildren(item, flag4);
					}
					else
					{
						this.m_TreeView.data.SetExpanded(item, flag4);
					}
					if (flag4)
					{
						this.m_TreeView.UserExpandedNode(item);
					}
				}
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}
		protected virtual void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
		{
			if (!isPinging)
			{
				float contentIndent = this.GetContentIndent(item);
				rect.x += contentIndent;
				rect.width -= contentIndent;
			}
			GUIStyle gUIStyle = (!useBoldFont) ? TreeViewGUI.s_Styles.lineStyle : TreeViewGUI.s_Styles.lineBoldStyle;
			gUIStyle.padding.left = (int)(this.k_IconWidth + this.iconTotalPadding + this.k_SpaceBetweenIconAndText);
			gUIStyle.Draw(rect, label, false, false, selected, focused);
			Rect position = rect;
			position.width = this.k_IconWidth;
			position.height = this.k_IconWidth;
			position.x += this.iconLeftPadding;
			Texture iconForNode = this.GetIconForNode(item);
			if (iconForNode != null)
			{
				GUI.DrawTexture(position, iconForNode);
			}
			if (this.iconOverlayGUI != null)
			{
				Rect arg = rect;
				arg.width = this.k_IconWidth + this.iconTotalPadding;
				this.iconOverlayGUI(item, arg);
			}
		}
		public virtual void BeginPingNode(TreeViewItem item, float topPixelOfRow, float availableWidth)
		{
			if (item == null)
			{
				return;
			}
			if (topPixelOfRow >= 0f)
			{
				this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
				this.m_Ping.m_PingStyle = TreeViewGUI.s_Styles.ping;
				GUIContent content = GUIContent.Temp(item.displayName);
				Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(content);
				this.m_Ping.m_ContentRect = new Rect(this.GetContentIndent(item), topPixelOfRow, this.k_IconWidth + this.k_SpaceBetweenIconAndText + vector.x + this.iconTotalPadding, vector.y);
				this.m_Ping.m_AvailableWidth = availableWidth;
				bool useBoldFont = item.displayName.Equals("Assets");
				this.m_Ping.m_ContentDraw = delegate(Rect r)
				{
					this.DrawIconAndLabel(r, item, item.displayName, false, false, useBoldFont, true);
				};
				this.m_TreeView.Repaint();
			}
		}
		public virtual void EndPingNode()
		{
			this.m_Ping.m_TimeStart = -1f;
		}
		private void HandlePing()
		{
			this.m_Ping.HandlePing();
			if (this.m_Ping.isPinging)
			{
				this.m_TreeView.Repaint();
			}
		}
		protected RenameOverlay GetRenameOverlay()
		{
			return this.m_TreeView.state.renameOverlay;
		}
		protected virtual bool IsRenaming(int id)
		{
			return this.GetRenameOverlay().IsRenaming() && this.GetRenameOverlay().userData == id && !this.GetRenameOverlay().isWaitingForDelay;
		}
		public virtual bool BeginRename(TreeViewItem item, float delay)
		{
			return this.GetRenameOverlay().BeginRename(item.displayName, item.id, delay);
		}
		public virtual void EndRename()
		{
			if (this.GetRenameOverlay().HasKeyboardFocus())
			{
				this.m_TreeView.GrabKeyboardFocus();
			}
			this.RenameEnded();
			this.ClearRenameAndNewNodeState();
		}
		protected abstract void RenameEnded();
		public virtual void DoRenameOverlay()
		{
			if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().OnGUI())
			{
				this.EndRename();
			}
		}
		protected virtual void SyncFakeItem()
		{
		}
		protected virtual void ClearRenameAndNewNodeState()
		{
			this.m_TreeView.data.RemoveFakeItem();
			this.GetRenameOverlay().Clear();
		}
		public virtual float GetFoldoutIndent(TreeViewItem item)
		{
			if (this.m_TreeView.isSearching)
			{
				return this.k_BaseIndent;
			}
			return this.k_BaseIndent + (float)item.depth * this.indentWidth;
		}
		public virtual float GetContentIndent(TreeViewItem item)
		{
			return this.GetFoldoutIndent(item) + this.k_FoldoutWidth;
		}
	}
}
