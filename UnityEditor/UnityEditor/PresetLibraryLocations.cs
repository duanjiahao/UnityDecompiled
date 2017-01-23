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
			string result;
			if (fileLocation != PresetFileLocation.PreferencesFolder)
			{
				if (fileLocation != PresetFileLocation.ProjectFolder)
				{
					Debug.LogError("Enum not handled!");
					result = "";
				}
				else
				{
					result = "Assets/Editor/";
				}
			}
			else
			{
				result = InternalEditorUtility.unityPreferencesFolder + "/Presets/";
			}
			return result;
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
			PresetFileLocation result;
			if (path.Contains(InternalEditorUtility.unityPreferencesFolder))
			{
				result = PresetFileLocation.PreferencesFolder;
			}
			else if (path.Contains("Assets/"))
			{
				result = PresetFileLocation.ProjectFolder;
			}
			else
			{
				Debug.LogError("Could not determine preset file location type " + path);
				result = PresetFileLocation.ProjectFolder;
			}
			return result;
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
				text += "";
			}
			return text;
		}

		public static string GetCurveLibraryExtension(bool normalized)
		{
			string result;
			if (normalized)
			{
				result = "curvesNormalized";
			}
			else
			{
				result = "curves";
			}
			return result;
		}
	}
}
