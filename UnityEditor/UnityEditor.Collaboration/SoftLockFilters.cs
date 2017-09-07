using System;
using System.Collections.Generic;

namespace UnityEditor.Collaboration
{
	internal class SoftLockFilters : AbstractFilters
	{
		public SoftLockFilters()
		{
			this.InitializeFilters();
		}

		public override void InitializeFilters()
		{
			base.filters = new List<string[]>
			{
				new string[]
				{
					"All In Progress",
					"s:inprogress"
				}
			};
		}

		public void OnSettingStatusChanged(CollabSettingType type, CollabSettingStatus status)
		{
			if (type == CollabSettingType.InProgressEnabled && status == CollabSettingStatus.Available)
			{
				if (Collab.instance.IsCollabEnabledForCurrentProject() && CollabSettingsManager.inProgressEnabled)
				{
					base.ShowInFavoriteSearchFilters();
				}
				else
				{
					base.HideFromFavoriteSearchFilters();
				}
			}
		}
	}
}
