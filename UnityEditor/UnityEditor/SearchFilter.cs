using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
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
		private string m_NameFilter = string.Empty;

		[SerializeField]
		private string[] m_ClassNames = new string[0];

		[SerializeField]
		private string[] m_AssetLabels = new string[0];

		[SerializeField]
		private string[] m_AssetBundleNames = new string[0];

		[SerializeField]
		private string[] m_VersionControlStates = new string[0];

		[SerializeField]
		private int[] m_ReferencingInstanceIDs = new int[0];

		[SerializeField]
		private string[] m_ScenePaths;

		[SerializeField]
		private bool m_ShowAllHits;

		[SerializeField]
		private SearchFilter.SearchArea m_SearchArea;

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
			this.m_NameFilter = string.Empty;
			this.m_ClassNames = new string[0];
			this.m_AssetLabels = new string[0];
			this.m_AssetBundleNames = new string[0];
			this.m_ReferencingInstanceIDs = new int[0];
			this.m_ScenePaths = new string[0];
			this.m_VersionControlStates = new string[0];
			this.m_ShowAllHits = false;
		}

		private bool IsNullOrEmtpy<T>(T[] list)
		{
			return list == null || list.Length == 0;
		}

		public SearchFilter.State GetState()
		{
			bool flag = !string.IsNullOrEmpty(this.m_NameFilter) || !this.IsNullOrEmtpy<string>(this.m_AssetLabels) || !this.IsNullOrEmtpy<string>(this.m_ClassNames) || !this.IsNullOrEmtpy<string>(this.m_AssetBundleNames) || !this.IsNullOrEmtpy<int>(this.m_ReferencingInstanceIDs);
			if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted)
			{
				flag = (flag || !this.IsNullOrEmtpy<string>(this.m_VersionControlStates));
			}
			bool flag2 = !this.IsNullOrEmtpy<string>(this.m_Folders);
			if (flag)
			{
				if (this.m_SearchArea == SearchFilter.SearchArea.AssetStore)
				{
					return SearchFilter.State.SearchingInAssetStore;
				}
				if (flag2 && this.m_SearchArea == SearchFilter.SearchArea.SelectedFolders)
				{
					return SearchFilter.State.SearchingInFolders;
				}
				return SearchFilter.State.SearchingInAllAssets;
			}
			else
			{
				if (flag2)
				{
					return SearchFilter.State.FolderBrowsing;
				}
				return SearchFilter.State.EmptySearchFilter;
			}
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
			if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted && newFilter.m_VersionControlStates != this.m_VersionControlStates)
			{
				this.m_VersionControlStates = newFilter.m_VersionControlStates;
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
			if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted && this.m_VersionControlStates != null && this.m_VersionControlStates.Length > 0)
			{
				text = text + "[VersionStates: " + this.m_VersionControlStates[0] + "]";
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
			string text = string.Empty;
			if (!string.IsNullOrEmpty(this.m_NameFilter))
			{
				text += this.m_NameFilter;
			}
			this.AddToString<string>("t:", this.m_ClassNames, ref text);
			this.AddToString<string>("l:", this.m_AssetLabels, ref text);
			if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted)
			{
				this.AddToString<string>("v:", this.m_VersionControlStates, ref text);
			}
			this.AddToString<string>("b:", this.m_AssetBundleNames, ref text);
			return text;
		}

		private void AddToString<T>(string prefix, T[] list, ref string result)
		{
			if (list == null)
			{
				return;
			}
			if (result == null)
			{
				result = string.Empty;
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

		internal void SearchFieldStringToFilter(string searchString)
		{
			this.ClearSearch();
			if (string.IsNullOrEmpty(searchString))
			{
				return;
			}
			SearchUtility.ParseSearchString(searchString, this);
		}

		internal static SearchFilter CreateSearchFilterFromString(string searchText)
		{
			SearchFilter searchFilter = new SearchFilter();
			SearchUtility.ParseSearchString(searchText, searchFilter);
			return searchFilter;
		}

		public static string[] Split(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return new string[0];
			}
			List<string> list = new List<string>();
			foreach (Match match in Regex.Matches(text, "\".+?\"|\\S+"))
			{
				list.Add(match.Value.Replace("\"", string.Empty));
			}
			return list.ToArray();
		}
	}
}
