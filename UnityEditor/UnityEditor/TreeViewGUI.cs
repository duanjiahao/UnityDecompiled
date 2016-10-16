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

			public GUIStyle lineBoldStyle;

			public GUIStyle selectionStyle = new GUIStyle("PR Label");

			public GUIContent content = new GUIContent(EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName));

			public Styles()
			{
				Texture2D background = this.lineStyle.hover.background;
				this.lineStyle.onNormal.background = background;
				this.lineStyle.onActive.background = background;
				this.lineStyle.onFocused.background = background;
				this.lineStyle.alignment = TextAnchor.MiddleLeft;
				this.lineBoldStyle = new GUIStyle(this.lineStyle);
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

		private bool m_AnimateScrollBarOnExpandCollapse = true;

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

		public virtual void OnInitialize()
		{
		}

		protected virtual void InitStyles()
		{
			if (TreeViewGUI.s_Styles == null)
			{
				TreeViewGUI.s_Styles = new TreeViewGUI.Styles();
			}
		}

		protected virtual Texture GetIconForItem(TreeViewItem item)
		{
			return item.icon;
		}

		public virtual Vector2 GetTotalSize()
		{
			this.InitStyles();
			float x = 1f;
			if (this.m_UseHorizontalScroll)
			{
				List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
				x = this.GetMaxWidth(rows);
			}
			float num = (float)this.m_TreeView.data.rowCount * this.k_LineHeight + this.topRowMargin + this.bottomRowMargin;
			if (this.m_AnimateScrollBarOnExpandCollapse && this.m_TreeView.expansionAnimator.isAnimating)
			{
				num -= this.m_TreeView.expansionAnimator.deltaHeight;
			}
			return new Vector2(x, num);
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

		public virtual int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
		{
			return (int)Mathf.Floor(heightOfTreeView / this.k_LineHeight);
		}

		public virtual void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
		{
			if (this.m_TreeView.data.rowCount == 0)
			{
				firstRowVisible = (lastRowVisible = -1);
				return;
			}
			float y = this.m_TreeView.state.scrollPos.y;
			float height = this.m_TreeView.GetTotalRect().height;
			firstRowVisible = (int)Mathf.Floor((y - this.topRowMargin) / this.k_LineHeight);
			lastRowVisible = firstRowVisible + (int)Mathf.Ceil(height / this.k_LineHeight);
			firstRowVisible = Mathf.Max(firstRowVisible, 0);
			lastRowVisible = Mathf.Min(lastRowVisible, this.m_TreeView.data.rowCount - 1);
			if (firstRowVisible >= this.m_TreeView.data.rowCount && firstRowVisible > 0)
			{
				this.m_TreeView.state.scrollPos.y = 0f;
				this.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
			}
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

		public virtual void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
		{
			this.DoItemGUI(rowRect, row, item, selected, focused, false);
		}

		protected virtual void DoItemGUI(Rect rect, int row, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
		{
			EditorGUIUtility.SetIconSize(new Vector2(this.k_IconWidth, this.k_IconWidth));
			float foldoutIndent = this.GetFoldoutIndent(item);
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
				float num = (!(item.icon == null)) ? this.k_IconWidth : 0f;
				float num2 = foldoutIndent + this.k_FoldoutWidth + num + this.iconTotalPadding - 1f;
				this.GetRenameOverlay().editFieldRect = new Rect(rect.x + num2, rect.y, rect.width - num2, rect.height);
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
					TreeViewGUI.s_Styles.selectionStyle.Draw(rect, false, false, true, focused);
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
				this.DoFoldout(rect, item, row);
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}

		private float GetTopPixelOfRow(int row)
		{
			return (float)row * this.k_LineHeight + this.topRowMargin;
		}

		public virtual Rect GetRowRect(int row, float rowWidth)
		{
			return new Rect(0f, this.GetTopPixelOfRow(row), rowWidth, this.k_LineHeight);
		}

		public virtual Rect GetRectForFraming(int row)
		{
			return this.GetRowRect(row, 1f);
		}

		protected virtual Rect DoFoldout(Rect rowRect, TreeViewItem item, int row)
		{
			float foldoutIndent = this.GetFoldoutIndent(item);
			Rect rect = new Rect(foldoutIndent, rowRect.y, this.k_FoldoutWidth, rowRect.height);
			TreeViewItemExpansionAnimator expansionAnimator = this.m_TreeView.expansionAnimator;
			EditorGUI.BeginChangeCheck();
			bool expand;
			if (expansionAnimator.IsAnimating(item.id))
			{
				Matrix4x4 matrix = GUI.matrix;
				float num = Mathf.Min(1f, expansionAnimator.expandedValueNormalized * 2f);
				float angle;
				if (!expansionAnimator.isExpanding)
				{
					angle = num * 90f;
				}
				else
				{
					angle = (1f - num) * -90f;
				}
				GUIUtility.RotateAroundPivot(angle, rect.center);
				bool isExpanding = expansionAnimator.isExpanding;
				expand = GUI.Toggle(rect, isExpanding, GUIContent.none, TreeViewGUI.s_Styles.foldout);
				GUI.matrix = matrix;
			}
			else
			{
				expand = GUI.Toggle(rect, this.m_TreeView.data.IsExpanded(item), GUIContent.none, TreeViewGUI.s_Styles.foldout);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.m_TreeView.UserInputChangedExpandedState(item, row, expand);
			}
			return rect;
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
			Texture iconForItem = this.GetIconForItem(item);
			if (iconForItem != null)
			{
				GUI.DrawTexture(position, iconForItem);
			}
			if (this.iconOverlayGUI != null)
			{
				Rect arg = rect;
				arg.width = this.k_IconWidth + this.iconTotalPadding;
				this.iconOverlayGUI(item, arg);
			}
		}

		public virtual void BeginPingItem(TreeViewItem item, float topPixelOfRow, float availableWidth)
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

		public virtual void EndPingItem()
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
			this.ClearRenameAndNewItemState();
		}

		protected virtual void RenameEnded()
		{
		}

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

		protected virtual void ClearRenameAndNewItemState()
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
