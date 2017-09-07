using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal abstract class AbstractFilters
	{
		[SerializeField]
		private List<string[]> m_Filters;

		public List<string[]> filters
		{
			get
			{
				return this.m_Filters;
			}
			set
			{
				this.m_Filters = value;
			}
		}

		public abstract void InitializeFilters();

		public bool ContainsSearchFilter(string name, string searchString)
		{
			bool result;
			foreach (string[] current in this.filters)
			{
				if (current[0] == name && current[1] == searchString)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public void ShowInFavoriteSearchFilters()
		{
			if (SavedSearchFilters.GetRootInstanceID() == 0)
			{
				SavedSearchFilters.AddInitializedListener(new Action(this.ShowInFavoriteSearchFilters));
			}
			else
			{
				SavedSearchFilters.RemoveInitializedListener(new Action(this.ShowInFavoriteSearchFilters));
				int num = 0;
				foreach (string[] current in this.filters)
				{
					if (SavedSearchFilters.GetFilterInstanceID(current[0], current[1]) == 0)
					{
						SearchFilter filter = SearchFilter.CreateSearchFilterFromString(current[1]);
						if (num == 0)
						{
							num = SavedSearchFilters.AddSavedFilter(current[0], filter, 64f);
						}
						else
						{
							num = SavedSearchFilters.AddSavedFilterAfterInstanceID(current[0], filter, 64f, num, false);
						}
					}
				}
				SavedSearchFilters.RefreshSavedFilters();
				foreach (ProjectBrowser current2 in ProjectBrowser.GetAllProjectBrowsers())
				{
					current2.Repaint();
				}
			}
		}

		public void HideFromFavoriteSearchFilters()
		{
			SavedSearchFilters.RefreshSavedFilters();
			foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
			{
				current.Repaint();
			}
		}
	}
}
