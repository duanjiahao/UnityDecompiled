using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	internal abstract class TreeViewGUI : ITreeViewGUI
	{
		internal static class Styles
		{
			public static GUIStyle foldout;

			public static GUIStyle insertion;

			public static GUIStyle ping;

			public static GUIStyle toolbarButton;

			public static GUIStyle lineStyle;

			public static GUIStyle lineBoldStyle;

			public static GUIStyle selectionStyle;

			public static GUIContent content;

			public static float foldoutWidth
			{
				get
				{
					return TreeViewGUI.Styles.foldout.fixedWidth;
				}
			}

			static Styles()
			{
				TreeViewGUI.Styles.foldout = "IN Foldout";
				TreeViewGUI.Styles.insertion = new GUIStyle("PR Insertion");
				TreeViewGUI.Styles.ping = new GUIStyle("PR Ping");
				TreeViewGUI.Styles.toolbarButton = "ToolbarButton";
				TreeViewGUI.Styles.lineStyle = new GUIStyle("PR Label");
				TreeViewGUI.Styles.selectionStyle = new GUIStyle("PR Label");
				TreeViewGUI.Styles.content = new GUIContent(EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName));
				Texture2D background = TreeViewGUI.Styles.lineStyle.hover.background;
				TreeViewGUI.Styles.lineStyle.onNormal.background = background;
				TreeViewGUI.Styles.lineStyle.onActive.background = background;
				TreeViewGUI.Styles.lineStyle.onFocused.background = background;
				TreeViewGUI.Styles.lineStyle.alignment = TextAnchor.UpperLeft;
				TreeViewGUI.Styles.lineStyle.padding.top = 2;
				TreeViewGUI.Styles.lineStyle.fixedHeight = 0f;
				TreeViewGUI.Styles.lineBoldStyle = new GUIStyle(TreeViewGUI.Styles.lineStyle);
				TreeViewGUI.Styles.lineBoldStyle.font = EditorStyles.boldLabel.font;
				TreeViewGUI.Styles.lineBoldStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
				TreeViewGUI.Styles.ping.padding.left = 16;
				TreeViewGUI.Styles.ping.padding.right = 16;
				TreeViewGUI.Styles.ping.fixedHeight = 16f;
				TreeViewGUI.Styles.selectionStyle.fixedHeight = 0f;
				TreeViewGUI.Styles.selectionStyle.border = new RectOffset();
				TreeViewGUI.Styles.insertion.overflow = new RectOffset(4, 0, 0, 0);
			}
		}

		protected TreeViewController m_TreeView;

		protected PingData m_Ping = new PingData();

		protected Rect m_DraggingInsertionMarkerRect;

		protected bool m_UseHorizontalScroll;

		private bool m_AnimateScrollBarOnExpandCollapse = true;

		public float k_LineHeight = 16f;

		public float k_BaseIndent = 2f;

		public float k_IndentWidth = 14f;

		public float k_IconWidth = 16f;

		public float k_SpaceBetweenIconAndText = 2f;

		public float k_TopRowMargin = 0f;

		public float k_BottomRowMargin = 0f;

		public float k_HalfDropBetweenHeight = 4f;

		public float customFoldoutYOffset = 0f;

		public float extraInsertionMarkerIndent = 0f;

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

		public float indentWidth
		{
			get
			{
				return this.k_IndentWidth + this.iconTotalPadding;
			}
		}

		public float extraSpaceBeforeIconAndLabel
		{
			get;
			set;
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

		public TreeViewGUI(TreeViewController treeView)
		{
			this.m_TreeView = treeView;
		}

		public TreeViewGUI(TreeViewController treeView, bool useHorizontalScroll)
		{
			this.m_TreeView = treeView;
			this.m_UseHorizontalScroll = useHorizontalScroll;
		}

		public virtual void OnInitialize()
		{
		}

		protected virtual Texture GetIconForItem(TreeViewItem item)
		{
			return item.icon;
		}

		public virtual Vector2 GetTotalSize()
		{
			float x = 1f;
			if (this.m_UseHorizontalScroll)
			{
				IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
				x = this.GetMaxWidth(rows);
			}
			float num = (float)this.m_TreeView.data.rowCount * this.k_LineHeight + this.topRowMargin + this.bottomRowMargin;
			if (this.m_AnimateScrollBarOnExpandCollapse && this.m_TreeView.expansionAnimator.isAnimating)
			{
				num -= this.m_TreeView.expansionAnimator.deltaHeight;
			}
			return new Vector2(x, num);
		}

		protected float GetMaxWidth(IList<TreeViewItem> rows)
		{
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
				TreeViewGUI.Styles.lineStyle.CalcMinMaxWidth(GUIContent.Temp(current.displayName), out num3, out num4);
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
			}
			else
			{
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
		}

		public virtual void BeginRowGUI()
		{
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
				TreeViewGUI.Styles.insertion.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
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

		protected virtual void DrawItemBackground(Rect rect, int row, TreeViewItem item, bool selected, bool focused)
		{
		}

		public virtual Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
		{
			float num = this.GetContentIndent(item) + this.extraSpaceBeforeIconAndLabel;
			if (this.GetIconForItem(item) != null)
			{
				num += this.k_SpaceBetweenIconAndText + this.k_IconWidth + this.iconTotalPadding;
			}
			return new Rect(rowRect.x + num, rowRect.y, rowRect.width - num, EditorGUIUtility.singleLineHeight);
		}

		protected virtual void DoItemGUI(Rect rect, int row, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
		{
			EditorGUIUtility.SetIconSize(new Vector2(this.k_IconWidth, this.k_IconWidth));
			float foldoutIndent = this.GetFoldoutIndent(item);
			int itemControlID = TreeViewController.GetItemControlID(item);
			bool flag = false;
			if (this.m_TreeView.dragging != null)
			{
				flag = (this.m_TreeView.dragging.GetDropTargetControlID() == itemControlID && this.m_TreeView.data.CanBeParent(item));
			}
			bool flag2 = this.IsRenaming(item.id);
			bool flag3 = this.m_TreeView.data.IsExpandable(item);
			if (flag2 && Event.current.type == EventType.Repaint)
			{
				this.GetRenameOverlay().editFieldRect = this.GetRenameRect(rect, row, item);
			}
			string label = item.displayName;
			if (flag2)
			{
				selected = false;
				label = "";
			}
			if (Event.current.type == EventType.Repaint)
			{
				this.DrawItemBackground(rect, row, item, selected, focused);
				if (selected)
				{
					TreeViewGUI.Styles.selectionStyle.Draw(rect, false, false, true, focused);
				}
				if (flag)
				{
					TreeViewGUI.Styles.lineStyle.Draw(rect, GUIContent.none, true, true, false, false);
				}
				if (this.m_TreeView.dragging != null && this.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID)
				{
					float y = ((!this.m_TreeView.dragging.drawRowMarkerAbove) ? rect.yMax : rect.y) - TreeViewGUI.Styles.insertion.fixedHeight * 0.5f;
					this.m_DraggingInsertionMarkerRect = new Rect(rect.x + foldoutIndent + this.extraInsertionMarkerIndent + TreeViewGUI.Styles.foldoutWidth + (float)TreeViewGUI.Styles.lineStyle.margin.left, y, rect.width - foldoutIndent, rect.height);
				}
			}
			this.OnContentGUI(rect, row, item, label, selected, focused, useBoldFont, false);
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

		private float GetFoldoutYPosition(float rectY)
		{
			return rectY + this.customFoldoutYOffset;
		}

		protected virtual Rect DoFoldout(Rect rect, TreeViewItem item, int row)
		{
			float foldoutIndent = this.GetFoldoutIndent(item);
			Rect rect2 = new Rect(rect.x + foldoutIndent, this.GetFoldoutYPosition(rect.y), TreeViewGUI.Styles.foldoutWidth, EditorGUIUtility.singleLineHeight);
			this.FoldoutButton(rect2, item, row, TreeViewGUI.Styles.foldout);
			return rect2;
		}

		protected virtual void FoldoutButton(Rect foldoutRect, TreeViewItem item, int row, GUIStyle foldoutStyle)
		{
			TreeViewItemExpansionAnimator expansionAnimator = this.m_TreeView.expansionAnimator;
			EditorGUI.BeginChangeCheck();
			bool value = (!expansionAnimator.IsAnimating(item.id)) ? this.m_TreeView.data.IsExpanded(item) : expansionAnimator.isExpanding;
			bool expand = GUI.Toggle(foldoutRect, value, GUIContent.none, foldoutStyle);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_TreeView.UserInputChangedExpandedState(item, row, expand);
			}
		}

		protected virtual void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
		{
			if (Event.current.rawType == EventType.Repaint)
			{
				if (!isPinging)
				{
					float num = this.GetContentIndent(item) + this.extraSpaceBeforeIconAndLabel;
					rect.xMin += num;
				}
				GUIStyle gUIStyle = (!useBoldFont) ? TreeViewGUI.Styles.lineStyle : TreeViewGUI.Styles.lineBoldStyle;
				Rect position = rect;
				position.width = this.k_IconWidth;
				position.x += this.iconLeftPadding;
				Texture iconForItem = this.GetIconForItem(item);
				if (iconForItem != null)
				{
					GUI.DrawTexture(position, iconForItem, ScaleMode.ScaleToFit);
				}
				if (this.iconOverlayGUI != null)
				{
					Rect arg = rect;
					arg.width = this.k_IconWidth + this.iconTotalPadding;
					this.iconOverlayGUI(item, arg);
				}
				gUIStyle.padding.left = 0;
				if (iconForItem != null)
				{
					rect.xMin += this.k_IconWidth + this.iconTotalPadding + this.k_SpaceBetweenIconAndText;
				}
				gUIStyle.Draw(rect, label, false, false, selected, focused);
			}
		}

		public virtual void BeginPingItem(TreeViewItem item, float topPixelOfRow, float availableWidth)
		{
			if (item != null)
			{
				if (topPixelOfRow >= 0f)
				{
					this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
					this.m_Ping.m_PingStyle = TreeViewGUI.Styles.ping;
					GUIContent content = GUIContent.Temp(item.displayName);
					Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(content);
					this.m_Ping.m_ContentRect = new Rect(this.GetContentIndent(item) + this.extraSpaceBeforeIconAndLabel, topPixelOfRow, this.k_IconWidth + this.k_SpaceBetweenIconAndText + vector.x + this.iconTotalPadding, vector.y);
					this.m_Ping.m_AvailableWidth = availableWidth;
					int row = this.m_TreeView.data.GetRow(item.id);
					bool useBoldFont = item.displayName.Equals("Assets");
					this.m_Ping.m_ContentDraw = delegate(Rect r)
					{
						this.OnContentGUI(r, row, item, item.displayName, false, false, useBoldFont, true);
					};
					this.m_TreeView.Repaint();
				}
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
			float result;
			if (this.m_TreeView.isSearching)
			{
				result = this.k_BaseIndent;
			}
			else
			{
				result = this.k_BaseIndent + (float)item.depth * this.indentWidth;
			}
			return result;
		}

		public virtual float GetContentIndent(TreeViewItem item)
		{
			return this.GetFoldoutIndent(item) + TreeViewGUI.Styles.foldoutWidth + (float)TreeViewGUI.Styles.lineStyle.margin.left;
		}
	}
}
