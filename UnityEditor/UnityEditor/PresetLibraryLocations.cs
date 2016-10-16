using System;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal static class PresetLibraryLocations
	{
		public static string defaultLibraryLocation
		{
			get
			{
				return PresetLibraryLocations.GetDefaultFilePathForFileLocation(PresetFileLocation.PreferencesFolder);
			}
		}

		public static string defaultPresetLibraryPath
		{
			get
			{
				return Path.Combine(PresetLibraryLocations.defaultLibraryLocation, PresetLibraryLocations.defaultLibraryName);
			}
		}

		public static string defaultLibraryName
		{
			get
			{
				return "Default";
			}
		}

		public static List<string> GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation fileLocation, string fileExtensionWithoutDot)
		{
			List<string> directoryPaths = PresetLibraryLocations.GetDirectoryPaths(fileLocation);
			List<string> filesWithExentionFromFolders = PresetLibraryLocations.GetFilesWithExentionFromFolders(directoryPaths, fileExtensionWithoutDot);
			for (int i = 0; i < filesWithExentionFromFolders.Count; i++)
			{
				filesWithExentionFromFolders[i] = PresetLibraryLocations.ConvertToUnitySeperators(filesWithExentionFromFolders[i]);
			}
			return filesWithExentionFromFolders;
		}

		public static string GetDefaultFilePathForFileLocation(PresetFileLocation fileLocation)
		{
			if (fileLocation == PresetFileLocation.PreferencesFolder)
			{
				return InternalEditorUtility.unityPreferencesFolder + "/Presets/";
			}
			if (fileLocation != PresetFileLocation.ProjectFolder)
			{
				Debug.LogError("Enum not handled!");
				return string.Empty;
			}
			return "Assets/Editor/";
		}

		private static List<string> GetDirectoryPaths(PresetFileLocation fileLocation)
		{
			List<string> list = new List<string>();
			if (fileLocation != PresetFileLocation.PreferencesFolder)
			{
				if (fileLocation != PresetFileLocation.ProjectFolder)
				{
					Debug.LogError("Enum not handled!");
				}
				else
				{
					string[] directories = Directory.GetDirectories("Assets/", "Editor", SearchOption.AllDirectories);
					list.AddRange(directories);
				}
			}
			else
			{
				list.Add(PresetLibraryLocations.GetDefaultFilePathForFileLocation(PresetFileLocation.PreferencesFolder));
			}
			return list;
		}

		private static List<string> GetFilesWithExentionFromFolders(List<string> folderPaths, string fileExtensionWithoutDot)
		{
			List<string> list = new List<string>();
			foreach (string current in folderPaths)
			{
				string[] files = Directory.GetFiles(current, "*." + fileExtensionWithoutDot);
				list.AddRange(files);
			}
			return list;
		}

		public static PresetFileLocation GetFileLocationFromPath(string path)
		{
			if (path.Contains(InternalEditorUtility.unityPreferencesFolder))
			{
				return PresetFileLocation.PreferencesFolder;
			}
			if (path.Contains("Assets/"))
			{
				return PresetFileLocation.ProjectFolder;
			}
			Debug.LogError("Could not determine preset file location type " + path);
			return PresetFileLocation.ProjectFolder;
		}

		private static string ConvertToUnitySeperators(string path)
		{
			return path.Replace('\\', '/');
		}

		public static string GetParticleCurveLibraryExtension(bool singleCurve, bool signedRange)
		{
			string text = "particle";
			if (singleCurve)
			{
				text += "Curves";
			}
			else
			{
				text += "DoubleCurves";
			}
			if (signedRange)
			{
				text += "Signed";
			}
			else
			{
				text += string.Empty;
			}
			return text;
		}

		public static string GetCurveLibraryExtension(bool normalized)
		{
			if (normalized)
			{
				return "curvesNormalized";
			}
			return "curves";
		}
	}
}
