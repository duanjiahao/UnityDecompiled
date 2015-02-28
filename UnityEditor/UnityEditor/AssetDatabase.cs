using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngineInternal;
namespace UnityEditor
{
	public sealed class AssetDatabase
	{
		internal static extern bool isLocked
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static bool Contains(UnityEngine.Object obj)
		{
			return AssetDatabase.Contains(obj.GetInstanceID());
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Contains(int instanceID);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CreateFolder(string parentFolder, string newFolderName);
		public static bool IsMainAsset(UnityEngine.Object obj)
		{
			return AssetDatabase.IsMainAsset(obj.GetInstanceID());
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsMainAsset(int instanceID);
		public static bool IsSubAsset(UnityEngine.Object obj)
		{
			return AssetDatabase.IsSubAsset(obj.GetInstanceID());
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSubAsset(int instanceID);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GenerateUniqueAssetPath(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartAssetEditing();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopAssetEditing();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ValidateMoveAsset(string oldPath, string newPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string MoveAsset(string oldPath, string newPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string RenameAsset(string pathName, string newName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveAssetToTrash(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DeleteAsset(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ImportAsset(string path, [DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);
		[ExcludeFromDocs]
		public static void ImportAsset(string path)
		{
			ImportAssetOptions options = ImportAssetOptions.Default;
			AssetDatabase.ImportAsset(path, options);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyAsset(string path, string newPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool WriteImportSettingsIfDirty(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubFolders(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidFolder(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CreateAsset(UnityEngine.Object asset, string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CreateAssetFromObjects(UnityEngine.Object[] assets, string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddObjectToAsset(UnityEngine.Object objectToAdd, string path);
		public static void AddObjectToAsset(UnityEngine.Object objectToAdd, UnityEngine.Object assetObject)
		{
			AssetDatabase.AddObjectToAsset_OBJ_Internal(objectToAdd, assetObject);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AddInstanceIDToAssetWithRandomFileId(int instanceIDToAdd, UnityEngine.Object assetObject, bool hide);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddObjectToAsset_OBJ_Internal(UnityEngine.Object newAsset, UnityEngine.Object sameAssetFile);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAssetPath(UnityEngine.Object assetObject);
		public static string GetAssetPath(int instanceID)
		{
			return AssetDatabase.GetAssetPathFromInstanceID(instanceID);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAssetPathFromInstanceID(int instanceID);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMainAssetInstanceID(string assetPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAssetOrScenePath(UnityEngine.Object assetObject);
		[Obsolete("GetTextMetaDataPathFromAssetPath has been renamed to GetTextMetaFilePathFromAssetPath (UnityUpgradable).", true)]
		public static string GetTextMetaDataPathFromAssetPath(string path)
		{
			return null;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetTextMetaFilePathFromAssetPath(string path);
		[WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object LoadAssetAtPath(string assetPath, Type type);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object LoadMainAssetAtPath(string assetPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] LoadAllAssetRepresentationsAtPath(string assetPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] LoadAllAssetsAtPath(string assetPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAllAssetPaths();
		[Obsolete("Please use AssetDatabase.Refresh instead", true)]
		public static void RefreshDelayed(ImportAssetOptions options)
		{
		}
		[Obsolete("Please use AssetDatabase.Refresh instead", true)]
		public static void RefreshDelayed()
		{
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Refresh([DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);
		[ExcludeFromDocs]
		public static void Refresh()
		{
			ImportAssetOptions options = ImportAssetOptions.Default;
			AssetDatabase.Refresh(options);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool OpenAsset(int instanceID, [DefaultValue("-1")] int lineNumber);
		[ExcludeFromDocs]
		public static bool OpenAsset(int instanceID)
		{
			int lineNumber = -1;
			return AssetDatabase.OpenAsset(instanceID, lineNumber);
		}
		[ExcludeFromDocs]
		public static bool OpenAsset(UnityEngine.Object target)
		{
			int lineNumber = -1;
			return AssetDatabase.OpenAsset(target, lineNumber);
		}
		public static bool OpenAsset(UnityEngine.Object target, [DefaultValue("-1")] int lineNumber)
		{
			return target && AssetDatabase.OpenAsset(target.GetInstanceID(), lineNumber);
		}
		public static bool OpenAsset(UnityEngine.Object[] objects)
		{
			bool result = true;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object target = objects[i];
				if (!AssetDatabase.OpenAsset(target))
				{
					result = false;
				}
			}
			return result;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string AssetPathToGUID(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GUIDToAssetPath(string guid);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetAssetHashFromPath(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetInstanceIDFromGUID(string guid);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SaveAssets();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture GetCachedIcon(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLabels(UnityEngine.Object obj, string[] labels);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_GetAllLabels(out string[] labels, out float[] scores);
		internal static Dictionary<string, float> GetAllLabels()
		{
			string[] array;
			float[] array2;
			AssetDatabase.INTERNAL_GetAllLabels(out array, out array2);
			Dictionary<string, float> dictionary = new Dictionary<string, float>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				dictionary[array[i]] = array2[i];
			}
			return dictionary;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetLabels(UnityEngine.Object obj);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearLabels(UnityEngine.Object obj);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] MatchLabelsPartial(UnityEngine.Object obj, string partial);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAllAssetBundleNames();
		[Obsolete("Method GetAssetBundleNames has been deprecated. Use GetAllAssetBundleNames instead.")]
		public string[] GetAssetBundleNames()
		{
			return AssetDatabase.GetAllAssetBundleNames();
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetAllAssetBundleNamesWithoutVariant();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetAllAssetBundleVariants();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetUnusedAssetBundleNames();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RemoveAssetBundleName(string assetBundleName, bool forceRemove);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveUnusedAssetBundleNames();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAssetPathsFromAssetBundle(string assetBundleName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAssetPathsFromAssetBundleAndAssetName(string assetBundleName, string assetName);
		private static string[] GetDependencies(string pathName)
		{
			return AssetDatabase.GetDependencies(new string[]
			{
				pathName
			});
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetDependencies(string[] pathNames);
		public static void ExportPackage(string assetPathName, string fileName)
		{
			AssetDatabase.ExportPackage(new string[]
			{
				assetPathName
			}, fileName, ExportPackageOptions.Default);
		}
		public static void ExportPackage(string assetPathName, string fileName, ExportPackageOptions flags)
		{
			AssetDatabase.ExportPackage(new string[]
			{
				assetPathName
			}, fileName, flags);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExportPackage(string[] assetPathNames, string fileName, [DefaultValue("ExportPackageOptions.Default")] ExportPackageOptions flags);
		[ExcludeFromDocs]
		public static void ExportPackage(string[] assetPathNames, string fileName)
		{
			ExportPackageOptions flags = ExportPackageOptions.Default;
			AssetDatabase.ExportPackage(assetPathNames, fileName, flags);
		}
		public static void ImportPackage(string packagePath, bool interactive)
		{
			string packageIconPath;
			AssetsItem[] array = AssetServer.ImportPackageStep1(packagePath, out packageIconPath);
			if (array == null)
			{
				return;
			}
			if (interactive)
			{
				PackageImport.ShowImportPackage(packagePath, array, packageIconPath);
			}
			else
			{
				AssetServer.ImportPackageStep2(array);
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetUniquePathNameAtSelectedPath(string fileName);
		public static bool IsOpenForEdit(UnityEngine.Object assetObject)
		{
			string assetOrScenePath = AssetDatabase.GetAssetOrScenePath(assetObject);
			return AssetDatabase.IsOpenForEdit(assetOrScenePath);
		}
		public static bool IsOpenForEdit(string assetPath)
		{
			string empty = string.Empty;
			return AssetDatabase.IsOpenForEdit(assetPath, out empty);
		}
		public static bool IsOpenForEdit(UnityEngine.Object assetObject, out string message)
		{
			string assetOrScenePath = AssetDatabase.GetAssetOrScenePath(assetObject);
			return AssetDatabase.IsOpenForEdit(assetOrScenePath, out message);
		}
		public static bool IsOpenForEdit(string assetPath, out string message)
		{
			return AssetModificationProcessorInternal.IsOpenForEdit(assetPath, out message);
		}
		[WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetBuiltinExtraResource(Type type, string path);
		public static T GetBuiltinExtraResource<T>(string path) where T : UnityEngine.Object
		{
			return (T)((object)AssetDatabase.GetBuiltinExtraResource(typeof(T), path));
		}
		public static string[] FindAssets(string filter)
		{
			return AssetDatabase.FindAssets(filter, null);
		}
		public static string[] FindAssets(string filter, string[] searchInFolders)
		{
			SearchFilter searchFilter = new SearchFilter();
			SearchUtility.ParseSearchString(filter, searchFilter);
			if (searchInFolders != null)
			{
				searchFilter.folders = searchInFolders;
			}
			return AssetDatabase.FindAssets(searchFilter);
		}
		private static string[] FindAssets(SearchFilter searchFilter)
		{
			if (searchFilter.folders != null && searchFilter.folders.Length > 0)
			{
				return AssetDatabase.SearchInFolders(searchFilter);
			}
			return AssetDatabase.SearchAllAssets(searchFilter);
		}
		private static string[] SearchAllAssets(SearchFilter searchFilter)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			hierarchyProperty.SetSearchFilter(searchFilter);
			hierarchyProperty.Reset();
			List<string> list = new List<string>();
			while (hierarchyProperty.Next(null))
			{
				list.Add(hierarchyProperty.guid);
			}
			return list.ToArray();
		}
		private static string[] SearchInFolders(SearchFilter searchFilter)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			List<string> list = new List<string>();
			string[] folders = searchFilter.folders;
			for (int i = 0; i < folders.Length; i++)
			{
				string text = folders[i];
				hierarchyProperty.SetSearchFilter(new SearchFilter());
				int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(text);
				if (hierarchyProperty.Find(mainAssetInstanceID, null))
				{
					hierarchyProperty.SetSearchFilter(searchFilter);
					int depth = hierarchyProperty.depth;
					int[] expanded = null;
					while (hierarchyProperty.NextWithDepthCheck(expanded, depth + 1))
					{
						list.Add(hierarchyProperty.guid);
					}
				}
				else
				{
					Debug.LogWarning("AssetDatabase.FindAssets: Folder not found: '" + text + "'");
				}
			}
			return list.ToArray();
		}
	}
}
