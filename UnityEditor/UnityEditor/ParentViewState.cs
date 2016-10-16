using System;

namespace UnityEditor
{
	[Serializable]
	internal class ParentViewState
	{
		public ListViewState lv;

		public int selectedFolder = -1;

		public int selectedFile = -1;

		public int initialSelectedItem = -1;

		public ParentViewFolder[] folders = new ParentViewFolder[0];

		public bool[] selectedItems;

		public int GetLineCount()
		{
			int num = 0;
			for (int i = 0; i < this.folders.Length; i++)
			{
				num += this.folders[i].files.Length + 1;
			}
			return num;
		}

		public bool HasTrue()
		{
			for (int i = 0; i < this.selectedItems.Length; i++)
			{
				if (this.selectedItems[i])
				{
					return true;
				}
			}
			return false;
		}

		public void SetLineCount()
		{
			this.lv.totalRows = this.GetLineCount();
		}

		public int GetFoldersCount()
		{
			return this.folders.Length;
		}

		public void ClearSelection()
		{
			for (int i = 0; i < this.selectedItems.Length; i++)
			{
				this.selectedItems[i] = false;
			}
			this.initialSelectedItem = -1;
		}

		internal static int IndexOf(ParentViewFolder[] foldersFrom, string lfname)
		{
			for (int i = 0; i < foldersFrom.Length; i++)
			{
				if (string.Compare(foldersFrom[i].name, lfname, true) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		internal static int IndexOf(ParentViewFile[] filesFrom, string lfname)
		{
			for (int i = 0; i < filesFrom.Length; i++)
			{
				if (string.Compare(filesFrom[i].name, lfname, true) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		internal static int CompareViewFolder(ParentViewFolder p1, ParentViewFolder p2)
		{
			return string.Compare(p1.name, p2.name, true);
		}

		internal static int CompareViewFile(ParentViewFile p1, ParentViewFile p2)
		{
			return string.Compare(p1.name, p2.name, true);
		}

		private void AddAssetItem(string guid, string pathName, bool isDir, ChangeFlags changeFlags, int changeset)
		{
			if (pathName == string.Empty)
			{
				return;
			}
			if (isDir)
			{
				string text = ParentViewFolder.MakeNiceName(pathName);
				int num = ParentViewState.IndexOf(this.folders, text);
				if (num == -1)
				{
					ParentViewFolder item = new ParentViewFolder(text, guid, changeFlags);
					ArrayUtility.Add<ParentViewFolder>(ref this.folders, item);
				}
				else
				{
					this.folders[num].changeFlags = changeFlags;
					this.folders[num].guid = guid;
				}
			}
			else
			{
				string text2 = ParentViewFolder.MakeNiceName(FileUtil.DeleteLastPathNameComponent(pathName));
				string text3 = pathName.Substring(pathName.LastIndexOf("/") + 1);
				int num2 = ParentViewState.IndexOf(this.folders, text2);
				ParentViewFolder parentViewFolder;
				if (num2 == -1)
				{
					parentViewFolder = new ParentViewFolder(text2, AssetServer.GetParentGUID(guid, changeset));
					ArrayUtility.Add<ParentViewFolder>(ref this.folders, parentViewFolder);
				}
				else
				{
					parentViewFolder = this.folders[num2];
				}
				num2 = ParentViewState.IndexOf(parentViewFolder.files, text3);
				if (num2 != -1)
				{
					if ((parentViewFolder.files[num2].changeFlags & ChangeFlags.Deleted) == ChangeFlags.None)
					{
						parentViewFolder.files[num2].guid = guid;
						parentViewFolder.files[num2].changeFlags = changeFlags;
					}
					return;
				}
				ArrayUtility.Add<ParentViewFile>(ref parentViewFolder.files, new ParentViewFile(text3, guid, changeFlags));
			}
		}

		public void AddAssetItems(AssetsItem[] assets)
		{
			for (int i = 0; i < assets.Length; i++)
			{
				AssetsItem assetsItem = assets[i];
				this.AddAssetItem(assetsItem.guid, assetsItem.pathName, assetsItem.assetIsDir != 0, (ChangeFlags)assetsItem.changeFlags, -1);
			}
			Array.Sort<ParentViewFolder>(this.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
			for (int j = 0; j < this.folders.Length; j++)
			{
				Array.Sort<ParentViewFile>(this.folders[j].files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
			}
		}

		public void AddAssetItems(Changeset assets)
		{
			ChangesetItem[] items = assets.items;
			for (int i = 0; i < items.Length; i++)
			{
				ChangesetItem changesetItem = items[i];
				this.AddAssetItem(changesetItem.guid, changesetItem.fullPath, changesetItem.assetIsDir != 0, changesetItem.changeFlags, assets.changeset);
			}
			Array.Sort<ParentViewFolder>(this.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
			for (int j = 0; j < this.folders.Length; j++)
			{
				Array.Sort<ParentViewFile>(this.folders[j].files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
			}
		}

		public void AddAssetItems(DeletedAsset[] assets)
		{
			for (int i = 0; i < assets.Length; i++)
			{
				DeletedAsset deletedAsset = assets[i];
				this.AddAssetItem(deletedAsset.guid, deletedAsset.fullPath, deletedAsset.assetIsDir != 0, ChangeFlags.None, -1);
			}
			Array.Sort<ParentViewFolder>(this.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
			for (int j = 0; j < this.folders.Length; j++)
			{
				Array.Sort<ParentViewFile>(this.folders[j].files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
			}
		}

		public void Clear()
		{
			this.folders = new ParentViewFolder[0];
			this.selectedFolder = -1;
			this.selectedFile = -1;
			this.initialSelectedItem = -1;
		}

		public bool NextFileFolder(ref int folder, ref int file)
		{
			if (folder >= this.folders.Length)
			{
				return false;
			}
			ParentViewFolder parentViewFolder = this.folders[folder];
			if (file >= parentViewFolder.files.Length - 1)
			{
				folder++;
				file = -1;
				if (folder >= this.folders.Length)
				{
					return false;
				}
			}
			else
			{
				file++;
			}
			return true;
		}

		public bool IndexToFolderAndFile(int index, ref int folder, ref int file)
		{
			folder = 0;
			file = -1;
			for (int i = 0; i < index; i++)
			{
				if (!this.NextFileFolder(ref folder, ref file))
				{
					return false;
				}
			}
			return true;
		}
	}
}
