using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor.Collaboration
{
	internal static class SoftLockUIData
	{
		public enum SectionEnum
		{
			None,
			Inspector,
			Scene,
			ProjectBrowser
		}

		private static Dictionary<string, Texture> s_ImageCache = new Dictionary<string, Texture>();

		private static Dictionary<SoftLockUIData.SectionEnum, string> s_ImageNameCache = new Dictionary<SoftLockUIData.SectionEnum, string>();

		private const string kIconMipSuffix = " Icon";

		public static List<string> GetLocksNamesOnAsset(string assetGuid)
		{
			List<SoftLock> list = null;
			List<string> list2 = new List<string>();
			if (SoftLockData.TryGetLocksOnAssetGUID(assetGuid, out list))
			{
				foreach (SoftLock current in list)
				{
					list2.Add(current.displayName);
				}
			}
			return list2;
		}

		public static List<string> GetLocksNamesOnScene(Scene scene)
		{
			return SoftLockUIData.GetLockNamesOnScenePath(scene.path);
		}

		public static List<string> GetLockNamesOnScenePath(string scenePath)
		{
			string assetGuid = AssetDatabase.AssetPathToGUID(scenePath);
			return SoftLockUIData.GetLocksNamesOnAsset(assetGuid);
		}

		public static string GetSceneNameFromPath(string scenePath)
		{
			string result = "";
			if (scenePath != null)
			{
				result = scenePath;
			}
			return result;
		}

		public static List<List<string>> GetLockNamesOnScenes(List<Scene> scenes)
		{
			List<List<string>> list = new List<List<string>>();
			List<List<string>> result;
			if (scenes == null)
			{
				result = list;
			}
			else
			{
				foreach (Scene current in scenes)
				{
					List<string> locksNamesOnScene = SoftLockUIData.GetLocksNamesOnScene(current);
					list.Add(locksNamesOnScene);
				}
				result = list;
			}
			return result;
		}

		[DebuggerHidden]
		public static IEnumerable<KeyValuePair<string, List<string>>> GetLockNamesOnOpenScenes()
		{
			SoftLockUIData.<GetLockNamesOnOpenScenes>c__Iterator0 <GetLockNamesOnOpenScenes>c__Iterator = new SoftLockUIData.<GetLockNamesOnOpenScenes>c__Iterator0();
			SoftLockUIData.<GetLockNamesOnOpenScenes>c__Iterator0 expr_07 = <GetLockNamesOnOpenScenes>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}

		public static int CountOfLocksOnOpenScenes()
		{
			int num = 0;
			foreach (KeyValuePair<string, List<string>> current in SoftLockUIData.GetLockNamesOnOpenScenes())
			{
				num += current.Value.Count;
			}
			return num;
		}

		public static List<string> GetLockNamesOnObject(UnityEngine.Object objectWithGUID)
		{
			string assetGuid = null;
			AssetAccess.TryGetAssetGUIDFromObject(objectWithGUID, out assetGuid);
			return SoftLockUIData.GetLocksNamesOnAsset(assetGuid);
		}

		public static Texture GetIconForSection(SoftLockUIData.SectionEnum section)
		{
			string fileName = SoftLockUIData.IconNameForSection(section);
			return SoftLockUIData.GetIconForName(fileName);
		}

		private static string IconNameForSection(SoftLockUIData.SectionEnum section)
		{
			string text;
			string result;
			if (!SoftLockUIData.s_ImageNameCache.TryGetValue(section, out text))
			{
				switch (section)
				{
				case SoftLockUIData.SectionEnum.Inspector:
				case SoftLockUIData.SectionEnum.Scene:
					text = "SoftlockInline.png";
					break;
				case SoftLockUIData.SectionEnum.ProjectBrowser:
					text = string.Format("SoftlockProjectBrowser{0}", " Icon");
					break;
				default:
					result = null;
					return result;
				}
				SoftLockUIData.s_ImageNameCache.Add(section, text);
			}
			result = text;
			return result;
		}

		private static Texture GetIconForName(string fileName)
		{
			Texture result;
			if (string.IsNullOrEmpty(fileName))
			{
				result = null;
			}
			else
			{
				Texture texture;
				if (!SoftLockUIData.s_ImageCache.TryGetValue(fileName, out texture) || texture == null)
				{
					if (fileName.EndsWith(" Icon"))
					{
						texture = EditorGUIUtility.FindTexture(fileName);
					}
					else
					{
						texture = EditorGUIUtility.LoadIconRequired(fileName);
					}
					SoftLockUIData.s_ImageCache.Remove(fileName);
					SoftLockUIData.s_ImageCache.Add(fileName, texture);
				}
				result = texture;
			}
			return result;
		}
	}
}
