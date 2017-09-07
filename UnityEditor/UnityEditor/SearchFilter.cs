using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class SearchFilter
	{
		public enum SearchArea
		{
			AllAssets,
			SelectedFolders,
			AssetStore
		}

		public enum State
		{
			EmptySearchFilter,
			FolderBrowsing,
			SearchingInAllAssets,
			SearchingInFolders,
			SearchingInAssetStore
		}

		[SerializeField]
		private string m_NameFilter = "";

		[SerializeField]
		private string[] m_ClassNames = new string[0];

		[SerializeField]
		private string[] m_AssetLabels = new string[0];

		[SerializeField]
		private string[] m_AssetBundleNames = new string[0];

		[SerializeField]
		private string[] m_VersionControlStates = new string[0];

		[SerializeField]
		private string[] m_SoftLockControlStates = new string[0];

		[SerializeField]
		private int[] m_ReferencingInstanceIDs = new int[0];

		[SerializeField]
		private string[] m_ScenePaths;

		[SerializeField]
		private bool m_ShowAllHits = false;

		[SerializeField]
		private SearchFilter.SearchArea m_SearchArea = SearchFilter.SearchArea.AllAssets;

		[SerializeField]
		private string[] m_Folders = new string[0];

		public string nameFilter
		{
			get
			{
				return this.m_NameFilter;
			}
			set
			{
				this.m_NameFilter = value;
			}
		}

		public string[] classNames
		{
			get
			{
				return this.m_ClassNames;
			}
			set
			{
				this.m_ClassNames = value;
			}
		}

		public string[] assetLabels
		{
			get
			{
				return this.m_AssetLabels;
			}
			set
			{
				this.m_AssetLabels = value;
			}
		}

		public string[] versionControlStates
		{
			get
			{
				return this.m_VersionControlStates;
			}
			set
			{
				this.m_VersionControlStates = value;
			}
		}

		public string[] softLockControlStates
		{
			get
			{
				return this.m_SoftLockControlStates;
			}
			set
			{
				this.m_SoftLockControlStates = value;
			}
		}

		public string[] assetBundleNames
		{
			get
			{
				return this.m_AssetBundleNames;
			}
			set
			{
				this.m_AssetBundleNames = value;
			}
		}

		public int[] referencingInstanceIDs
		{
			get
			{
				return this.m_ReferencingInstanceIDs;
			}
			set
			{
				this.m_ReferencingInstanceIDs = value;
			}
		}

		public string[] scenePaths
		{
			get
			{
				return this.m_ScenePaths;
			}
			set
			{
				this.m_ScenePaths = value;
			}
		}

		public bool showAllHits
		{
			get
			{
				return this.m_ShowAllHits;
			}
			set
			{
				this.m_ShowAllHits = value;
			}
		}

		public string[] folders
		{
			get
			{
				return this.m_Folders;
			}
			set
			{
				this.m_Folders = value;
			}
		}

		public SearchFilter.SearchArea searchArea
		{
			get
			{
				return this.m_SearchArea;
			}
			set
			{
				this.m_SearchArea = value;
			}
		}

		public void ClearSearch()
		{
			this.m_NameFilter = "";
			this.m_ClassNames = new string[0];
			this.m_AssetLabels = new string[0];
			this.m_AssetBundleNames = new string[0];
			this.m_ReferencingInstanceIDs = new int[0];
			this.m_ScenePaths = new string[0];
			this.m_VersionControlStates = new string[0];
			this.m_SoftLockControlStates = new string[0];
			this.m_ShowAllHits = false;
		}

		private bool IsNullOrEmpty<T>(T[] list)
		{
			return list == null || list.Length == 0;
		}

		public SearchFilter.State GetState()
		{
			bool flag = !string.IsNullOrEmpty(this.m_NameFilter) || !this.IsNullOrEmpty<string>(this.m_AssetLabels) || !this.IsNullOrEmpty<string>(this.m_ClassNames) || !this.IsNullOrEmpty<string>(this.m_AssetBundleNames) || !this.IsNullOrEmpty<int>(this.m_ReferencingInstanceIDs) || !this.IsNullOrEmpty<string>(this.m_VersionControlStates) || !this.IsNullOrEmpty<string>(this.m_SoftLockControlStates);
			bool flag2 = !this.IsNullOrEmpty<string>(this.m_Folders);
			SearchFilter.State result;
			if (flag)
			{
				if (this.m_SearchArea == SearchFilter.SearchArea.AssetStore)
				{
					result = SearchFilter.State.SearchingInAssetStore;
				}
				else if (flag2 && this.m_SearchArea == SearchFilter.SearchArea.SelectedFolders)
				{
					result = SearchFilter.State.SearchingInFolders;
				}
				else
				{
					result = SearchFilter.State.SearchingInAllAssets;
				}
			}
			else if (flag2)
			{
				result = SearchFilter.State.FolderBrowsing;
			}
			else
			{
				result = SearchFilter.State.EmptySearchFilter;
			}
			return result;
		}

		public bool IsSearching()
		{
			SearchFilter.State state = this.GetState();
			return state == SearchFilter.State.SearchingInAllAssets || state == SearchFilter.State.SearchingInFolders || state == SearchFilter.State.SearchingInAssetStore;
		}

		public bool SetNewFilter(SearchFilter newFilter)
		{
			bool result = false;
			if (newFilter.m_NameFilter != this.m_NameFilter)
			{
				this.m_NameFilter = newFilter.m_NameFilter;
				result = true;
			}
			if (newFilter.m_ClassNames != this.m_ClassNames)
			{
				this.m_ClassNames = newFilter.m_ClassNames;
				result = true;
			}
			if (newFilter.m_Folders != this.m_Folders)
			{
				this.m_Folders = newFilter.m_Folders;
				result = true;
			}
			if (newFilter.m_VersionControlStates != this.m_VersionControlStates)
			{
				this.m_VersionControlStates = newFilter.m_VersionControlStates;
				result = true;
			}
			if (newFilter.m_SoftLockControlStates != this.m_SoftLockControlStates)
			{
				this.m_SoftLockControlStates = newFilter.m_SoftLockControlStates;
				result = true;
			}
			if (newFilter.m_AssetLabels != this.m_AssetLabels)
			{
				this.m_AssetLabels = newFilter.m_AssetLabels;
				result = true;
			}
			if (newFilter.m_AssetBundleNames != this.m_AssetBundleNames)
			{
				this.m_AssetBundleNames = newFilter.m_AssetBundleNames;
				result = true;
			}
			if (newFilter.m_ReferencingInstanceIDs != this.m_ReferencingInstanceIDs)
			{
				this.m_ReferencingInstanceIDs = newFilter.m_ReferencingInstanceIDs;
				result = true;
			}
			if (newFilter.m_ScenePaths != this.m_ScenePaths)
			{
				this.m_ScenePaths = newFilter.m_ScenePaths;
				result = true;
			}
			if (newFilter.m_SearchArea != this.m_SearchArea)
			{
				this.m_SearchArea = newFilter.m_SearchArea;
				result = true;
			}
			this.m_ShowAllHits = newFilter.m_ShowAllHits;
			return result;
		}

		public override string ToString()
		{
			string text = "SearchFilter: ";
			text += string.Format("[Area: {0}, State: {1}]", this.m_SearchArea, this.GetState());
			if (!string.IsNullOrEmpty(this.m_NameFilter))
			{
				text = text + "[Name: " + this.m_NameFilter + "]";
			}
			if (this.m_AssetLabels != null && this.m_AssetLabels.Length > 0)
			{
				text = text + "[Labels: " + this.m_AssetLabels[0] + "]";
			}
			if (this.m_VersionControlStates != null && this.m_VersionControlStates.Length > 0)
			{
				text = text + "[VersionStates: " + this.m_VersionControlStates[0] + "]";
			}
			if (this.m_SoftLockControlStates != null && this.m_SoftLockControlStates.Length > 0)
			{
				text = text + "[SoftLockStates: " + this.m_SoftLockControlStates[0] + "]";
			}
			if (this.m_AssetBundleNames != null && this.m_AssetBundleNames.Length > 0)
			{
				text = text + "[AssetBundleNames: " + this.m_AssetBundleNames[0] + "]";
			}
			string text2;
			if (this.m_ClassNames != null && this.m_ClassNames.Length > 0)
			{
				text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"[Types: ",
					this.m_ClassNames[0],
					" (",
					this.m_ClassNames.Length,
					")]"
				});
			}
			if (this.m_ReferencingInstanceIDs != null && this.m_ReferencingInstanceIDs.Length > 0)
			{
				text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"[RefIDs: ",
					this.m_ReferencingInstanceIDs[0],
					"]"
				});
			}
			if (this.m_Folders != null && this.m_Folders.Length > 0)
			{
				text = text + "[Folders: " + this.m_Folders[0] + "]";
			}
			text2 = text;
			return string.Concat(new object[]
			{
				text2,
				"[ShowAllHits: ",
				this.showAllHits,
				"]"
			});
		}

		internal string FilterToSearchFieldString()
		{
			string text = "";
			if (!string.IsNullOrEmpty(this.m_NameFilter))
			{
				text += this.m_NameFilter;
			}
			this.AddToString<string>("t:", this.m_ClassNames, ref text);
			this.AddToString<string>("l:", this.m_AssetLabels, ref text);
			this.AddToString<string>("v:", this.m_VersionControlStates, ref text);
			this.AddToString<string>("s:", this.m_SoftLockControlStates, ref text);
			this.AddToString<string>("b:", this.m_AssetBundleNames, ref text);
			return text;
		}

		private void AddToString<T>(string prefix, T[] list, ref string result)
		{
			if (list != null)
			{
				if (result == null)
				{
					result = "";
				}
				for (int i = 0; i < list.Length; i++)
				{
					T t = list[i];
					if (!string.IsNullOrEmpty(result))
					{
						result += " ";
					}
					result = result + prefix + t;
				}
			}
		}

		internal void SearchFieldStringToFilter(string searchString)
		{
			this.ClearSearch();
			if (!string.IsNullOrEmpty(searchString))
			{
				SearchUtility.ParseSearchString(searchString, this);
			}
		}

		internal static SearchFilter CreateSearchFilterFromString(string searchText)
		{
			SearchFilter searchFilter = new SearchFilter();
			SearchUtility.ParseSearchString(searchText, searchFilter);
			return searchFilter;
		}

		public static string[] Split(string text)
		{
			string[] result;
			if (string.IsNullOrEmpty(text))
			{
				result = new string[0];
			}
			else
			{
				List<string> list = new List<string>();
				IEnumerator enumerator = Regex.Matches(text, "\".+?\"|\\S+").GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Match match = (Match)enumerator.Current;
						list.Add(match.Value.Replace("\"", ""));
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				result = list.ToArray();
			}
			return result;
		}
	}
}
