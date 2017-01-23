using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class PackageImportTreeView
	{
		public enum EnabledState
		{
			NotSet = -1,
			None,
			All,
			Mixed
		}

		private class PackageImportTreeViewItem : TreeViewItem
		{
			private PackageImportTreeView.EnabledState m_EnableState;

			public ImportPackageItem item
			{
				get;
				set;
			}

			public PackageImportTreeView.EnabledState enableState
			{
				get
				{
					return this.m_EnableState;
				}
				set
				{
					if (this.item == null || !this.item.projectAsset)
					{
						this.m_EnableState = value;
						if (this.item != null)
						{
							this.item.enabledStatus = (int)value;
						}
					}
				}
			}

			public PackageImportTreeViewItem(ImportPackageItem itemIn, int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
			{
				this.item = itemIn;
				if (this.item == null)
				{
					this.m_EnableState = PackageImportTreeView.EnabledState.All;
				}
				else
				{
					this.m_EnableState = (PackageImportTreeView.EnabledState)this.item.enabledStatus;
				}
			}
		}

		private class PackageImportTreeViewGUI : TreeViewGUI
		{
			internal static class Constants
			{
				public static Texture2D folderIcon;

				public static GUIContent badgeNew;

				public static GUIContent badgeDelete;

				public static GUIContent badgeWarn;

				public static GUIContent badgeChange;

				public static GUIStyle paddinglessStyle;

				static Constants()
				{
					PackageImportTreeView.PackageImportTreeViewGUI.Constants.folderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
					PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeNew = EditorGUIUtility.IconContent("AS Badge New", "|This is a new Asset");
					PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeDelete = EditorGUIUtility.IconContent("AS Badge Delete", "|These files will be deleted!");
					PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeWarn = EditorGUIUtility.IconContent("console.warnicon", "|Warning: File exists in project, but with different GUID. Will override existing asset which may be undesired.");
					PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeChange = EditorGUIUtility.IconContent("playLoopOff", "|This file is new or has changed.");
					PackageImportTreeView.PackageImportTreeViewGUI.Constants.paddinglessStyle = new GUIStyle();
					PackageImportTreeView.PackageImportTreeViewGUI.Constants.paddinglessStyle.padding = new RectOffset(0, 0, 0, 0);
				}
			}

			public Action<PackageImportTreeView.PackageImportTreeViewItem> itemWasToggled;

			private PackageImportTreeView m_PackageImportView;

			protected float k_FoldoutWidth = 12f;

			public int showPreviewForID
			{
				get;
				set;
			}

			public PackageImportTreeViewGUI(TreeViewController treeView, PackageImportTreeView view) : base(treeView)
			{
				this.m_PackageImportView = view;
				this.k_BaseIndent = 4f;
				if (!PackageImportTreeView.s_UseFoldouts)
				{
					this.k_FoldoutWidth = 0f;
				}
			}

			public override void OnRowGUI(Rect rowRect, TreeViewItem tvItem, int row, bool selected, bool focused)
			{
				this.k_IndentWidth = 18f;
				this.k_FoldoutWidth = 18f;
				PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = tvItem as PackageImportTreeView.PackageImportTreeViewItem;
				ImportPackageItem item = packageImportTreeViewItem.item;
				bool flag = Event.current.type == EventType.Repaint;
				if (selected && flag)
				{
					TreeViewGUI.s_Styles.selectionStyle.Draw(rowRect, false, false, true, focused);
				}
				bool flag2 = item != null;
				bool flag3 = item == null || item.isFolder;
				bool flag4 = item != null && item.assetChanged;
				bool flag5 = item != null && item.pathConflict;
				bool flag6 = item == null || item.exists;
				bool flag7 = item != null && item.projectAsset;
				bool doReInstall = this.m_PackageImportView.doReInstall;
				if (this.m_TreeView.data.IsExpandable(tvItem))
				{
					this.DoFoldout(rowRect, tvItem, row);
				}
				Rect toggleRect = new Rect(this.k_BaseIndent + (float)tvItem.depth * base.indentWidth + this.k_FoldoutWidth, rowRect.y, 18f, rowRect.height);
				if ((flag3 && !flag7) || (flag2 && !flag7 && (flag4 || doReInstall)))
				{
					this.DoToggle(packageImportTreeViewItem, toggleRect);
				}
				using (new EditorGUI.DisabledScope(!flag2 || flag7))
				{
					Rect contentRect = new Rect(toggleRect.xMax, rowRect.y, rowRect.width, rowRect.height);
					this.DoIconAndText(tvItem, contentRect, selected, focused);
					this.DoPreviewPopup(packageImportTreeViewItem, rowRect);
					if (flag && flag2 && flag5)
					{
						Rect position = new Rect(rowRect.xMax - 58f, rowRect.y, rowRect.height, rowRect.height);
						EditorGUIUtility.SetIconSize(new Vector2(rowRect.height, rowRect.height));
						GUI.Label(position, PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeWarn);
						EditorGUIUtility.SetIconSize(Vector2.zero);
					}
					if (flag && flag2 && !flag6 && !flag5)
					{
						Texture image = PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeNew.image;
						Rect position2 = new Rect(rowRect.xMax - (float)image.width - 6f, rowRect.y + (rowRect.height - (float)image.height) / 2f, (float)image.width, (float)image.height);
						GUI.Label(position2, PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeNew, PackageImportTreeView.PackageImportTreeViewGUI.Constants.paddinglessStyle);
					}
					if (flag && doReInstall && flag7)
					{
						Texture image2 = PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeDelete.image;
						Rect position3 = new Rect(rowRect.xMax - (float)image2.width - 6f, rowRect.y + (rowRect.height - (float)image2.height) / 2f, (float)image2.width, (float)image2.height);
						GUI.Label(position3, PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeDelete, PackageImportTreeView.PackageImportTreeViewGUI.Constants.paddinglessStyle);
					}
					if (flag && flag2 && (flag6 || flag5) && flag4)
					{
						Texture image3 = PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeChange.image;
						Rect position4 = new Rect(rowRect.xMax - (float)image3.width - 6f, rowRect.y, rowRect.height, rowRect.height);
						GUI.Label(position4, PackageImportTreeView.PackageImportTreeViewGUI.Constants.badgeChange, PackageImportTreeView.PackageImportTreeViewGUI.Constants.paddinglessStyle);
					}
				}
			}

			private static void Toggle(ImportPackageItem[] items, PackageImportTreeView.PackageImportTreeViewItem pitem, Rect toggleRect)
			{
				bool flag = pitem.enableState > PackageImportTreeView.EnabledState.None;
				bool flag2 = pitem.item == null || pitem.item.isFolder;
				GUIStyle style = EditorStyles.toggle;
				bool flag3 = flag2 && pitem.enableState == PackageImportTreeView.EnabledState.Mixed;
				if (flag3)
				{
					style = EditorStyles.toggleMixed;
				}
				bool flag4 = GUI.Toggle(toggleRect, flag, GUIContent.none, style);
				if (flag4 != flag)
				{
					pitem.enableState = ((!flag4) ? PackageImportTreeView.EnabledState.None : PackageImportTreeView.EnabledState.All);
				}
			}

			private void DoToggle(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect toggleRect)
			{
				EditorGUI.BeginChangeCheck();
				PackageImportTreeView.PackageImportTreeViewGUI.Toggle(this.m_PackageImportView.packageItems, pitem, toggleRect);
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

			private void DoPreviewPopup(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect rowRect)
			{
				ImportPackageItem item = pitem.item;
				if (item != null)
				{
					if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition) && !PopupWindowWithoutFocus.IsVisible())
					{
						this.showPreviewForID = pitem.id;
					}
					if (pitem.id == this.showPreviewForID && Event.current.type != EventType.Layout)
					{
						this.showPreviewForID = 0;
						if (!string.IsNullOrEmpty(item.previewPath))
						{
							Texture2D preview = PackageImport.GetPreview(item.previewPath);
							Rect activatorRect = rowRect;
							activatorRect.width = EditorGUIUtility.currentViewWidth;
							PopupWindowWithoutFocus.Show(activatorRect, new PackageImportTreeView.PreviewPopup(preview), new PopupLocationHelper.PopupLocation[]
							{
								PopupLocationHelper.PopupLocation.Right,
								PopupLocationHelper.PopupLocation.Left,
								PopupLocationHelper.PopupLocation.Below
							});
						}
					}
				}
			}

			private void DoIconAndText(TreeViewItem item, Rect contentRect, bool selected, bool focused)
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

			protected override Texture GetIconForItem(TreeViewItem tvItem)
			{
				PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = tvItem as PackageImportTreeView.PackageImportTreeViewItem;
				ImportPackageItem item = packageImportTreeViewItem.item;
				Texture result;
				if (item == null || item.isFolder)
				{
					result = PackageImportTreeView.PackageImportTreeViewGUI.Constants.folderIcon;
				}
				else
				{
					Texture cachedIcon = AssetDatabase.GetCachedIcon(item.destinationAssetPath);
					if (cachedIcon != null)
					{
						result = cachedIcon;
					}
					else
					{
						result = InternalEditorUtility.GetIconForFile(item.destinationAssetPath);
					}
				}
				return result;
			}

			protected override void RenameEnded()
			{
			}
		}

		private class PackageImportTreeViewDataSource : TreeViewDataSource
		{
			private PackageImportTreeView m_PackageImportView;

			public PackageImportTreeViewDataSource(TreeViewController treeView, PackageImportTreeView view) : base(treeView)
			{
				this.m_PackageImportView = view;
				base.rootIsCollapsable = false;
				base.showRootItem = false;
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
				int depth = -1;
				this.m_RootItem = new PackageImportTreeView.PackageImportTreeViewItem(null, "Assets".GetHashCode(), depth, null, "InvisibleAssetsFolder");
				bool flag = true;
				if (flag)
				{
					this.m_TreeView.state.expandedIDs.Add(this.m_RootItem.id);
				}
				ImportPackageItem[] packageItems = this.m_PackageImportView.packageItems;
				Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> dictionary = new Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem>();
				for (int i = 0; i < packageItems.Length; i++)
				{
					ImportPackageItem importPackageItem = packageItems[i];
					if (!PackageImport.HasInvalidCharInFilePath(importPackageItem.destinationAssetPath))
					{
						string fileName = Path.GetFileName(importPackageItem.destinationAssetPath);
						string directoryName = Path.GetDirectoryName(importPackageItem.destinationAssetPath);
						TreeViewItem treeViewItem = this.EnsureFolderPath(directoryName, dictionary, flag);
						if (treeViewItem != null)
						{
							int hashCode = importPackageItem.destinationAssetPath.GetHashCode();
							PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = new PackageImportTreeView.PackageImportTreeViewItem(importPackageItem, hashCode, treeViewItem.depth + 1, treeViewItem, fileName);
							treeViewItem.AddChild(packageImportTreeViewItem);
							if (flag)
							{
								this.m_TreeView.state.expandedIDs.Add(hashCode);
							}
							if (importPackageItem.isFolder)
							{
								dictionary[importPackageItem.destinationAssetPath] = packageImportTreeViewItem;
							}
						}
					}
				}
				if (flag)
				{
					this.m_TreeView.state.expandedIDs.Sort();
				}
			}

			private TreeViewItem EnsureFolderPath(string folderPath, Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> treeViewFolders, bool initExpandedState)
			{
				TreeViewItem result;
				if (folderPath == "")
				{
					result = this.m_RootItem;
				}
				else
				{
					int hashCode = folderPath.GetHashCode();
					TreeViewItem treeViewItem = TreeViewUtility.FindItem(hashCode, this.m_RootItem);
					if (treeViewItem != null)
					{
						result = treeViewItem;
					}
					else
					{
						string[] array = folderPath.Split(new char[]
						{
							'/'
						});
						string text = "";
						TreeViewItem treeViewItem2 = this.m_RootItem;
						int num = -1;
						for (int i = 0; i < array.Length; i++)
						{
							string text2 = array[i];
							if (text != "")
							{
								text += '/';
							}
							text += text2;
							if (i != 0 || !(text == "Assets"))
							{
								num++;
								hashCode = text.GetHashCode();
								PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem;
								if (treeViewFolders.TryGetValue(text, out packageImportTreeViewItem))
								{
									treeViewItem2 = packageImportTreeViewItem;
								}
								else
								{
									PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem2 = new PackageImportTreeView.PackageImportTreeViewItem(null, hashCode, num, treeViewItem2, text2);
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
						result = treeViewItem2;
					}
				}
				return result;
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

		private TreeViewController m_TreeView;

		private List<PackageImportTreeView.PackageImportTreeViewItem> m_Selection = new List<PackageImportTreeView.PackageImportTreeViewItem>();

		private static readonly bool s_UseFoldouts = true;

		private PackageImport m_PackageImport;

		public bool canReInstall
		{
			get
			{
				return this.m_PackageImport.canReInstall;
			}
		}

		public bool doReInstall
		{
			get
			{
				return this.m_PackageImport.doReInstall;
			}
		}

		public ImportPackageItem[] packageItems
		{
			get
			{
				return this.m_PackageImport.packageItems;
			}
		}

		public PackageImportTreeView(PackageImport packageImport, TreeViewState treeViewState, Rect startRect)
		{
			this.m_PackageImport = packageImport;
			this.m_TreeView = new TreeViewController(this.m_PackageImport, treeViewState);
			PackageImportTreeView.PackageImportTreeViewDataSource data = new PackageImportTreeView.PackageImportTreeViewDataSource(this.m_TreeView, this);
			PackageImportTreeView.PackageImportTreeViewGUI packageImportTreeViewGUI = new PackageImportTreeView.PackageImportTreeViewGUI(this.m_TreeView, this);
			this.m_TreeView.Init(startRect, data, packageImportTreeViewGUI, null);
			this.m_TreeView.ReloadData();
			TreeViewController expr_65 = this.m_TreeView;
			expr_65.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_65.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
			PackageImportTreeView.PackageImportTreeViewGUI expr_87 = packageImportTreeViewGUI;
			expr_87.itemWasToggled = (Action<PackageImportTreeView.PackageImportTreeViewItem>)Delegate.Combine(expr_87.itemWasToggled, new Action<PackageImportTreeView.PackageImportTreeViewItem>(this.ItemWasToggled));
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
			if (pitem.item == null || pitem.item.isFolder)
			{
				if (pitem.hasChildren)
				{
					foreach (TreeViewItem current in pitem.children)
					{
						this.RecursiveComputeEnabledStateForFolders(current as PackageImportTreeView.PackageImportTreeViewItem, done);
					}
				}
				if (!done.Contains(pitem))
				{
					PackageImportTreeView.EnabledState folderChildrenEnabledState = this.GetFolderChildrenEnabledState(pitem);
					pitem.enableState = folderChildrenEnabledState;
					if (folderChildrenEnabledState == PackageImportTreeView.EnabledState.Mixed)
					{
						done.Add(pitem);
						for (PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = pitem.parent as PackageImportTreeView.PackageImportTreeViewItem; packageImportTreeViewItem != null; packageImportTreeViewItem = (packageImportTreeViewItem.parent as PackageImportTreeView.PackageImportTreeViewItem))
						{
							if (!done.Contains(packageImportTreeViewItem))
							{
								packageImportTreeViewItem.enableState = PackageImportTreeView.EnabledState.Mixed;
								done.Add(packageImportTreeViewItem);
							}
						}
					}
				}
			}
		}

		private bool ItemShouldBeConsideredForEnabledCheck(PackageImportTreeView.PackageImportTreeViewItem pitem)
		{
			bool result;
			if (pitem == null)
			{
				result = false;
			}
			else if (pitem.item == null)
			{
				result = true;
			}
			else
			{
				ImportPackageItem item = pitem.item;
				result = (!item.projectAsset && (item.isFolder || item.assetChanged || this.doReInstall));
			}
			return result;
		}

		private PackageImportTreeView.EnabledState GetFolderChildrenEnabledState(PackageImportTreeView.PackageImportTreeViewItem folder)
		{
			if (folder.item != null && !folder.item.isFolder)
			{
				Debug.LogError("Should be a folder item!");
			}
			PackageImportTreeView.EnabledState result;
			if (!folder.hasChildren)
			{
				result = PackageImportTreeView.EnabledState.None;
			}
			else
			{
				PackageImportTreeView.EnabledState enabledState = PackageImportTreeView.EnabledState.NotSet;
				int i;
				for (i = 0; i < folder.children.Count; i++)
				{
					PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = folder.children[i] as PackageImportTreeView.PackageImportTreeViewItem;
					if (this.ItemShouldBeConsideredForEnabledCheck(packageImportTreeViewItem))
					{
						enabledState = packageImportTreeViewItem.enableState;
						break;
					}
				}
				for (i++; i < folder.children.Count; i++)
				{
					PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem2 = folder.children[i] as PackageImportTreeView.PackageImportTreeViewItem;
					if (this.ItemShouldBeConsideredForEnabledCheck(packageImportTreeViewItem2))
					{
						if (enabledState != packageImportTreeViewItem2.enableState)
						{
							enabledState = PackageImportTreeView.EnabledState.Mixed;
							break;
						}
					}
				}
				if (enabledState == PackageImportTreeView.EnabledState.NotSet)
				{
					result = PackageImportTreeView.EnabledState.None;
				}
				else
				{
					result = enabledState;
				}
			}
			return result;
		}

		private void SelectionChanged(int[] selectedIDs)
		{
			this.m_Selection = new List<PackageImportTreeView.PackageImportTreeViewItem>();
			IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
			foreach (TreeViewItem current in rows)
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
			ImportPackageItem item = this.m_Selection[0].item;
			if (this.m_Selection.Count == 1 && item != null && !string.IsNullOrEmpty(item.previewPath))
			{
				PackageImportTreeView.PackageImportTreeViewGUI packageImportTreeViewGUI = this.m_TreeView.gui as PackageImportTreeView.PackageImportTreeViewGUI;
				packageImportTreeViewGUI.showPreviewForID = this.m_Selection[0].id;
			}
			else
			{
				PopupWindowWithoutFocus.Hide();
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
				PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = this.m_Selection[0];
				if (packageImportTreeViewItem != null)
				{
					PackageImportTreeView.EnabledState enableState = (packageImportTreeViewItem.enableState != PackageImportTreeView.EnabledState.None) ? PackageImportTreeView.EnabledState.None : PackageImportTreeView.EnabledState.All;
					packageImportTreeViewItem.enableState = enableState;
					this.ItemWasToggled(this.m_Selection[0]);
				}
				Event.current.Use();
			}
		}

		public void SetAllEnabled(PackageImportTreeView.EnabledState state)
		{
			this.EnableChildrenRecursive(this.m_TreeView.data.root, state);
			this.ComputeEnabledStateForFolders();
		}

		private void ItemWasToggled(PackageImportTreeView.PackageImportTreeViewItem pitem)
		{
			if (this.m_Selection.Count <= 1)
			{
				this.EnableChildrenRecursive(pitem, pitem.enableState);
			}
			else
			{
				foreach (PackageImportTreeView.PackageImportTreeViewItem current in this.m_Selection)
				{
					current.enableState = pitem.enableState;
				}
			}
			this.ComputeEnabledStateForFolders();
		}

		private void EnableChildrenRecursive(TreeViewItem parentItem, PackageImportTreeView.EnabledState state)
		{
			if (parentItem.hasChildren)
			{
				foreach (TreeViewItem current in parentItem.children)
				{
					PackageImportTreeView.PackageImportTreeViewItem packageImportTreeViewItem = current as PackageImportTreeView.PackageImportTreeViewItem;
					packageImportTreeViewItem.enableState = state;
					this.EnableChildrenRecursive(packageImportTreeViewItem, state);
				}
			}
		}
	}
}
