using System;
using UnityEngine;
namespace UnityEditor
{
	internal class GameObjectTreeViewGUI : TreeViewGUI
	{
		private enum GameObjectColorType
		{
			Normal,
			Prefab,
			BrokenPrefab,
			Count
		}
		internal class GameObjectStyles
		{
			public GUIStyle disabledLabel = new GUIStyle("PR DisabledLabel");
			public GUIStyle prefabLabel = new GUIStyle("PR PrefabLabel");
			public GUIStyle disabledPrefabLabel = new GUIStyle("PR DisabledPrefabLabel");
			public GUIStyle brokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel");
			public GUIStyle disabledBrokenPrefabLabel = new GUIStyle("PR DisabledBrokenPrefabLabel");
			public GameObjectStyles()
			{
				this.disabledLabel.alignment = TextAnchor.MiddleLeft;
				this.prefabLabel.alignment = TextAnchor.MiddleLeft;
				this.disabledPrefabLabel.alignment = TextAnchor.MiddleLeft;
				this.brokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
				this.disabledBrokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
			}
		}
		protected static GameObjectTreeViewGUI.GameObjectStyles s_GOStyles;
		public GameObjectTreeViewGUI(TreeView treeView, bool useHorizontalScroll) : base(treeView, useHorizontalScroll)
		{
			this.k_IconWidth = 0f;
			this.k_TopRowMargin = 4f;
		}
		protected override void InitStyles()
		{
			base.InitStyles();
			if (GameObjectTreeViewGUI.s_GOStyles == null)
			{
				GameObjectTreeViewGUI.s_GOStyles = new GameObjectTreeViewGUI.GameObjectStyles();
			}
		}
		public override bool BeginRename(TreeViewItem item, float delay)
		{
			GameObjectTreeViewItem gameObjectTreeViewItem = item as GameObjectTreeViewItem;
			UnityEngine.Object objectPPTR = gameObjectTreeViewItem.objectPPTR;
			if ((objectPPTR.hideFlags & HideFlags.NotEditable) != HideFlags.None)
			{
				Debug.LogWarning("Unable to rename a GameObject with HideFlags.NotEditable.");
				return false;
			}
			return base.BeginRename(item, delay);
		}
		protected override void RenameEnded()
		{
			string text = (!string.IsNullOrEmpty(base.GetRenameOverlay().name)) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
			int userData = base.GetRenameOverlay().userData;
			bool userAcceptedRename = base.GetRenameOverlay().userAcceptedRename;
			if (userAcceptedRename)
			{
				ObjectNames.SetNameSmartWithInstanceID(userData, text);
				TreeViewItem treeViewItem = this.m_TreeView.data.FindItem(userData);
				if (treeViewItem != null)
				{
					treeViewItem.displayName = text;
				}
				EditorApplication.RepaintAnimationWindow();
			}
		}
		protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
		{
			if (!isPinging)
			{
				float contentIndent = this.GetContentIndent(item);
				rect.x += contentIndent;
				rect.width -= contentIndent;
			}
			GameObjectTreeViewItem gameObjectTreeViewItem = item as GameObjectTreeViewItem;
			int colorCode = gameObjectTreeViewItem.colorCode;
			if (string.IsNullOrEmpty(item.displayName))
			{
				if (gameObjectTreeViewItem.objectPPTR != null)
				{
					gameObjectTreeViewItem.displayName = gameObjectTreeViewItem.objectPPTR.name;
				}
				else
				{
					gameObjectTreeViewItem.displayName = "deleted gameobject";
				}
				label = gameObjectTreeViewItem.displayName;
			}
			GUIStyle gUIStyle = TreeViewGUI.s_Styles.lineStyle;
			if (!gameObjectTreeViewItem.shouldDisplay)
			{
				gUIStyle = GameObjectTreeViewGUI.s_GOStyles.disabledLabel;
			}
			else
			{
				if ((colorCode & 3) == 0)
				{
					gUIStyle = ((colorCode >= 4) ? GameObjectTreeViewGUI.s_GOStyles.disabledLabel : TreeViewGUI.s_Styles.lineStyle);
				}
				else
				{
					if ((colorCode & 3) == 1)
					{
						gUIStyle = ((colorCode >= 4) ? GameObjectTreeViewGUI.s_GOStyles.disabledPrefabLabel : GameObjectTreeViewGUI.s_GOStyles.prefabLabel);
					}
					else
					{
						if ((colorCode & 3) == 2)
						{
							gUIStyle = ((colorCode >= 4) ? GameObjectTreeViewGUI.s_GOStyles.disabledBrokenPrefabLabel : GameObjectTreeViewGUI.s_GOStyles.brokenPrefabLabel);
						}
					}
				}
			}
			gUIStyle.padding.left = (int)this.k_SpaceBetweenIconAndText;
			gUIStyle.Draw(rect, label, false, false, selected, focused);
		}
		protected override Texture GetIconForNode(TreeViewItem item)
		{
			return null;
		}
	}
}
