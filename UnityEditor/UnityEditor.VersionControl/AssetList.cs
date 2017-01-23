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
			AssetList result;
			if (!includeFolder && (states == null || states.Length == 0))
			{
				result = assetList;
			}
			else
			{
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
				result = assetList;
			}
			return result;
		}

		public int FilterCount(bool includeFolder, params Asset.States[] states)
		{
			int num = 0;
			int result;
			if (!includeFolder && states == null)
			{
				result = base.Count;
			}
			else
			{
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
				result = num;
			}
			return result;
		}

		public AssetList FilterChildren()
		{
			AssetList assetList = new AssetList();
			assetList.AddRange(this);
			using (List<Asset>.Enumerator enumerator = base.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Asset asset = enumerator.Current;
					assetList.RemoveAll((Asset p) => p.IsChildOf(asset));
				}
			}
			return assetList;
		}
	}
}
