using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class PackageExportTreeView
	{
		public enum EnabledState
		{
			NotSet = -1,
			None,
			All,
			Mixed
		}

		private class PackageExportTreeViewItem : TreeViewItem
		{
			private PackageExportTreeView.EnabledState m_EnabledState = PackageExportTreeView.EnabledState.NotSet;

			public ExportPackageItem item
			{
				get;
				set;
			}

			public PackageExportTreeView.EnabledState enabledState
			{
				get
				{
					return (this.item == null) ? this.m_EnabledState : ((PackageExportTreeView.EnabledState)this.item.enabledStatus);
				}
				set
				{
					if (this.item != null)
					{
						this.item.enabledStatus = (int)value;
					}
					else
					{
						this.m_EnabledState = value;
					}
				}
			}

			public bool isFolder
			{
				get
				{
					return this.item == null || this.item.isFolder;
				}
			}

			public PackageExportTreeViewItem(ExportPackageItem itemIn, int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
			{
				this.item = itemIn;
			}
		}

		private class PackageExportTreeViewGUI : TreeViewGUI
		{
			internal static class Constants
			{
				public static Texture2D folderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
			}

			public Action<PackageExportTreeView.PackageExportTreeViewItem> itemWasToggled;

			private PackageExportTreeView m_PackageExportView;

			public int showPreviewForID
			{
				get;
				set;
			}

			public PackageExportTreeViewGUI(TreeView treeView, PackageExportTreeView view) : base(treeView)
			{
				this.m_PackageExportView = view;
				this.k_BaseIndent = 4f;
				if (!PackageExportTreeView.s_UseFoldouts)
				{
					this.k_FoldoutWidth = 0f;
				}
			}

			public override void OnRowGUI(Rect rowRect, TreeViewItem tvItem, int row, bool selected, bool focused)
			{
				this.k_IndentWidth = 18f;
				this.k_FoldoutWidth = 18f;
				PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = tvItem as PackageExportTreeView.PackageExportTreeViewItem;
				bool flag = Event.current.type == EventType.Repaint;
				if (selected && flag)
				{
					TreeViewGUI.s_Styles.selectionStyle.Draw(rowRect, false, false, true, focused);
				}
				if (this.m_TreeView.data.IsExpandable(tvItem))
				{
					this.DoFoldout(rowRect, tvItem, row);
				}
				Rect toggleRect = new Rect(this.k_BaseIndent + (float)tvItem.depth * base.indentWidth + this.k_FoldoutWidth, rowRect.y, 18f, rowRect.height);
				this.DoToggle(packageExportTreeViewItem, toggleRect);
				using (new EditorGUI.DisabledScope(packageExportTreeViewItem.item == null))
				{
					Rect contentRect = new Rect(toggleRect.xMax, rowRect.y, rowRect.width, rowRect.height);
					this.DoIconAndText(packageExportTreeViewItem, contentRect, selected, focused);
				}
			}

			private static void Toggle(ExportPackageItem[] items, PackageExportTreeView.PackageExportTreeViewItem pitem, Rect toggleRect)
			{
				bool flag = pitem.enabledState > PackageExportTreeView.EnabledState.None;
				GUIStyle style = EditorStyles.toggle;
				bool flag2 = pitem.isFolder && pitem.enabledState == PackageExportTreeView.EnabledState.Mixed;
				if (flag2)
				{
					style = EditorStyles.toggleMixed;
				}
				bool flag3 = GUI.Toggle(toggleRect, flag, GUIContent.none, style);
				if (flag3 != flag)
				{
					pitem.enabledState = ((!flag3) ? PackageExportTreeView.EnabledState.None : PackageExportTreeView.EnabledState.All);
				}
			}

			private void DoToggle(PackageExportTreeView.PackageExportTreeViewItem pitem, Rect toggleRect)
			{
				EditorGUI.BeginChangeCheck();
				PackageExportTreeView.PackageExportTreeViewGUI.Toggle(this.m_PackageExportView.items, pitem, toggleRect);
				if (EditorGUI.EndChangeCheck())
				{
					if (this.m_TreeView.GetSelection().Length <= 1 || !this.m_TreeView.GetSelection().Contains(pitem.id))
					{
						this.m_TreeView.SetSelection(new int[]
						{
							pitem.id
						}, false);
						this.m_TreeView.NotifyListenersThatSelectionChanged();
					}
					if (this.itemWasToggled != null)
					{
						this.itemWasToggled(pitem);
					}
					Event.current.Use();
				}
			}

			private void DoIconAndText(PackageExportTreeView.PackageExportTreeViewItem item, Rect contentRect, bool selected, bool focused)
			{
				EditorGUIUtility.SetIconSize(new Vector2(this.k_IconWidth, this.k_IconWidth));
				GUIStyle lineStyle = TreeViewGUI.s_Styles.lineStyle;
				lineStyle.padding.left = 0;
				if (Event.current.type == EventType.Repaint)
				{
					lineStyle.Draw(contentRect, GUIContent.Temp(item.displayName, this.GetIconForItem(item)), false, false, selected, focused);
				}
				EditorGUIUtility.SetIconSize(Vector2.zero);
			}

			protected override Texture GetIconForItem(TreeViewItem tItem)
			{
				PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = tItem as PackageExportTreeView.PackageExportTreeViewItem;
				ExportPackageItem item = packageExportTreeViewItem.item;
				if (item == null || item.isFolder)
				{
					return PackageExportTreeView.PackageExportTreeViewGUI.Constants.folderIcon;
				}
				Texture cachedIcon = AssetDatabase.GetCachedIcon(item.assetPath);
				if (cachedIcon != null)
				{
					return cachedIcon;
				}
				return InternalEditorUtility.GetIconForFile(item.assetPath);
			}

			protected override void RenameEnded()
			{
			}
		}

		private class PackageExportTreeViewDataSource : TreeViewDataSource
		{
			private PackageExportTreeView m_PackageExportView;

			public PackageExportTreeViewDataSource(TreeView treeView, PackageExportTreeView view) : base(treeView)
			{
				this.m_PackageExportView = view;
				base.rootIsCollapsable = false;
				base.showRootItem = false;
			}

			public override bool IsRenamingItemAllowed(TreeViewItem item)
			{
				return false;
			}

			public override bool IsExpandable(TreeViewItem item)
			{
				return PackageExportTreeView.s_UseFoldouts && base.IsExpandable(item);
			}

			public override void FetchData()
			{
				int depth = -1;
				this.m_RootItem = new PackageExportTreeView.PackageExportTreeViewItem(null, "Assets".GetHashCode(), depth, null, "InvisibleAssetsFolder");
				bool flag = true;
				if (flag)
				{
					this.m_TreeView.state.expandedIDs.Add(this.m_RootItem.id);
				}
				ExportPackageItem[] items = this.m_PackageExportView.items;
				Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem> dictionary = new Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem>();
				for (int i = 0; i < items.Length; i++)
				{
					ExportPackageItem exportPackageItem = items[i];
					if (!PackageImport.HasInvalidCharInFilePath(exportPackageItem.assetPath))
					{
						string fileName = Path.GetFileName(exportPackageItem.assetPath);
						string directoryName = Path.GetDirectoryName(exportPackageItem.assetPath);
						TreeViewItem treeViewItem = this.EnsureFolderPath(directoryName, dictionary, flag);
						if (treeViewItem != null)
						{
							int hashCode = exportPackageItem.assetPath.GetHashCode();
							PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = new PackageExportTreeView.PackageExportTreeViewItem(exportPackageItem, hashCode, treeViewItem.depth + 1, treeViewItem, fileName);
							treeViewItem.AddChild(packageExportTreeViewItem);
							if (flag)
							{
								this.m_TreeView.state.expandedIDs.Add(hashCode);
							}
							if (exportPackageItem.isFolder)
							{
								dictionary[exportPackageItem.assetPath] = packageExportTreeViewItem;
							}
						}
					}
				}
				if (flag)
				{
					this.m_TreeView.state.expandedIDs.Sort();
				}
			}

			private TreeViewItem EnsureFolderPath(string folderPath, Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem> treeViewFolders, bool initExpandedState)
			{
				if (folderPath == string.Empty)
				{
					return this.m_RootItem;
				}
				int hashCode = folderPath.GetHashCode();
				TreeViewItem treeViewItem = TreeViewUtility.FindItem(hashCode, this.m_RootItem);
				if (treeViewItem != null)
				{
					return treeViewItem;
				}
				string[] array = folderPath.Split(new char[]
				{
					'/'
				});
				string text = string.Empty;
				TreeViewItem treeViewItem2 = this.m_RootItem;
				int num = -1;
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					if (text != string.Empty)
					{
						text += '/';
					}
					text += text2;
					if (i != 0 || !(text == "Assets"))
					{
						num++;
						hashCode = text.GetHashCode();
						PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem;
						if (treeViewFolders.TryGetValue(text, out packageExportTreeViewItem))
						{
							treeViewItem2 = packageExportTreeViewItem;
						}
						else
						{
							PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem2 = new PackageExportTreeView.PackageExportTreeViewItem(null, hashCode, num, treeViewItem2, text2);
							treeViewItem2.AddChild(packageExportTreeViewItem2);
							treeViewItem2 = packageExportTreeViewItem2;
							if (initExpandedState)
							{
								this.m_TreeView.state.expandedIDs.Add(hashCode);
							}
							treeViewFolders[text] = packageExportTreeViewItem2;
						}
					}
				}
				return treeViewItem2;
			}
		}

		private TreeView m_TreeView;

		private List<PackageExportTreeView.PackageExportTreeViewItem> m_Selection = new List<PackageExportTreeView.PackageExportTreeViewItem>();

		private static readonly bool s_UseFoldouts = true;

		private PackageExport m_PackageExport;

		public ExportPackageItem[] items
		{
			get
			{
				return this.m_PackageExport.items;
			}
		}

		public PackageExportTreeView(PackageExport packageExport, TreeViewState treeViewState, Rect startRect)
		{
			this.m_PackageExport = packageExport;
			this.m_TreeView = new TreeView(this.m_PackageExport, treeViewState);
			PackageExportTreeView.PackageExportTreeViewDataSource data = new PackageExportTreeView.PackageExportTreeViewDataSource(this.m_TreeView, this);
			PackageExportTreeView.PackageExportTreeViewGUI packageExportTreeViewGUI = new PackageExportTreeView.PackageExportTreeViewGUI(this.m_TreeView, this);
			this.m_TreeView.Init(startRect, data, packageExportTreeViewGUI, null);
			this.m_TreeView.ReloadData();
			TreeView expr_64 = this.m_TreeView;
			expr_64.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_64.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
			PackageExportTreeView.PackageExportTreeViewGUI expr_86 = packageExportTreeViewGUI;
			expr_86.itemWasToggled = (Action<PackageExportTreeView.PackageExportTreeViewItem>)Delegate.Combine(expr_86.itemWasToggled, new Action<PackageExportTreeView.PackageExportTreeViewItem>(this.ItemWasToggled));
			this.ComputeEnabledStateForFolders();
		}

		private void ComputeEnabledStateForFolders()
		{
			PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = this.m_TreeView.data.root as PackageExportTreeView.PackageExportTreeViewItem;
			this.RecursiveComputeEnabledStateForFolders(packageExportTreeViewItem, new HashSet<PackageExportTreeView.PackageExportTreeViewItem>
			{
				packageExportTreeViewItem
			});
		}

		private void RecursiveComputeEnabledStateForFolders(PackageExportTreeView.PackageExportTreeViewItem pitem, HashSet<PackageExportTreeView.PackageExportTreeViewItem> done)
		{
			if (!pitem.isFolder)
			{
				return;
			}
			if (pitem.hasChildren)
			{
				foreach (TreeViewItem current in pitem.children)
				{
					this.RecursiveComputeEnabledStateForFolders(current as PackageExportTreeView.PackageExportTreeViewItem, done);
				}
			}
			if (!done.Contains(pitem))
			{
				PackageExportTreeView.EnabledState folderChildrenEnabledState = this.GetFolderChildrenEnabledState(pitem);
				pitem.enabledState = folderChildrenEnabledState;
				if (folderChildrenEnabledState == PackageExportTreeView.EnabledState.Mixed)
				{
					done.Add(pitem);
					for (PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = pitem.parent as PackageExportTreeView.PackageExportTreeViewItem; packageExportTreeViewItem != null; packageExportTreeViewItem = (packageExportTreeViewItem.parent as PackageExportTreeView.PackageExportTreeViewItem))
					{
						if (!done.Contains(packageExportTreeViewItem))
						{
							packageExportTreeViewItem.enabledState = PackageExportTreeView.EnabledState.Mixed;
							done.Add(packageExportTreeViewItem);
						}
					}
				}
			}
		}

		private PackageExportTreeView.EnabledState GetFolderChildrenEnabledState(PackageExportTreeView.PackageExportTreeViewItem folder)
		{
			if (!folder.isFolder)
			{
				Debug.LogError("Should be a folder item!");
			}
			if (!folder.hasChildren)
			{
				return PackageExportTreeView.EnabledState.None;
			}
			PackageExportTreeView.EnabledState enabledState = PackageExportTreeView.EnabledState.NotSet;
			PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = folder.children[0] as PackageExportTreeView.PackageExportTreeViewItem;
			PackageExportTreeView.EnabledState enabledState2 = packageExportTreeViewItem.enabledState;
			for (int i = 1; i < folder.children.Count; i++)
			{
				PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem2 = folder.children[i] as PackageExportTreeView.PackageExportTreeViewItem;
				if (enabledState2 != packageExportTreeViewItem2.enabledState)
				{
					enabledState = PackageExportTreeView.EnabledState.Mixed;
					break;
				}
			}
			if (enabledState == PackageExportTreeView.EnabledState.NotSet)
			{
				enabledState = ((enabledState2 != PackageExportTreeView.EnabledState.All) ? PackageExportTreeView.EnabledState.None : PackageExportTreeView.EnabledState.All);
			}
			return enabledState;
		}

		private void SelectionChanged(int[] selectedIDs)
		{
			this.m_Selection = new List<PackageExportTreeView.PackageExportTreeViewItem>();
			List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
			foreach (TreeViewItem current in rows)
			{
				if (selectedIDs.Contains(current.id))
				{
					PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = current as PackageExportTreeView.PackageExportTreeViewItem;
					if (packageExportTreeViewItem != null)
					{
						this.m_Selection.Add(packageExportTreeViewItem);
					}
				}
			}
		}

		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.m_TreeView.OnGUI(rect, controlID);
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space && this.m_Selection != null && this.m_Selection.Count > 0 && GUIUtility.keyboardControl == controlID)
			{
				PackageExportTreeView.EnabledState enabledState = (this.m_Selection[0].enabledState == PackageExportTreeView.EnabledState.All) ? PackageExportTreeView.EnabledState.None : PackageExportTreeView.EnabledState.All;
				this.m_Selection[0].enabledState = enabledState;
				this.ItemWasToggled(this.m_Selection[0]);
				Event.current.Use();
			}
		}

		public void SetAllEnabled(PackageExportTreeView.EnabledState enabled)
		{
			this.EnableChildrenRecursive(this.m_TreeView.data.root, enabled);
			this.ComputeEnabledStateForFolders();
		}

		private void ItemWasToggled(PackageExportTreeView.PackageExportTreeViewItem pitem)
		{
			if (this.m_Selection.Count <= 1)
			{
				this.EnableChildrenRecursive(pitem, pitem.enabledState);
			}
			else
			{
				foreach (PackageExportTreeView.PackageExportTreeViewItem current in this.m_Selection)
				{
					current.enabledState = pitem.enabledState;
				}
			}
			this.ComputeEnabledStateForFolders();
		}

		private void EnableChildrenRecursive(TreeViewItem parentItem, PackageExportTreeView.EnabledState enabled)
		{
			if (!parentItem.hasChildren)
			{
				return;
			}
			foreach (TreeViewItem current in parentItem.children)
			{
				PackageExportTreeView.PackageExportTreeViewItem packageExportTreeViewItem = current as PackageExportTreeView.PackageExportTreeViewItem;
				packageExportTreeViewItem.enabledState = enabled;
				this.EnableChildrenRecursive(packageExportTreeViewItem, enabled);
			}
		}
	}
}
