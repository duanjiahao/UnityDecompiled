using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AudioProfilerGroupInfoHelper
	{
		public enum ColumnIndices
		{
			ObjectName,
			AssetName,
			Volume,
			Audibility,
			PlayCount,
			Is3D,
			IsPaused,
			IsMuted,
			IsVirtual,
			IsOneShot,
			IsLooped,
			DistanceToListener,
			MinDist,
			MaxDist,
			Time,
			Duration,
			Frequency,
			IsStream,
			IsCompressed,
			IsNonBlocking,
			IsOpenUser,
			IsOpenMemory,
			IsOpenMemoryPoint,
			_LastColumn
		}

		public class AudioProfilerGroupInfoComparer : IComparer<AudioProfilerGroupInfoWrapper>
		{
			public AudioProfilerGroupInfoHelper.ColumnIndices primarySortKey;

			public AudioProfilerGroupInfoHelper.ColumnIndices secondarySortKey;

			public bool sortByDescendingOrder;

			public AudioProfilerGroupInfoComparer(AudioProfilerGroupInfoHelper.ColumnIndices primarySortKey, AudioProfilerGroupInfoHelper.ColumnIndices secondarySortKey, bool sortByDescendingOrder)
			{
				this.primarySortKey = primarySortKey;
				this.secondarySortKey = secondarySortKey;
				this.sortByDescendingOrder = sortByDescendingOrder;
			}

			private int CompareInternal(AudioProfilerGroupInfoWrapper a, AudioProfilerGroupInfoWrapper b, AudioProfilerGroupInfoHelper.ColumnIndices key)
			{
				int num = 0;
				switch (key)
				{
				case AudioProfilerGroupInfoHelper.ColumnIndices.ObjectName:
					num = a.objectName.CompareTo(b.objectName);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.AssetName:
					num = a.assetName.CompareTo(b.assetName);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.Volume:
					num = a.info.volume.CompareTo(b.info.volume);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.Audibility:
					num = a.info.audibility.CompareTo(b.info.audibility);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.PlayCount:
					num = a.info.playCount.CompareTo(b.info.playCount);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.Is3D:
					num = (a.info.flags & 1).CompareTo(b.info.flags & 1) + (a.info.flags & 2).CompareTo(b.info.flags & 2) * 2;
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsPaused:
					num = (a.info.flags & 4).CompareTo(b.info.flags & 4);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsMuted:
					num = (a.info.flags & 8).CompareTo(b.info.flags & 8);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsVirtual:
					num = (a.info.flags & 16).CompareTo(b.info.flags & 16);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsOneShot:
					num = (a.info.flags & 32).CompareTo(b.info.flags & 32);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsLooped:
					num = (a.info.flags & 512).CompareTo(b.info.flags & 512);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.DistanceToListener:
					num = a.info.distanceToListener.CompareTo(b.info.distanceToListener);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.MinDist:
					num = a.info.minDist.CompareTo(b.info.minDist);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.MaxDist:
					num = a.info.maxDist.CompareTo(b.info.maxDist);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.Time:
					num = a.info.time.CompareTo(b.info.time);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.Duration:
					num = a.info.duration.CompareTo(b.info.duration);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.Frequency:
					num = a.info.frequency.CompareTo(b.info.frequency);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsStream:
					num = (a.info.flags & 128).CompareTo(b.info.flags & 128);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsCompressed:
					num = (a.info.flags & 256).CompareTo(b.info.flags & 256);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsNonBlocking:
					num = (a.info.flags & 8192).CompareTo(b.info.flags & 8192);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenUser:
					num = (a.info.flags & 4096).CompareTo(b.info.flags & 4096);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenMemory:
					num = (a.info.flags & 1024).CompareTo(b.info.flags & 1024);
					break;
				case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenMemoryPoint:
					num = (a.info.flags & 2048).CompareTo(b.info.flags & 2048);
					break;
				}
				return (!this.sortByDescendingOrder) ? num : (-num);
			}

			public int Compare(AudioProfilerGroupInfoWrapper a, AudioProfilerGroupInfoWrapper b)
			{
				int num = this.CompareInternal(a, b, this.primarySortKey);
				return (num != 0) ? num : this.CompareInternal(a, b, this.secondarySortKey);
			}
		}

		public const int AUDIOPROFILER_FLAGS_3D = 1;

		public const int AUDIOPROFILER_FLAGS_ISSPATIAL = 2;

		public const int AUDIOPROFILER_FLAGS_PAUSED = 4;

		public const int AUDIOPROFILER_FLAGS_MUTED = 8;

		public const int AUDIOPROFILER_FLAGS_VIRTUAL = 16;

		public const int AUDIOPROFILER_FLAGS_ONESHOT = 32;

		public const int AUDIOPROFILER_FLAGS_GROUP = 64;

		public const int AUDIOPROFILER_FLAGS_STREAM = 128;

		public const int AUDIOPROFILER_FLAGS_COMPRESSED = 256;

		public const int AUDIOPROFILER_FLAGS_LOOPED = 512;

		public const int AUDIOPROFILER_FLAGS_OPENMEMORY = 1024;

		public const int AUDIOPROFILER_FLAGS_OPENMEMORYPOINT = 2048;

		public const int AUDIOPROFILER_FLAGS_OPENUSER = 4096;

		public const int AUDIOPROFILER_FLAGS_NONBLOCKING = 8192;

		private static string FormatDb(float vol)
		{
			string result;
			if (vol == 0f)
			{
				result = "-∞ dB";
			}
			else
			{
				result = string.Format("{0:0.00} dB", 20f * Mathf.Log10(vol));
			}
			return result;
		}

		public static string GetColumnString(AudioProfilerGroupInfoWrapper info, AudioProfilerGroupInfoHelper.ColumnIndices index)
		{
			bool flag = (info.info.flags & 1) != 0;
			bool flag2 = (info.info.flags & 64) != 0;
			string result;
			switch (index)
			{
			case AudioProfilerGroupInfoHelper.ColumnIndices.ObjectName:
				result = info.objectName;
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.AssetName:
				result = info.assetName;
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.Volume:
				result = AudioProfilerGroupInfoHelper.FormatDb(info.info.volume);
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.Audibility:
				result = ((!flag2) ? AudioProfilerGroupInfoHelper.FormatDb(info.info.audibility) : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.PlayCount:
				result = ((!flag2) ? info.info.playCount.ToString() : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.Is3D:
				result = ((!flag2) ? ((!flag) ? "NO" : (((info.info.flags & 2) == 0) ? "YES" : "Spatial")) : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsPaused:
				result = ((!flag2) ? (((info.info.flags & 4) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsMuted:
				result = ((!flag2) ? (((info.info.flags & 8) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsVirtual:
				result = ((!flag2) ? (((info.info.flags & 16) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsOneShot:
				result = ((!flag2) ? (((info.info.flags & 32) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsLooped:
				result = ((!flag2) ? (((info.info.flags & 512) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.DistanceToListener:
				result = ((!flag2) ? (flag ? ((info.info.distanceToListener < 1000f) ? string.Format("{0:0.00} m", info.info.distanceToListener) : string.Format("{0:0.00} km", info.info.distanceToListener * 0.001f)) : "N/A") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.MinDist:
				result = ((!flag2) ? (flag ? ((info.info.minDist < 1000f) ? string.Format("{0:0.00} m", info.info.minDist) : string.Format("{0:0.00} km", info.info.minDist * 0.001f)) : "N/A") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.MaxDist:
				result = ((!flag2) ? (flag ? ((info.info.maxDist < 1000f) ? string.Format("{0:0.00} m", info.info.maxDist) : string.Format("{0:0.00} km", info.info.maxDist * 0.001f)) : "N/A") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.Time:
				result = ((!flag2) ? string.Format("{0:0.00} s", info.info.time) : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.Duration:
				result = ((!flag2) ? string.Format("{0:0.00} s", info.info.duration) : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.Frequency:
				result = ((!flag2) ? ((info.info.frequency < 1000f) ? string.Format("{0:0.00} Hz", info.info.frequency) : string.Format("{0:0.00} kHz", info.info.frequency * 0.001f)) : string.Format("{0:0.00} x", info.info.frequency));
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsStream:
				result = ((!flag2) ? (((info.info.flags & 128) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsCompressed:
				result = ((!flag2) ? (((info.info.flags & 256) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsNonBlocking:
				result = ((!flag2) ? (((info.info.flags & 8192) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenUser:
				result = ((!flag2) ? (((info.info.flags & 4096) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenMemory:
				result = ((!flag2) ? (((info.info.flags & 1024) == 0) ? "NO" : "YES") : "");
				break;
			case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenMemoryPoint:
				result = ((!flag2) ? (((info.info.flags & 2048) == 0) ? "NO" : "YES") : "");
				break;
			default:
				result = "Unknown";
				break;
			}
			return result;
		}

		public static int GetLastColumnIndex()
		{
			return (!Unsupported.IsDeveloperBuild()) ? 15 : 22;
		}
	}
}
