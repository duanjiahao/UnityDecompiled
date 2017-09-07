using System;
using System.Collections.Generic;
using UnityEditor.Collaboration;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	[FilePath("SearchFilters", FilePathAttribute.Location.PreferencesFolder)]
	internal class SavedSearchFilters : ScriptableSingleton<SavedSearchFilters>
	{
		[SerializeField]
		private List<SavedFilter> m_SavedFilters;

		private Action m_SavedFiltersChanged;

		private Action m_SavedFiltersInitialized;

		private bool m_AllowHierarchy = false;

		public static int AddSavedFilter(string displayName, SearchFilter filter, float previewSize)
		{
			return ScriptableSingleton<SavedSearchFilters>.instance.Add(displayName, filter, previewSize, SavedSearchFilters.GetRootInstanceID(), true);
		}

		public static int AddSavedFilterAfterInstanceID(string displayName, SearchFilter filter, float previewSize, int insertAfterID, bool addAsChild)
		{
			return ScriptableSingleton<SavedSearchFilters>.instance.Add(displayName, filter, previewSize, insertAfterID, addAsChild);
		}

		public static void RemoveSavedFilter(int instanceID)
		{
			ScriptableSingleton<SavedSearchFilters>.instance.Remove(instanceID);
		}

		public static bool IsSavedFilter(int instanceID)
		{
			return ScriptableSingleton<SavedSearchFilters>.instance.IndexOf(instanceID) >= 0;
		}

		public static int GetRootInstanceID()
		{
			return ScriptableSingleton<SavedSearchFilters>.instance.GetRoot();
		}

		public static SearchFilter GetFilter(int instanceID)
		{
			SavedFilter savedFilter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
			SearchFilter result;
			if (savedFilter != null && savedFilter.m_Filter != null)
			{
				result = ObjectCopier.DeepClone<SearchFilter>(savedFilter.m_Filter);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static int GetFilterInstanceID(string name, string searchFieldString)
		{
			int result;
			if (ScriptableSingleton<SavedSearchFilters>.instance.m_SavedFilters != null && ScriptableSingleton<SavedSearchFilters>.instance.m_SavedFilters.Count > 0)
			{
				foreach (SavedFilter current in ScriptableSingleton<SavedSearchFilters>.instance.m_SavedFilters)
				{
					if ((string.IsNullOrEmpty(name) || current.m_Name == name) && current.m_Filter.FilterToSearchFieldString() == searchFieldString)
					{
						result = current.m_ID;
						return result;
					}
				}
			}
			result = 0;
			return result;
		}

		public static float GetPreviewSize(int instanceID)
		{
			SavedFilter savedFilter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
			float result;
			if (savedFilter != null)
			{
				result = savedFilter.m_PreviewSize;
			}
			else
			{
				result = -1f;
			}
			return result;
		}

		public static string GetName(int instanceID)
		{
			SavedFilter savedFilter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
			string result;
			if (savedFilter != null)
			{
				result = savedFilter.m_Name;
			}
			else
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Could not find saved filter ",
					instanceID,
					" ",
					ScriptableSingleton<SavedSearchFilters>.instance.ToString()
				}));
				result = "";
			}
			return result;
		}

		public static void SetName(int instanceID, string name)
		{
			SavedFilter savedFilter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
			if (savedFilter != null)
			{
				savedFilter.m_Name = name;
				ScriptableSingleton<SavedSearchFilters>.instance.Changed();
			}
			else
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Could not set name of saved filter ",
					instanceID,
					" ",
					ScriptableSingleton<SavedSearchFilters>.instance.ToString()
				}));
			}
		}

		public static void UpdateExistingSavedFilter(int instanceID, SearchFilter filter, float previewSize)
		{
			ScriptableSingleton<SavedSearchFilters>.instance.UpdateFilter(instanceID, filter, previewSize);
		}

		public static TreeViewItem ConvertToTreeView()
		{
			return ScriptableSingleton<SavedSearchFilters>.instance.BuildTreeView();
		}

		public static void RefreshSavedFilters()
		{
			ScriptableSingleton<SavedSearchFilters>.instance.Changed();
		}

		public static void AddChangeListener(Action callback)
		{
			SavedSearchFilters expr_06 = ScriptableSingleton<SavedSearchFilters>.instance;
			expr_06.m_SavedFiltersChanged = (Action)Delegate.Remove(expr_06.m_SavedFiltersChanged, callback);
			SavedSearchFilters expr_21 = ScriptableSingleton<SavedSearchFilters>.instance;
			expr_21.m_SavedFiltersChanged = (Action)Delegate.Combine(expr_21.m_SavedFiltersChanged, callback);
		}

		internal static void AddInitializedListener(Action callback)
		{
			SavedSearchFilters expr_06 = ScriptableSingleton<SavedSearchFilters>.instance;
			expr_06.m_SavedFiltersInitialized = (Action)Delegate.Remove(expr_06.m_SavedFiltersInitialized, callback);
			SavedSearchFilters expr_21 = ScriptableSingleton<SavedSearchFilters>.instance;
			expr_21.m_SavedFiltersInitialized = (Action)Delegate.Combine(expr_21.m_SavedFiltersInitialized, callback);
		}

		internal static void RemoveInitializedListener(Action callback)
		{
			SavedSearchFilters expr_06 = ScriptableSingleton<SavedSearchFilters>.instance;
			expr_06.m_SavedFiltersInitialized = (Action)Delegate.Remove(expr_06.m_SavedFiltersInitialized, callback);
		}

		public static void MoveSavedFilter(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
		{
			ScriptableSingleton<SavedSearchFilters>.instance.Move(instanceID, parentInstanceID, targetInstanceID, after);
		}

		public static bool CanMoveSavedFilter(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
		{
			return ScriptableSingleton<SavedSearchFilters>.instance.IsValidMove(instanceID, parentInstanceID, targetInstanceID, after);
		}

		public static bool AllowsHierarchy()
		{
			return ScriptableSingleton<SavedSearchFilters>.instance.m_AllowHierarchy;
		}

		private bool IsValidMove(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
		{
			int num = this.IndexOf(instanceID);
			int num2 = this.IndexOf(parentInstanceID);
			int num3 = this.IndexOf(targetInstanceID);
			bool result;
			if (num < 0 || num2 < 0 || num3 < 0)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Move of a SavedFilter has invalid input: ",
					num,
					" ",
					num2,
					" ",
					num3
				}));
				result = false;
			}
			else if (instanceID == targetInstanceID)
			{
				result = false;
			}
			else
			{
				for (int i = num + 1; i < this.m_SavedFilters.Count; i++)
				{
					if (this.m_SavedFilters[i].m_Depth <= this.m_SavedFilters[num].m_Depth)
					{
						break;
					}
					if (i == num3 || i == num2)
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		private void Move(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
		{
			if (this.IsValidMove(instanceID, parentInstanceID, targetInstanceID, after))
			{
				int index = this.IndexOf(instanceID);
				int index2 = this.IndexOf(parentInstanceID);
				int num = this.IndexOf(targetInstanceID);
				SavedFilter savedFilter = this.m_SavedFilters[index];
				SavedFilter savedFilter2 = this.m_SavedFilters[index2];
				int num2 = 0;
				if (this.m_AllowHierarchy)
				{
					num2 = savedFilter2.m_Depth + 1 - savedFilter.m_Depth;
				}
				List<SavedFilter> savedFilterAndChildren = this.GetSavedFilterAndChildren(instanceID);
				this.m_SavedFilters.RemoveRange(index, savedFilterAndChildren.Count);
				foreach (SavedFilter current in savedFilterAndChildren)
				{
					current.m_Depth += num2;
				}
				num = this.IndexOf(targetInstanceID);
				if (num != -1)
				{
					if (after)
					{
						num++;
					}
					this.m_SavedFilters.InsertRange(num, savedFilterAndChildren);
				}
				this.Changed();
			}
		}

		private void UpdateFilter(int instanceID, SearchFilter filter, float previewSize)
		{
			SavedFilter savedFilter = this.Find(instanceID);
			if (savedFilter != null)
			{
				if (filter != null)
				{
					SearchFilter filter2 = ObjectCopier.DeepClone<SearchFilter>(filter);
					savedFilter.m_Filter = filter2;
				}
				savedFilter.m_PreviewSize = previewSize;
				this.Changed();
			}
			else
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Could not find saved filter ",
					instanceID,
					" ",
					ScriptableSingleton<SavedSearchFilters>.instance.ToString()
				}));
			}
		}

		private int GetNextAvailableID()
		{
			List<int> list = new List<int>();
			foreach (SavedFilter current in this.m_SavedFilters)
			{
				if (current.m_ID >= ProjectWindowUtil.k_FavoritesStartInstanceID)
				{
					list.Add(current.m_ID);
				}
			}
			list.Sort();
			int num = ProjectWindowUtil.k_FavoritesStartInstanceID;
			int result;
			for (int i = 0; i < 1000; i++)
			{
				if (list.BinarySearch(num) < 0)
				{
					result = num;
					return result;
				}
				num++;
			}
			Debug.LogError(string.Concat(new object[]
			{
				"Could not assign valid ID to saved filter ",
				DebugUtils.ListToString<int>(list),
				" ",
				num
			}));
			result = ProjectWindowUtil.k_FavoritesStartInstanceID + 1000;
			return result;
		}

		private int Add(string displayName, SearchFilter filter, float previewSize, int insertAfterInstanceID, bool addAsChild)
		{
			SearchFilter searchFilter = null;
			if (filter != null)
			{
				searchFilter = ObjectCopier.DeepClone<SearchFilter>(filter);
			}
			if (searchFilter.GetState() == SearchFilter.State.SearchingInAllAssets || searchFilter.GetState() == SearchFilter.State.SearchingInAssetStore)
			{
				searchFilter.folders = new string[0];
			}
			int num = 0;
			int result;
			if (insertAfterInstanceID != 0)
			{
				num = this.IndexOf(insertAfterInstanceID);
				if (num == -1)
				{
					Debug.LogError("Invalid insert position");
					result = 0;
					return result;
				}
			}
			int depth = this.m_SavedFilters[num].m_Depth + ((!addAsChild) ? 0 : 1);
			SavedFilter savedFilter = new SavedFilter(displayName, searchFilter, depth, previewSize);
			savedFilter.m_ID = this.GetNextAvailableID();
			if (this.m_SavedFilters.Count == 0)
			{
				this.m_SavedFilters.Add(savedFilter);
			}
			else
			{
				this.m_SavedFilters.Insert(num + 1, savedFilter);
			}
			this.Changed();
			result = savedFilter.m_ID;
			return result;
		}

		private List<SavedFilter> GetSavedFilterAndChildren(int instanceID)
		{
			List<SavedFilter> list = new List<SavedFilter>();
			int num = this.IndexOf(instanceID);
			if (num >= 0)
			{
				list.Add(this.m_SavedFilters[num]);
				for (int i = num + 1; i < this.m_SavedFilters.Count; i++)
				{
					if (this.m_SavedFilters[i].m_Depth <= this.m_SavedFilters[num].m_Depth)
					{
						break;
					}
					list.Add(this.m_SavedFilters[i]);
				}
			}
			return list;
		}

		private void Remove(int instanceID)
		{
			int num = this.IndexOf(instanceID);
			if (num >= 1)
			{
				List<SavedFilter> savedFilterAndChildren = this.GetSavedFilterAndChildren(instanceID);
				if (savedFilterAndChildren.Count > 0)
				{
					this.m_SavedFilters.RemoveRange(num, savedFilterAndChildren.Count);
					this.Changed();
				}
			}
		}

		private int IndexOf(int instanceID)
		{
			int result;
			for (int i = 0; i < this.m_SavedFilters.Count; i++)
			{
				if (this.m_SavedFilters[i].m_ID == instanceID)
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		private SavedFilter Find(int instanceID)
		{
			int num = this.IndexOf(instanceID);
			SavedFilter result;
			if (num >= 0)
			{
				result = this.m_SavedFilters[num];
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void Init()
		{
			if (this.m_SavedFilters == null || this.m_SavedFilters.Count == 0)
			{
				this.m_SavedFilters = new List<SavedFilter>();
				this.m_SavedFilters.Add(new SavedFilter("Favorites", null, 0, -1f));
			}
			SearchFilter searchFilter = new SearchFilter();
			searchFilter.classNames = new string[0];
			this.m_SavedFilters[0].m_Name = "Favorites";
			this.m_SavedFilters[0].m_Filter = searchFilter;
			this.m_SavedFilters[0].m_Depth = 0;
			this.m_SavedFilters[0].m_ID = ProjectWindowUtil.k_FavoritesStartInstanceID;
			for (int i = 0; i < this.m_SavedFilters.Count; i++)
			{
				if (this.m_SavedFilters[i].m_ID < ProjectWindowUtil.k_FavoritesStartInstanceID)
				{
					this.m_SavedFilters[i].m_ID = this.GetNextAvailableID();
				}
			}
			if (!this.m_AllowHierarchy)
			{
				for (int j = 1; j < this.m_SavedFilters.Count; j++)
				{
					this.m_SavedFilters[j].m_Depth = 1;
				}
			}
			if (this.m_SavedFiltersInitialized != null && this.m_SavedFilters.Count > 1)
			{
				this.m_SavedFiltersInitialized();
			}
		}

		private int GetRoot()
		{
			int result;
			if (this.m_SavedFilters != null && this.m_SavedFilters.Count > 0)
			{
				result = this.m_SavedFilters[0].m_ID;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private TreeViewItem BuildTreeView()
		{
			this.Init();
			TreeViewItem result;
			if (this.m_SavedFilters.Count == 0)
			{
				Debug.LogError("BuildTreeView: No saved filters! We should at least have a root");
				result = null;
			}
			else
			{
				TreeViewItem treeViewItem = null;
				List<TreeViewItem> list = new List<TreeViewItem>();
				int i = 0;
				while (i < this.m_SavedFilters.Count)
				{
					SavedFilter savedFilter = this.m_SavedFilters[i];
					int iD = savedFilter.m_ID;
					int depth = savedFilter.m_Depth;
					bool isFolder = savedFilter.m_Filter.GetState() == SearchFilter.State.FolderBrowsing;
					TreeViewItem treeViewItem2 = new SearchFilterTreeItem(iD, depth, null, savedFilter.m_Name, isFolder);
					if (i == 0)
					{
						treeViewItem = treeViewItem2;
					}
					else
					{
						if (Collab.instance.collabFilters.ContainsSearchFilter(savedFilter.m_Name, savedFilter.m_Filter.FilterToSearchFieldString()))
						{
							if (!Collab.instance.IsCollabEnabledForCurrentProject())
							{
								goto IL_133;
							}
						}
						if (SoftlockViewController.Instance.softLockFilters.ContainsSearchFilter(savedFilter.m_Name, savedFilter.m_Filter.FilterToSearchFieldString()))
						{
							if (!CollabSettingsManager.IsAvailable(CollabSettingType.InProgressEnabled))
							{
								goto IL_133;
							}
							if (!Collab.instance.IsCollabEnabledForCurrentProject() || !CollabSettingsManager.inProgressEnabled)
							{
								goto IL_133;
							}
						}
						list.Add(treeViewItem2);
					}
					IL_133:
					i++;
					continue;
					goto IL_133;
				}
				TreeViewUtility.SetChildParentReferences(list, treeViewItem);
				result = treeViewItem;
			}
			return result;
		}

		private void Changed()
		{
			bool saveAsText = true;
			this.Save(saveAsText);
			if (this.m_SavedFiltersChanged != null)
			{
				this.m_SavedFiltersChanged();
			}
		}

		public override string ToString()
		{
			string text = "Saved Filters ";
			for (int i = 0; i < this.m_SavedFilters.Count; i++)
			{
				int iD = this.m_SavedFilters[i].m_ID;
				SavedFilter savedFilter = this.m_SavedFilters[i];
				text += string.Format(": {0} ({1})({2})({3}) ", new object[]
				{
					savedFilter.m_Name,
					iD,
					savedFilter.m_Depth,
					savedFilter.m_PreviewSize
				});
			}
			return text;
		}
	}
}
