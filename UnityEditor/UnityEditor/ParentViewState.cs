using System;
using System.Runtime.CompilerServices;

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

		[CompilerGenerated]
		private static Comparison<ParentViewFolder> <>f__mg$cache0;

		[CompilerGenerated]
		private static Comparison<ParentViewFile> <>f__mg$cache1;

		[CompilerGenerated]
		private static Comparison<ParentViewFolder> <>f__mg$cache2;

		[CompilerGenerated]
		private static Comparison<ParentViewFile> <>f__mg$cache3;

		[CompilerGenerated]
		private static Comparison<ParentViewFolder> <>f__mg$cache4;

		[CompilerGenerated]
		private static Comparison<ParentViewFile> <>f__mg$cache5;

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
			bool result;
			for (int i = 0; i < this.selectedItems.Length; i++)
			{
				if (this.selectedItems[i])
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
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
			int result;
			for (int i = 0; i < foldersFrom.Length; i++)
			{
				if (string.Compare(foldersFrom[i].name, lfname, true) == 0)
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		internal static int IndexOf(ParentViewFile[] filesFrom, string lfname)
		{
			int result;
			for (int i = 0; i < filesFrom.Length; i++)
			{
				if (string.Compare(filesFrom[i].name, lfname, true) == 0)
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
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
			if (!(pathName == string.Empty))
			{
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
					}
					else
					{
						ArrayUtility.Add<ParentViewFile>(ref parentViewFolder.files, new ParentViewFile(text3, guid, changeFlags));
					}
				}
			}
		}

		public void AddAssetItems(AssetsItem[] assets)
		{
			for (int i = 0; i < assets.Length; i++)
			{
				AssetsItem assetsItem = assets[i];
				this.AddAssetItem(assetsItem.guid, assetsItem.pathName, assetsItem.assetIsDir != 0, (ChangeFlags)assetsItem.changeFlags, -1);
			}
			ParentViewFolder[] arg_66_0 = this.folders;
			if (ParentViewState.<>f__mg$cache0 == null)
			{
				ParentViewState.<>f__mg$cache0 = new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder);
			}
			Array.Sort<ParentViewFolder>(arg_66_0, ParentViewState.<>f__mg$cache0);
			for (int j = 0; j < this.folders.Length; j++)
			{
				ParentViewFile[] arg_9C_0 = this.folders[j].files;
				if (ParentViewState.<>f__mg$cache1 == null)
				{
					ParentViewState.<>f__mg$cache1 = new Comparison<ParentViewFile>(ParentViewState.CompareViewFile);
				}
				Array.Sort<ParentViewFile>(arg_9C_0, ParentViewState.<>f__mg$cache1);
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
			ParentViewFolder[] arg_70_0 = this.folders;
			if (ParentViewState.<>f__mg$cache2 == null)
			{
				ParentViewState.<>f__mg$cache2 = new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder);
			}
			Array.Sort<ParentViewFolder>(arg_70_0, ParentViewState.<>f__mg$cache2);
			for (int j = 0; j < this.folders.Length; j++)
			{
				ParentViewFile[] arg_A6_0 = this.folders[j].files;
				if (ParentViewState.<>f__mg$cache3 == null)
				{
					ParentViewState.<>f__mg$cache3 = new Comparison<ParentViewFile>(ParentViewState.CompareViewFile);
				}
				Array.Sort<ParentViewFile>(arg_A6_0, ParentViewState.<>f__mg$cache3);
			}
		}

		public void AddAssetItems(DeletedAsset[] assets)
		{
			for (int i = 0; i < assets.Length; i++)
			{
				DeletedAsset deletedAsset = assets[i];
				this.AddAssetItem(deletedAsset.guid, deletedAsset.fullPath, deletedAsset.assetIsDir != 0, ChangeFlags.None, -1);
			}
			ParentViewFolder[] arg_61_0 = this.folders;
			if (ParentViewState.<>f__mg$cache4 == null)
			{
				ParentViewState.<>f__mg$cache4 = new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder);
			}
			Array.Sort<ParentViewFolder>(arg_61_0, ParentViewState.<>f__mg$cache4);
			for (int j = 0; j < this.folders.Length; j++)
			{
				ParentViewFile[] arg_97_0 = this.folders[j].files;
				if (ParentViewState.<>f__mg$cache5 == null)
				{
					ParentViewState.<>f__mg$cache5 = new Comparison<ParentViewFile>(ParentViewState.CompareViewFile);
				}
				Array.Sort<ParentViewFile>(arg_97_0, ParentViewState.<>f__mg$cache5);
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
			bool result;
			if (folder >= this.folders.Length)
			{
				result = false;
			}
			else
			{
				ParentViewFolder parentViewFolder = this.folders[folder];
				if (file >= parentViewFolder.files.Length - 1)
				{
					folder++;
					file = -1;
					if (folder >= this.folders.Length)
					{
						result = false;
						return result;
					}
				}
				else
				{
					file++;
				}
				result = true;
			}
			return result;
		}

		public bool IndexToFolderAndFile(int index, ref int folder, ref int file)
		{
			folder = 0;
			file = -1;
			bool result;
			for (int i = 0; i < index; i++)
			{
				if (!this.NextFileFolder(ref folder, ref file))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
	}
}
