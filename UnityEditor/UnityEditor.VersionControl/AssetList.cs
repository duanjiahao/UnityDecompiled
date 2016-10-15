using System;
using System.Collections.Generic;

namespace UnityEditor.VersionControl
{
	public class AssetList : List<Asset>
	{
		public AssetList()
		{
		}

		public AssetList(AssetList src)
		{
		}

		public AssetList Filter(bool includeFolder, params Asset.States[] states)
		{
			AssetList assetList = new AssetList();
			if (!includeFolder && (states == null || states.Length == 0))
			{
				return assetList;
			}
			foreach (Asset current in this)
			{
				if (current.isFolder)
				{
					if (includeFolder)
					{
						assetList.Add(current);
					}
				}
				else if (current.IsOneOfStates(states))
				{
					assetList.Add(current);
				}
			}
			return assetList;
		}

		public int FilterCount(bool includeFolder, params Asset.States[] states)
		{
			int num = 0;
			if (!includeFolder && states == null)
			{
				return this.Count;
			}
			foreach (Asset current in this)
			{
				if (current.isFolder)
				{
					num++;
				}
				else if (current.IsOneOfStates(states))
				{
					num++;
				}
			}
			return num;
		}

		public AssetList FilterChildren()
		{
			AssetList assetList = new AssetList();
			assetList.AddRange(this);
			foreach (Asset asset in this)
			{
				assetList.RemoveAll((Asset p) => p.IsChildOf(asset));
			}
			return assetList;
		}
	}
}
