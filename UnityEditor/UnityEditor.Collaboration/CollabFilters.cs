using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class CollabFilters : AbstractFilters
	{
		[SerializeField]
		private bool m_SearchFilterWasSet = false;

		public CollabFilters()
		{
			this.InitializeFilters();
		}

		public override void InitializeFilters()
		{
			base.filters = new List<string[]>
			{
				new string[]
				{
					"All Modified",
					"v:any"
				},
				new string[]
				{
					"All Conflicts",
					"v:conflicted"
				},
				new string[]
				{
					"All Excluded",
					"v:ignored"
				}
			};
		}

		public void ShowInProjectBrowser(string filterString)
		{
			ProjectBrowser projectBrowser = ProjectBrowser.s_LastInteractedProjectBrowser;
			if (projectBrowser == null)
			{
				List<ProjectBrowser> allProjectBrowsers = ProjectBrowser.GetAllProjectBrowsers();
				if (allProjectBrowsers != null && allProjectBrowsers.Count > 0)
				{
					projectBrowser = allProjectBrowsers.First<ProjectBrowser>();
				}
			}
			if (!string.IsNullOrEmpty(filterString))
			{
				if (projectBrowser == null)
				{
					projectBrowser = EditorWindow.GetWindow<ProjectBrowser>();
					base.ShowInFavoriteSearchFilters();
					projectBrowser.RepaintImmediately();
				}
				this.m_SearchFilterWasSet = true;
				string text = "v:" + filterString;
				if (projectBrowser.IsTwoColumns())
				{
					foreach (string[] current in base.filters)
					{
						if (text == current[1])
						{
							int filterInstanceID = SavedSearchFilters.GetFilterInstanceID(current[0], text);
							if (filterInstanceID > ProjectWindowUtil.k_FavoritesStartInstanceID)
							{
								projectBrowser.SetFolderSelection(new int[]
								{
									filterInstanceID
								}, true);
								break;
							}
						}
					}
				}
				projectBrowser.SetSearch(text);
				projectBrowser.Repaint();
				projectBrowser.Focus();
			}
			else
			{
				if (this.m_SearchFilterWasSet)
				{
					if (projectBrowser != null)
					{
						if (projectBrowser.IsTwoColumns())
						{
							int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID("assets");
							projectBrowser.SetFolderSelection(new int[]
							{
								mainAssetInstanceID
							}, true);
						}
						projectBrowser.SetSearch("");
						projectBrowser.Repaint();
					}
				}
				this.m_SearchFilterWasSet = false;
			}
		}

		public void OnCollabStateChanged(CollabInfo info)
		{
			if (info.ready && !info.inProgress && !info.maintenance)
			{
				foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
				{
					current.RefreshSearchIfFilterContains("v:");
				}
			}
		}
	}
}
