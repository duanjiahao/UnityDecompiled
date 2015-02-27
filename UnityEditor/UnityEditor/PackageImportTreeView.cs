using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class PackageImportTreeView
	{
		public enum Amount
		{
			NotSet = -1,
			None,
			All,
			Mixed
		}
		private class PackageImportTreeViewItem : TreeViewItem
		{
			public AssetsItem item
			{
				get;
				set;
			}
			public bool isFolder
			{
				get;
				set;
			}
			public PackageImportTreeViewItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
			{
				this.item = this.item;
			}
		}
		private class PackageImportTreeViewGUI : TreeViewGUI
		{
			private static Texture2D folderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
			public Action<PackageImportTreeView.PackageImportTreeViewItem> itemWasToggled;
			public int showPreviewForID
			{
				get;
				set;
			}
			public PackageImportTreeViewGUI(TreeView treeView) : base(treeView)
			{
				this.k_BaseIndent = 4f;
				if (!PackageImportTreeView.s_UseFoldouts)
				{
					this.k_FoldoutWidth = 0f;
				}
			}
			public override Rect OnRowGUI(TreeViewItem node, int row, float rowWidth, bool selected, bool focused)
			{
				Rect rect = new Rect(0f, (float)row * this.k_LineHeight, rowWidth, this.k_LineHeight);
				this.DoNodeGUI(rect, node, selected, focused, false);
				PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = node as PackageImportTreeView.PackageImportTreeViewItem;
				if (packageImportTreeViewItem != null)
				{
					Rect toggleRect = new Rect(2f, rect.y, rect.height, rect.height);
					EditorGUI.BeginChangeCheck();
					PackageImportTreeView.PackageImportTreeViewGUI.Toggle(packageImportTreeViewItem, toggleRect);
					if (EditorGUI.EndChangeCheck())
					{
						if (this.m_TreeView.GetSelection().Length <= 1 || !this.m_TreeView.GetSelection().Contains(packageImportTreeViewItem.id))
						{
							this.m_TreeView.SetSelection(new int[]
							{
								packageImportTreeViewItem.id
							}, false);
							this.m_TreeView.NotifyListenersThatSelectionChanged();
						}
						if (this.itemWasToggled != null)
						{
							this.itemWasToggled(packageImportTreeViewItem);
						}
						Event.current.Use();
					}
					if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition) && !PopupWindowWithoutFocus.IsVisible())
					{
						this.showPreviewForID = packageImportTreeViewItem.id;
					}
					if (packageImportTreeViewItem.id == this.showPreviewForID && Event.current.type != EventType.Layout)
					{
						this.showPreviewForID = 0;
						if (!string.IsNullOrEmpty(packageImportTreeViewItem.item.previewPath))
						{
							Texture2D preview = PackageImport.GetPreview(packageImportTreeViewItem.item.previewPath);
							Rect rect2 = rect;
							rect2.width = EditorGUIUtility.currentViewWidth;
							Rect arg_190_0 = rect2;
							PopupWindowContent arg_190_1 = new PackageImportTreeView.PreviewPopup(preview);
							PopupLocationHelper.PopupLocation[] expr_188 = new PopupLocationHelper.PopupLocation[3];
							expr_188[0] = PopupLocationHelper.PopupLocation.Right;
							expr_188[1] = PopupLocationHelper.PopupLocation.Left;
							PopupWindowWithoutFocus.Show(arg_190_0, arg_190_1, expr_188);
						}
					}
					if (packageImportTreeViewItem.item.exists == 0)
					{
						Texture image = ASMainWindow.badgeNew.image;
						GUI.DrawTexture(new Rect(rect.xMax - (float)image.width - 6f, rect.y + (rect.height - (float)image.height) / 2f, (float)image.width, (float)image.height), image);
					}
				}
				return rect;
			}
			private static void Toggle(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect toggleRect)
			{
				bool flag = pitem.item.enabled > 0;
				GUIStyle style = EditorStyles.toggle;
				bool flag2 = pitem.isFolder && pitem.item.enabled == 2;
				if (flag2)
				{
					style = EditorStyles.toggleMixed;
				}
				bool flag3 = GUI.Toggle(toggleRect, flag, GUIContent.none, style);
				if (flag3 != flag)
				{
					pitem.item.enabled = ((!flag3) ? 0 : 1);
				}
			}
			protected override Texture GetIconForNode(TreeViewItem item)
			{
				PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = item as PackageImportTreeView.PackageImportTreeViewItem;
				if (packageImportTreeViewItem.isFolder)
				{
					return PackageImportTreeView.PackageImportTreeViewGUI.folderIcon;
				}
				return InternalEditorUtility.GetIconForFile(packageImportTreeViewItem.item.pathName);
			}
			protected override void RenameEnded()
			{
			}
		}
		private class PackageImportTreeViewDataSource : TreeViewDataSource
		{
			public AssetsItem[] m_AssetItems;
			public List<string> m_EnabledFolders;
			public PackageImportTreeViewDataSource(TreeView treeView, AssetsItem[] assetItems, List<string> enabledFolders) : base(treeView)
			{
				this.m_AssetItems = assetItems;
				this.m_EnabledFolders = enabledFolders;
				base.rootIsCollapsable = false;
				base.showRootNode = false;
			}
			public override bool IsRenamingItemAllowed(TreeViewItem item)
			{
				return false;
			}
			public override bool IsExpandable(TreeViewItem item)
			{
				return PackageImportTreeView.s_UseFoldouts && base.IsExpandable(item);
			}
			public override void FetchData()
			{
				this.m_RootItem = new PackageImportTreeView.PackageImportTreeViewItem("Assets".GetHashCode(), 0, null, "InvisibleAssetsFolder");
				((PackageImportTreeView.PackageImportTreeViewItem)this.m_RootItem).isFolder = true;
				bool flag = this.m_TreeView.state.expandedIDs.Count == 0;
				if (flag)
				{
					this.m_TreeView.state.expandedIDs.Add(this.m_RootItem.id);
				}
				Dictionary<string, AssetsItem> dictionary = new Dictionary<string, AssetsItem>();
				AssetsItem[] assetItems = this.m_AssetItems;
				for (int i = 0; i < assetItems.Length; i++)
				{
					AssetsItem assetsItem = assetItems[i];
					if (assetsItem.assetIsDir == 1)
					{
						dictionary[assetsItem.pathName] = assetsItem;
					}
				}
				Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> treeViewFolders = new Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem>();
				AssetsItem[] assetItems2 = this.m_AssetItems;
				for (int j = 0; j < assetItems2.Length; j++)
				{
					AssetsItem assetsItem2 = assetItems2[j];
					if (assetsItem2.assetIsDir != 1)
					{
						string fileName = Path.GetFileName(assetsItem2.pathName);
						string directoryName = Path.GetDirectoryName(assetsItem2.pathName);
						TreeViewItem treeViewItem = this.EnsureFolderPath(directoryName, dictionary, treeViewFolders, flag);
						if (treeViewItem != null)
						{
							int hashCode = assetsItem2.pathName.GetHashCode();
							treeViewItem.AddChild(new PackageImportTreeView.PackageImportTreeViewItem(hashCode, treeViewItem.depth + 1, treeViewItem, fileName)
							{
								item = assetsItem2
							});
						}
					}
				}
				if (flag)
				{
					this.m_TreeView.state.expandedIDs.Sort();
				}
			}
			private TreeViewItem EnsureFolderPath(string folderPath, Dictionary<string, AssetsItem> packageFolders, Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> treeViewFolders, bool initExpandedState)
			{
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
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					if (text != string.Empty)
					{
						text += '/';
					}
					text += text2;
					if (!(text == "Assets"))
					{
						hashCode = text.GetHashCode();
						PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem;
						if (treeViewFolders.TryGetValue(text, out packageImportTreeViewItem))
						{
							treeViewItem2 = packageImportTreeViewItem;
						}
						else
						{
							PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem2 = new PackageImportTreeView.PackageImportTreeViewItem(hashCode, i, treeViewItem2, text2);
							packageImportTreeViewItem2.isFolder = true;
							AssetsItem item;
							if (packageFolders.TryGetValue(text, out item))
							{
								packageImportTreeViewItem2.item = item;
							}
							if (packageImportTreeViewItem2.item == null)
							{
								packageImportTreeViewItem2.item = new AssetsItem
								{
									assetIsDir = 1,
									pathName = text,
									exportedAssetPath = text,
									enabled = (this.m_EnabledFolders != null) ? ((!this.m_EnabledFolders.Contains(text)) ? 0 : 1) : 1,
									guid = AssetDatabase.AssetPathToGUID(text),
									previewPath = string.Empty
								};
								packageImportTreeViewItem2.item.exists = ((!string.IsNullOrEmpty(packageImportTreeViewItem2.item.guid)) ? 1 : 0);
							}
							treeViewItem2.AddChild(packageImportTreeViewItem2);
							treeViewItem2 = packageImportTreeViewItem2;
							if (initExpandedState)
							{
								this.m_TreeView.state.expandedIDs.Add(hashCode);
							}
							treeViewFolders[text] = packageImportTreeViewItem2;
						}
					}
				}
				return treeViewItem2;
			}
		}
		private class PreviewPopup : PopupWindowContent
		{
			private readonly Texture2D m_Preview;
			private readonly Vector2 kPreviewSize = new Vector2(128f, 128f);
			public PreviewPopup(Texture2D preview)
			{
				this.m_Preview = preview;
			}
			public override void OnGUI(Rect rect)
			{
				PackageImport.DrawTexture(rect, this.m_Preview, false);
			}
			public override Vector2 GetWindowSize()
			{
				return this.kPreviewSize;
			}
		}
		private TreeView m_TreeView;
		private List<PackageImportTreeView.PackageImportTreeViewItem> m_Selection = new List<PackageImportTreeView.PackageImportTreeViewItem>();
		private static readonly bool s_UseFoldouts = true;
		public PackageImportTreeView(AssetsItem[] items, List<string> enabledFolders, TreeViewState treeViewState, PackageImport packageImportWindow, Rect startRect)
		{
			this.m_TreeView = new TreeView(packageImportWindow, treeViewState);
			PackageImportTreeView.PackageImportTreeViewDataSource data = new PackageImportTreeView.PackageImportTreeViewDataSource(this.m_TreeView, items, enabledFolders);
			PackageImportTreeView.PackageImportTreeViewGUI packageImportTreeViewGUI = new PackageImportTreeView.PackageImportTreeViewGUI(this.m_TreeView);
			this.m_TreeView.Init(startRect, data, packageImportTreeViewGUI, null);
			this.m_TreeView.ReloadData();
			TreeView expr_5A = this.m_TreeView;
			expr_5A.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_5A.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
			PackageImportTreeView.PackageImportTreeViewGUI expr_7C = packageImportTreeViewGUI;
			expr_7C.itemWasToggled = (Action<PackageImportTreeView.PackageImportTreeViewItem>)Delegate.Combine(expr_7C.itemWasToggled, new Action<PackageImportTreeView.PackageImportTreeViewItem>(this.ItemWasToggled));
			this.ComputeEnabledStateForFolders();
		}
		private void ComputeEnabledStateForFolders()
		{
			PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = this.m_TreeView.data.root as PackageImportTreeView.PackageImportTreeViewItem;
			this.RecursiveComputeEnabledStateForFolders(packageImportTreeViewItem, new HashSet<PackageImportTreeView.PackageImportTreeViewItem>
			{
				packageImportTreeViewItem
			});
		}
		private void RecursiveComputeEnabledStateForFolders(PackageImportTreeView.PackageImportTreeViewItem pitem, HashSet<PackageImportTreeView.PackageImportTreeViewItem> done)
		{
			if (!pitem.isFolder)
			{
				return;
			}
			if (pitem.hasChildren)
			{
				foreach (TreeViewItem current in pitem.children)
				{
					this.RecursiveComputeEnabledStateForFolders(current as PackageImportTreeView.PackageImportTreeViewItem, done);
				}
			}
			if (!done.Contains(pitem))
			{
				PackageImportTreeView.Amount folderChildrenEnabledState = this.GetFolderChildrenEnabledState(pitem);
				pitem.item.enabled = (int)folderChildrenEnabledState;
				if (folderChildrenEnabledState == PackageImportTreeView.Amount.Mixed)
				{
					done.Add(pitem);
					for (PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = pitem.parent as PackageImportTreeView.PackageImportTreeViewItem; packageImportTreeViewItem != null; packageImportTreeViewItem = (packageImportTreeViewItem.parent as PackageImportTreeView.PackageImportTreeViewItem))
					{
						if (!done.Contains(packageImportTreeViewItem))
						{
							packageImportTreeViewItem.item.enabled = 2;
							done.Add(packageImportTreeViewItem);
						}
					}
				}
			}
		}
		private PackageImportTreeView.Amount GetFolderChildrenEnabledState(PackageImportTreeView.PackageImportTreeViewItem folder)
		{
			if (!folder.isFolder)
			{
				Debug.LogError("Should be a folder item!");
			}
			if (!folder.hasChildren)
			{
				return PackageImportTreeView.Amount.None;
			}
			PackageImportTreeView.Amount amount = PackageImportTreeView.Amount.NotSet;
			PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = folder.children[0] as PackageImportTreeView.PackageImportTreeViewItem;
			int enabled = packageImportTreeViewItem.item.enabled;
			for (int i = 1; i < folder.children.Count; i++)
			{
				if (enabled != (folder.children[i] as PackageImportTreeView.PackageImportTreeViewItem).item.enabled)
				{
					amount = PackageImportTreeView.Amount.Mixed;
					break;
				}
			}
			if (amount == PackageImportTreeView.Amount.NotSet)
			{
				amount = ((enabled != 1) ? PackageImportTreeView.Amount.None : PackageImportTreeView.Amount.All);
			}
			return amount;
		}
		private void SelectionChanged(int[] selectedIDs)
		{
			this.m_Selection = new List<PackageImportTreeView.PackageImportTreeViewItem>();
			List<TreeViewItem> visibleRows = this.m_TreeView.data.GetVisibleRows();
			foreach (TreeViewItem current in visibleRows)
			{
				if (selectedIDs.Contains(current.id))
				{
					PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = current as PackageImportTreeView.PackageImportTreeViewItem;
					if (packageImportTreeViewItem != null)
					{
						this.m_Selection.Add(packageImportTreeViewItem);
					}
				}
			}
			if (this.m_Selection.Count == 1 && !string.IsNullOrEmpty(this.m_Selection[0].item.previewPath))
			{
				PackageImportTreeView.PackageImportTreeViewGUI packageImportTreeViewGUI = this.m_TreeView.gui as PackageImportTreeView.PackageImportTreeViewGUI;
				packageImportTreeViewGUI.showPreviewForID = this.m_Selection[0].id;
			}
			else
			{
				PopupWindowWithoutFocus.Hide();
			}
		}
		public AssetsItem GetSingleSelection()
		{
			if (this.m_Selection != null && this.m_Selection.Count == 1)
			{
				return this.m_Selection[0].item;
			}
			return null;
		}
		public void GetEnabledFolders(List<string> folderPaths)
		{
			this.GetEnabledFoldersRecursive(this.m_TreeView.data.root, folderPaths);
		}
		private void GetEnabledFoldersRecursive(TreeViewItem parentItem, List<string> folderPaths)
		{
			if (!parentItem.hasChildren)
			{
				return;
			}
			foreach (TreeViewItem current in parentItem.children)
			{
				PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = current as PackageImportTreeView.PackageImportTreeViewItem;
				if (packageImportTreeViewItem.isFolder && packageImportTreeViewItem.item.enabled > 0)
				{
					folderPaths.Add(packageImportTreeViewItem.item.pathName);
				}
				this.GetEnabledFoldersRecursive(packageImportTreeViewItem, folderPaths);
			}
		}
		public void OnGUI(Rect rect)
		{
			if (Event.current.type == EventType.ScrollWheel)
			{
				PopupWindowWithoutFocus.Hide();
			}
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.m_TreeView.OnGUI(rect, controlID);
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space && this.m_Selection != null && this.m_Selection.Count > 0 && GUIUtility.keyboardControl == controlID)
			{
				int enabled = (this.m_Selection[0].item.enabled != 0) ? 0 : 1;
				this.m_Selection[0].item.enabled = enabled;
				this.ItemWasToggled(this.m_Selection[0]);
				Event.current.Use();
			}
		}
		public void SetAllEnabled(int enabled)
		{
			this.EnableChildrenRecursive(this.m_TreeView.data.root, enabled);
			this.ComputeEnabledStateForFolders();
		}
		private void ItemWasToggled(PackageImportTreeView.PackageImportTreeViewItem pitem)
		{
			if (this.m_Selection.Count <= 1)
			{
				this.EnableChildrenRecursive(pitem, pitem.item.enabled);
			}
			else
			{
				foreach (PackageImportTreeView.PackageImportTreeViewItem current in this.m_Selection)
				{
					current.item.enabled = pitem.item.enabled;
				}
			}
			this.ComputeEnabledStateForFolders();
		}
		private void EnableChildrenRecursive(TreeViewItem parentItem, int enabled)
		{
			if (!parentItem.hasChildren)
			{
				return;
			}
			foreach (TreeViewItem current in parentItem.children)
			{
				PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = current as PackageImportTreeView.PackageImportTreeViewItem;
				packageImportTreeViewItem.item.enabled = enabled;
				this.EnableChildrenRecursive(packageImportTreeViewItem, enabled);
			}
		}
	}
}
