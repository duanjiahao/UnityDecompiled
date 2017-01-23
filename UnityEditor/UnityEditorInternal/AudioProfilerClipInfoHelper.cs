using System;
using System.Collections.Generic;

namespace UnityEditorInternal
{
	internal class AudioProfilerClipInfoHelper
	{
		public enum ColumnIndices
		{
			AssetName,
			LoadState,
			InternalLoadState,
			Age,
			Disposed,
			NumChannelInstances,
			_LastColumn
		}

		public class AudioProfilerClipInfoComparer : IComparer<AudioProfilerClipInfoWrapper>
		{
			public AudioProfilerClipInfoHelper.ColumnIndices primarySortKey;

			public AudioProfilerClipInfoHelper.ColumnIndices secondarySortKey;

			public bool sortByDescendingOrder;

			public AudioProfilerClipInfoComparer(AudioProfilerClipInfoHelper.ColumnIndices primarySortKey, AudioProfilerClipInfoHelper.ColumnIndices secondarySortKey, bool sortByDescendingOrder)
			{
				this.primarySortKey = primarySortKey;
				this.secondarySortKey = secondarySortKey;
				this.sortByDescendingOrder = sortByDescendingOrder;
			}

			private int CompareInternal(AudioProfilerClipInfoWrapper a, AudioProfilerClipInfoWrapper b, AudioProfilerClipInfoHelper.ColumnIndices key)
			{
				int num = 0;
				switch (key)
				{
				case AudioProfilerClipInfoHelper.ColumnIndices.AssetName:
					num = a.assetName.CompareTo(b.assetName);
					break;
				case AudioProfilerClipInfoHelper.ColumnIndices.LoadState:
					num = a.info.loadState.CompareTo(b.info.loadState);
					break;
				case AudioProfilerClipInfoHelper.ColumnIndices.InternalLoadState:
					num = a.info.internalLoadState.CompareTo(b.info.internalLoadState);
					break;
				case AudioProfilerClipInfoHelper.ColumnIndices.Age:
					num = a.info.age.CompareTo(b.info.age);
					break;
				case AudioProfilerClipInfoHelper.ColumnIndices.Disposed:
					num = a.info.disposed.CompareTo(b.info.disposed);
					break;
				case AudioProfilerClipInfoHelper.ColumnIndices.NumChannelInstances:
					num = a.info.numChannelInstances.CompareTo(b.info.numChannelInstances);
					break;
				}
				return (!this.sortByDescendingOrder) ? num : (-num);
			}

			public int Compare(AudioProfilerClipInfoWrapper a, AudioProfilerClipInfoWrapper b)
			{
				int num = this.CompareInternal(a, b, this.primarySortKey);
				return (num != 0) ? num : this.CompareInternal(a, b, this.secondarySortKey);
			}
		}

		private static string[] m_LoadStateNames = new string[]
		{
			"Unloaded",
			"Loading Base",
			"Loading Sub",
			"Loaded",
			"Failed"
		};

		private static string[] m_InternalLoadStateNames = new string[]
		{
			"Pending",
			"Loaded",
			"Failed"
		};

		public static string GetColumnString(AudioProfilerClipInfoWrapper info, AudioProfilerClipInfoHelper.ColumnIndices index)
		{
			string result;
			switch (index)
			{
			case AudioProfilerClipInfoHelper.ColumnIndices.AssetName:
				result = info.assetName;
				break;
			case AudioProfilerClipInfoHelper.ColumnIndices.LoadState:
				result = AudioProfilerClipInfoHelper.m_LoadStateNames[info.info.loadState];
				break;
			case AudioProfilerClipInfoHelper.ColumnIndices.InternalLoadState:
				result = AudioProfilerClipInfoHelper.m_InternalLoadStateNames[info.info.internalLoadState];
				break;
			case AudioProfilerClipInfoHelper.ColumnIndices.Age:
				result = info.info.age.ToString();
				break;
			case AudioProfilerClipInfoHelper.ColumnIndices.Disposed:
				result = ((info.info.disposed == 0) ? "NO" : "YES");
				break;
			case AudioProfilerClipInfoHelper.ColumnIndices.NumChannelInstances:
				result = info.info.numChannelInstances.ToString();
				break;
			default:
				result = "Unknown";
				break;
			}
			return result;
		}

		public static int GetLastColumnIndex()
		{
			return 5;
		}
	}
}
