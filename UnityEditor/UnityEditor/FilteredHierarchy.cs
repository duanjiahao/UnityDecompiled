using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class FilteredHierarchy
	{
		public class FilterResult
		{
			public int instanceID;

			public string name;

			public bool hasChildren;

			public int colorCode;

			public bool isMainRepresentation;

			public bool hasFullPreviewImage;

			public IconDrawStyle iconDrawStyle;

			public bool isFolder;

			public HierarchyType type;

			private Texture2D m_Icon;

			public Texture2D icon
			{
				get
				{
					if (this.m_Icon == null)
					{
						if (this.type == HierarchyType.Assets)
						{
							string assetPath = AssetDatabase.GetAssetPath(this.instanceID);
							if (assetPath != null)
							{
								return AssetDatabase.GetCachedIcon(assetPath) as Texture2D;
							}
						}
						else if (this.type == HierarchyType.GameObjects)
						{
							UnityEngine.Object obj = EditorUtility.InstanceIDToObject(this.instanceID);
							this.m_Icon = AssetPreview.GetMiniThumbnail(obj);
						}
					}
					return this.m_Icon;
				}
				set
				{
					this.m_Icon = value;
				}
			}

			public string guid
			{
				get
				{
					if (this.type == HierarchyType.Assets)
					{
						string assetPath = AssetDatabase.GetAssetPath(this.instanceID);
						if (assetPath != null)
						{
							return AssetDatabase.AssetPathToGUID(assetPath);
						}
					}
					return null;
				}
			}
		}

		private SearchFilter m_SearchFilter = new SearchFilter();

		private FilteredHierarchy.FilterResult[] m_Results = new FilteredHierarchy.FilterResult[0];

		private FilteredHierarchy.FilterResult[] m_VisibleItems = new FilteredHierarchy.FilterResult[0];

		private HierarchyType m_HierarchyType;

		public HierarchyType hierarchyType
		{
			get
			{
				return this.m_HierarchyType;
			}
		}

		public FilteredHierarchy.FilterResult[] results
		{
			get
			{
				if (this.m_VisibleItems.Length > 0)
				{
					return this.m_VisibleItems;
				}
				return this.m_Results;
			}
		}

		public SearchFilter searchFilter
		{
			get
			{
				return this.m_SearchFilter;
			}
			set
			{
				if (this.m_SearchFilter.SetNewFilter(value))
				{
					this.ResultsChanged();
				}
			}
		}

		public bool foldersFirst
		{
			get;
			set;
		}

		public FilteredHierarchy(HierarchyType type)
		{
			this.m_HierarchyType = type;
		}

		public void SetResults(int[] instanceIDs)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(this.m_HierarchyType);
			hierarchyProperty.Reset();
			Array.Resize<FilteredHierarchy.FilterResult>(ref this.m_Results, instanceIDs.Length);
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				if (hierarchyProperty.Find(instanceIDs[i], null))
				{
					this.CopyPropertyData(ref this.m_Results[i], hierarchyProperty);
				}
			}
		}

		private void CopyPropertyData(ref FilteredHierarchy.FilterResult result, HierarchyProperty property)
		{
			if (result == null)
			{
				result = new FilteredHierarchy.FilterResult();
			}
			result.instanceID = property.instanceID;
			result.name = property.name;
			result.hasChildren = property.hasChildren;
			result.colorCode = property.colorCode;
			result.isMainRepresentation = property.isMainRepresentation;
			result.hasFullPreviewImage = property.hasFullPreviewImage;
			result.iconDrawStyle = property.iconDrawStyle;
			result.isFolder = property.isFolder;
			result.type = this.hierarchyType;
			if (!property.isMainRepresentation)
			{
				result.icon = property.icon;
			}
			else if (property.isFolder && !property.hasChildren)
			{
				result.icon = EditorGUIUtility.FindTexture(EditorResourcesUtility.emptyFolderIconName);
			}
			else
			{
				result.icon = null;
			}
		}

		private void SearchAllAssets(HierarchyProperty property)
		{
			int num = property.CountRemaining(null);
			num = Mathf.Min(num, 3000);
			property.Reset();
			int num2 = this.m_Results.Length;
			Array.Resize<FilteredHierarchy.FilterResult>(ref this.m_Results, this.m_Results.Length + num);
			while (property.Next(null) && num2 < this.m_Results.Length)
			{
				this.CopyPropertyData(ref this.m_Results[num2], property);
				num2++;
			}
		}

		private void SearchInFolders(HierarchyProperty property)
		{
			List<FilteredHierarchy.FilterResult> list = new List<FilteredHierarchy.FilterResult>();
			string[] baseFolders = ProjectWindowUtil.GetBaseFolders(this.m_SearchFilter.folders);
			string[] array = baseFolders;
			for (int i = 0; i < array.Length; i++)
			{
				string assetPath = array[i];
				property.SetSearchFilter(new SearchFilter());
				int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(assetPath);
				if (property.Find(mainAssetInstanceID, null))
				{
					property.SetSearchFilter(this.m_SearchFilter);
					int depth = property.depth;
					int[] expanded = null;
					while (property.NextWithDepthCheck(expanded, depth + 1))
					{
						FilteredHierarchy.FilterResult item = new FilteredHierarchy.FilterResult();
						this.CopyPropertyData(ref item, property);
						list.Add(item);
					}
				}
			}
			this.m_Results = list.ToArray();
		}

		private void FolderBrowsing(HierarchyProperty property)
		{
			List<FilteredHierarchy.FilterResult> list = new List<FilteredHierarchy.FilterResult>();
			string[] folders = this.m_SearchFilter.folders;
			for (int i = 0; i < folders.Length; i++)
			{
				string assetPath = folders[i];
				int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(assetPath);
				if (property.Find(mainAssetInstanceID, null))
				{
					int depth = property.depth;
					int[] array = new int[]
					{
						mainAssetInstanceID
					};
					while (property.Next(array))
					{
						if (property.depth <= depth)
						{
							break;
						}
						FilteredHierarchy.FilterResult item = new FilteredHierarchy.FilterResult();
						this.CopyPropertyData(ref item, property);
						list.Add(item);
						if (property.hasChildren && !property.isFolder)
						{
							Array.Resize<int>(ref array, array.Length + 1);
							array[array.Length - 1] = property.instanceID;
						}
					}
				}
			}
			this.m_Results = list.ToArray();
		}

		private void AddResults(HierarchyProperty property)
		{
			switch (this.m_SearchFilter.GetState())
			{
			case SearchFilter.State.EmptySearchFilter:
				break;
			case SearchFilter.State.FolderBrowsing:
				this.FolderBrowsing(property);
				break;
			case SearchFilter.State.SearchingInAllAssets:
				this.SearchAllAssets(property);
				break;
			case SearchFilter.State.SearchingInFolders:
				this.SearchInFolders(property);
				break;
			case SearchFilter.State.SearchingInAssetStore:
				break;
			default:
				Debug.LogError("Unhandled enum!");
				break;
			}
		}

		public void ResultsChanged()
		{
			this.m_Results = new FilteredHierarchy.FilterResult[0];
			if (this.m_SearchFilter.GetState() != SearchFilter.State.EmptySearchFilter)
			{
				HierarchyProperty hierarchyProperty = new HierarchyProperty(this.m_HierarchyType);
				hierarchyProperty.SetSearchFilter(this.m_SearchFilter);
				this.AddResults(hierarchyProperty);
				if (this.m_SearchFilter.IsSearching())
				{
					Array.Sort<FilteredHierarchy.FilterResult>(this.m_Results, (FilteredHierarchy.FilterResult result1, FilteredHierarchy.FilterResult result2) => EditorUtility.NaturalCompare(result1.name, result2.name));
				}
				if (this.foldersFirst)
				{
					for (int i = 0; i < this.m_Results.Length; i++)
					{
						if (!this.m_Results[i].isFolder)
						{
							for (int j = i + 1; j < this.m_Results.Length; j++)
							{
								if (this.m_Results[j].isFolder)
								{
									FilteredHierarchy.FilterResult filterResult = this.m_Results[j];
									int length = j - i;
									Array.Copy(this.m_Results, i, this.m_Results, i + 1, length);
									this.m_Results[i] = filterResult;
									break;
								}
							}
						}
					}
				}
			}
			else if (this.m_HierarchyType == HierarchyType.GameObjects)
			{
				HierarchyProperty hierarchyProperty2 = new HierarchyProperty(HierarchyType.GameObjects);
				hierarchyProperty2.SetSearchFilter(this.m_SearchFilter);
			}
		}

		public void RefreshVisibleItems(List<int> expandedInstanceIDs)
		{
			bool flag = this.m_SearchFilter.IsSearching();
			List<FilteredHierarchy.FilterResult> list = new List<FilteredHierarchy.FilterResult>();
			for (int i = 0; i < this.m_Results.Length; i++)
			{
				list.Add(this.m_Results[i]);
				if (this.m_Results[i].isMainRepresentation && this.m_Results[i].hasChildren && !this.m_Results[i].isFolder)
				{
					bool flag2 = expandedInstanceIDs.IndexOf(this.m_Results[i].instanceID) >= 0 || flag;
					int num = this.AddSubItemsOfMainRepresentation(i, (!flag2) ? null : list);
					i += num;
				}
			}
			this.m_VisibleItems = list.ToArray();
		}

		public List<int> GetSubAssetInstanceIDs(int mainAssetInstanceID)
		{
			for (int i = 0; i < this.m_Results.Length; i++)
			{
				if (this.m_Results[i].instanceID == mainAssetInstanceID)
				{
					List<int> list = new List<int>();
					int num = i + 1;
					while (num < this.m_Results.Length && !this.m_Results[num].isMainRepresentation)
					{
						list.Add(this.m_Results[num].instanceID);
						num++;
					}
					return list;
				}
			}
			Debug.LogError("Not main rep " + mainAssetInstanceID);
			return new List<int>();
		}

		public int AddSubItemsOfMainRepresentation(int mainRepresentionIndex, List<FilteredHierarchy.FilterResult> visibleItems)
		{
			int num = 0;
			int num2 = mainRepresentionIndex + 1;
			while (num2 < this.m_Results.Length && !this.m_Results[num2].isMainRepresentation)
			{
				if (visibleItems != null)
				{
					visibleItems.Add(this.m_Results[num2]);
				}
				num2++;
				num++;
			}
			return num;
		}
	}
}
