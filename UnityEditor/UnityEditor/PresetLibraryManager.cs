using System;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class PresetLibraryManager : ScriptableSingleton<PresetLibraryManager>
	{
		private class LibraryCache
		{
			private string m_Identifier;

			private List<ScriptableObject> m_LoadedLibraries = new List<ScriptableObject>();

			private List<string> m_LoadedLibraryIDs = new List<string>();

			public string identifier
			{
				get
				{
					return this.m_Identifier;
				}
			}

			public List<ScriptableObject> loadedLibraries
			{
				get
				{
					return this.m_LoadedLibraries;
				}
			}

			public List<string> loadedLibraryIDs
			{
				get
				{
					return this.m_LoadedLibraryIDs;
				}
			}

			public LibraryCache(string identifier)
			{
				this.m_Identifier = identifier;
			}

			public void UnloadScriptableObjects()
			{
				foreach (ScriptableObject current in this.m_LoadedLibraries)
				{
					UnityEngine.Object.DestroyImmediate(current);
				}
				this.m_LoadedLibraries.Clear();
				this.m_LoadedLibraryIDs.Clear();
			}
		}

		private static string s_LastError;

		private List<PresetLibraryManager.LibraryCache> m_LibraryCaches = new List<PresetLibraryManager.LibraryCache>();

		private HideFlags libraryHideFlag
		{
			get
			{
				return HideFlags.DontSave;
			}
		}

		public void GetAvailableLibraries<T>(ScriptableObjectSaveLoadHelper<T> helper, out List<string> preferencesLibs, out List<string> projectLibs) where T : ScriptableObject
		{
			preferencesLibs = PresetLibraryLocations.GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation.PreferencesFolder, helper.fileExtensionWithoutDot);
			projectLibs = PresetLibraryLocations.GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation.ProjectFolder, helper.fileExtensionWithoutDot);
		}

		private string GetLibaryNameFromPath(string filePath)
		{
			return Path.GetFileNameWithoutExtension(filePath);
		}

		public T CreateLibrary<T>(ScriptableObjectSaveLoadHelper<T> helper, string presetLibraryPathWithoutExtension) where T : ScriptableObject
		{
			string libaryNameFromPath = this.GetLibaryNameFromPath(presetLibraryPathWithoutExtension);
			if (!InternalEditorUtility.IsValidFileName(libaryNameFromPath))
			{
				string displayStringOfInvalidCharsOfFileName = InternalEditorUtility.GetDisplayStringOfInvalidCharsOfFileName(libaryNameFromPath);
				if (displayStringOfInvalidCharsOfFileName.Length > 0)
				{
					PresetLibraryManager.s_LastError = string.Format("A library filename cannot contain the following character{0}:  {1}", (displayStringOfInvalidCharsOfFileName.Length <= 1) ? string.Empty : "s", displayStringOfInvalidCharsOfFileName);
				}
				else
				{
					PresetLibraryManager.s_LastError = "Invalid filename";
				}
				return (T)((object)null);
			}
			if (this.GetLibrary<T>(helper, presetLibraryPathWithoutExtension) != null)
			{
				PresetLibraryManager.s_LastError = "Library '" + libaryNameFromPath + "' already exists! Ensure a unique name.";
				return (T)((object)null);
			}
			T t = helper.Create();
			t.hideFlags = this.libraryHideFlag;
			PresetLibraryManager.LibraryCache presetLibraryCache = this.GetPresetLibraryCache(helper.fileExtensionWithoutDot);
			presetLibraryCache.loadedLibraries.Add(t);
			presetLibraryCache.loadedLibraryIDs.Add(presetLibraryPathWithoutExtension);
			PresetLibraryManager.s_LastError = null;
			return t;
		}

		public T GetLibrary<T>(ScriptableObjectSaveLoadHelper<T> helper, string presetLibraryPathWithoutExtension) where T : ScriptableObject
		{
			PresetLibraryManager.LibraryCache presetLibraryCache = this.GetPresetLibraryCache(helper.fileExtensionWithoutDot);
			int i = 0;
			while (i < presetLibraryCache.loadedLibraryIDs.Count)
			{
				if (presetLibraryCache.loadedLibraryIDs[i] == presetLibraryPathWithoutExtension)
				{
					if (presetLibraryCache.loadedLibraries[i] != null)
					{
						return presetLibraryCache.loadedLibraries[i] as T;
					}
					presetLibraryCache.loadedLibraries.RemoveAt(i);
					presetLibraryCache.loadedLibraryIDs.RemoveAt(i);
					Debug.LogError("Invalid library detected: Reload " + presetLibraryCache.loadedLibraryIDs[i] + " from HDD");
					break;
				}
				else
				{
					i++;
				}
			}
			T t = helper.Load(presetLibraryPathWithoutExtension);
			if (t != null)
			{
				t.hideFlags = this.libraryHideFlag;
				presetLibraryCache.loadedLibraries.Add(t);
				presetLibraryCache.loadedLibraryIDs.Add(presetLibraryPathWithoutExtension);
				return t;
			}
			return (T)((object)null);
		}

		public void UnloadAllLibrariesFor<T>(ScriptableObjectSaveLoadHelper<T> helper) where T : ScriptableObject
		{
			for (int i = 0; i < this.m_LibraryCaches.Count; i++)
			{
				if (this.m_LibraryCaches[i].identifier == helper.fileExtensionWithoutDot)
				{
					this.m_LibraryCaches[i].UnloadScriptableObjects();
					this.m_LibraryCaches.RemoveAt(i);
					break;
				}
			}
		}

		public void SaveLibrary<T>(ScriptableObjectSaveLoadHelper<T> helper, T library, string presetLibraryPathWithoutExtension) where T : ScriptableObject
		{
			bool flag = File.Exists(presetLibraryPathWithoutExtension + "." + helper.fileExtensionWithoutDot);
			helper.Save(library, presetLibraryPathWithoutExtension);
			if (!flag)
			{
				AssetDatabase.Refresh();
			}
		}

		public string GetLastError()
		{
			string result = PresetLibraryManager.s_LastError;
			PresetLibraryManager.s_LastError = null;
			return result;
		}

		private PresetLibraryManager.LibraryCache GetPresetLibraryCache(string identifier)
		{
			foreach (PresetLibraryManager.LibraryCache current in this.m_LibraryCaches)
			{
				if (current.identifier == identifier)
				{
					return current;
				}
			}
			PresetLibraryManager.LibraryCache libraryCache = new PresetLibraryManager.LibraryCache(identifier);
			this.m_LibraryCaches.Add(libraryCache);
			return libraryCache;
		}
	}
}
