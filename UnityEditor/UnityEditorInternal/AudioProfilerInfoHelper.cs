using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AudioProfilerInfoHelper
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

		public class AudioProfilerInfoComparer : IComparer<AudioProfilerInfoWrapper>
		{
			public AudioProfilerInfoHelper.ColumnIndices primarySortKey;

			public AudioProfilerInfoHelper.ColumnIndices secondarySortKey;

			public bool sortByDescendingOrder;

			public AudioProfilerInfoComparer(AudioProfilerInfoHelper.ColumnIndices primarySortKey, AudioProfilerInfoHelper.ColumnIndices secondarySortKey, bool sortByDescendingOrder)
			{
				this.primarySortKey = primarySortKey;
				this.secondarySortKey = secondarySortKey;
				this.sortByDescendingOrder = sortByDescendingOrder;
			}

			private int CompareInternal(AudioProfilerInfoWrapper a, AudioProfilerInfoWrapper b, AudioProfilerInfoHelper.ColumnIndices key)
			{
				int num = 0;
				switch (key)
				{
				case AudioProfilerInfoHelper.ColumnIndices.ObjectName:
					num = a.objectName.CompareTo(b.objectName);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.AssetName:
					num = a.assetName.CompareTo(b.assetName);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.Volume:
					num = a.info.volume.CompareTo(b.info.volume);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.Audibility:
					num = a.info.audibility.CompareTo(b.info.audibility);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.PlayCount:
					num = a.info.playCount.CompareTo(b.info.playCount);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.Is3D:
					num = (a.info.flags & 1).CompareTo(b.info.flags & 1) + (a.info.flags & 2).CompareTo(b.info.flags & 2) * 2;
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsPaused:
					num = (a.info.flags & 4).CompareTo(b.info.flags & 4);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsMuted:
					num = (a.info.flags & 8).CompareTo(b.info.flags & 8);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsVirtual:
					num = (a.info.flags & 16).CompareTo(b.info.flags & 16);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsOneShot:
					num = (a.info.flags & 32).CompareTo(b.info.flags & 32);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsLooped:
					num = (a.info.flags & 512).CompareTo(b.info.flags & 512);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.DistanceToListener:
					num = a.info.distanceToListener.CompareTo(b.info.distanceToListener);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.MinDist:
					num = a.info.minDist.CompareTo(b.info.minDist);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.MaxDist:
					num = a.info.maxDist.CompareTo(b.info.maxDist);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.Time:
					num = a.info.time.CompareTo(b.info.time);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.Duration:
					num = a.info.duration.CompareTo(b.info.duration);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.Frequency:
					num = a.info.frequency.CompareTo(b.info.frequency);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsStream:
					num = (a.info.flags & 128).CompareTo(b.info.flags & 128);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsCompressed:
					num = (a.info.flags & 256).CompareTo(b.info.flags & 256);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsNonBlocking:
					num = (a.info.flags & 8192).CompareTo(b.info.flags & 8192);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsOpenUser:
					num = (a.info.flags & 4096).CompareTo(b.info.flags & 4096);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsOpenMemory:
					num = (a.info.flags & 1024).CompareTo(b.info.flags & 1024);
					break;
				case AudioProfilerInfoHelper.ColumnIndices.IsOpenMemoryPoint:
					num = (a.info.flags & 2048).CompareTo(b.info.flags & 2048);
					break;
				}
				return (!this.sortByDescendingOrder) ? num : (-num);
			}

			public int Compare(AudioProfilerInfoWrapper a, AudioProfilerInfoWrapper b)
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
			if (vol == 0f)
			{
				return "-∞ dB";
			}
			return string.Format("{0:0.00} dB", 20f * Mathf.Log10(vol));
		}

		public static string GetColumnString(AudioProfilerInfoWrapper info, AudioProfilerInfoHelper.ColumnIndices index)
		{
			bool flag = (info.info.flags & 1) != 0;
			bool flag2 = (info.info.flags & 64) != 0;
			switch (index)
			{
			case AudioProfilerInfoHelper.ColumnIndices.ObjectName:
				return info.objectName;
			case AudioProfilerInfoHelper.ColumnIndices.AssetName:
				return info.assetName;
			case AudioProfilerInfoHelper.ColumnIndices.Volume:
				return AudioProfilerInfoHelper.FormatDb(info.info.volume);
			case AudioProfilerInfoHelper.ColumnIndices.Audibility:
				return (!flag2) ? AudioProfilerInfoHelper.FormatDb(info.info.audibility) : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.PlayCount:
				return (!flag2) ? info.info.playCount.ToString() : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.Is3D:
				return (!flag2) ? ((!flag) ? "NO" : (((info.info.flags & 2) == 0) ? "YES" : "Spatial")) : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsPaused:
				return (!flag2) ? (((info.info.flags & 4) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsMuted:
				return (!flag2) ? (((info.info.flags & 8) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsVirtual:
				return (!flag2) ? (((info.info.flags & 16) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsOneShot:
				return (!flag2) ? (((info.info.flags & 32) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsLooped:
				return (!flag2) ? (((info.info.flags & 512) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.DistanceToListener:
				return (!flag2) ? (flag ? ((info.info.distanceToListener < 1000f) ? string.Format("{0:0.00} m", info.info.distanceToListener) : string.Format("{0:0.00} km", info.info.distanceToListener * 0.001f)) : "N/A") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.MinDist:
				return (!flag2) ? (flag ? ((info.info.minDist < 1000f) ? string.Format("{0:0.00} m", info.info.minDist) : string.Format("{0:0.00} km", info.info.minDist * 0.001f)) : "N/A") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.MaxDist:
				return (!flag2) ? (flag ? ((info.info.maxDist < 1000f) ? string.Format("{0:0.00} m", info.info.maxDist) : string.Format("{0:0.00} km", info.info.maxDist * 0.001f)) : "N/A") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.Time:
				return (!flag2) ? string.Format("{0:0.00} s", info.info.time) : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.Duration:
				return (!flag2) ? string.Format("{0:0.00} s", info.info.duration) : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.Frequency:
				return (!flag2) ? ((info.info.frequency < 1000f) ? string.Format("{0:0.00} Hz", info.info.frequency) : string.Format("{0:0.00} kHz", info.info.frequency * 0.001f)) : string.Format("{0:0.00} x", info.info.frequency);
			case AudioProfilerInfoHelper.ColumnIndices.IsStream:
				return (!flag2) ? (((info.info.flags & 128) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsCompressed:
				return (!flag2) ? (((info.info.flags & 256) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsNonBlocking:
				return (!flag2) ? (((info.info.flags & 8192) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsOpenUser:
				return (!flag2) ? (((info.info.flags & 4096) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsOpenMemory:
				return (!flag2) ? (((info.info.flags & 1024) == 0) ? "NO" : "YES") : string.Empty;
			case AudioProfilerInfoHelper.ColumnIndices.IsOpenMemoryPoint:
				return (!flag2) ? (((info.info.flags & 2048) == 0) ? "NO" : "YES") : string.Empty;
			default:
				return "Unknown";
			}
		}

		public static int GetLastColumnIndex()
		{
			return (!Unsupported.IsDeveloperBuild()) ? 15 : 22;
		}
	}
}
