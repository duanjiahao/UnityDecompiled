using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEditor
{
	public sealed class AssetDatabase
	{
		public delegate void ImportPackageCallback(string packageName);

		public delegate void ImportPackageFailedCallback(string packageName, string errorMessage);

		public static event AssetDatabase.ImportPackageCallback importPackageStarted
		{
			add
			{
				AssetDatabase.ImportPackageCallback importPackageCallback = AssetDatabase.importPackageStarted;
				AssetDatabase.ImportPackageCallback importPackageCallback2;
				do
				{
					importPackageCallback2 = importPackageCallback;
					importPackageCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageCallback>(ref AssetDatabase.importPackageStarted, (AssetDatabase.ImportPackageCallback)Delegate.Combine(importPackageCallback2, value), importPackageCallback);
				}
				while (importPackageCallback != importPackageCallback2);
			}
			remove
			{
				AssetDatabase.ImportPackageCallback importPackageCallback = AssetDatabase.importPackageStarted;
				AssetDatabase.ImportPackageCallback importPackageCallback2;
				do
				{
					importPackageCallback2 = importPackageCallback;
					importPackageCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageCallback>(ref AssetDatabase.importPackageStarted, (AssetDatabase.ImportPackageCallback)Delegate.Remove(importPackageCallback2, value), importPackageCallback);
				}
				while (importPackageCallback != importPackageCallback2);
			}
		}

		public static event AssetDatabase.ImportPackageCallback importPackageCompleted
		{
			add
			{
				AssetDatabase.ImportPackageCallback importPackageCallback = AssetDatabase.importPackageCompleted;
				AssetDatabase.ImportPackageCallback importPackageCallback2;
				do
				{
					importPackageCallback2 = importPackageCallback;
					importPackageCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageCallback>(ref AssetDatabase.importPackageCompleted, (AssetDatabase.ImportPackageCallback)Delegate.Combine(importPackageCallback2, value), importPackageCallback);
				}
				while (importPackageCallback != importPackageCallback2);
			}
			remove
			{
				AssetDatabase.ImportPackageCallback importPackageCallback = AssetDatabase.importPackageCompleted;
				AssetDatabase.ImportPackageCallback importPackageCallback2;
				do
				{
					importPackageCallback2 = importPackageCallback;
					importPackageCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageCallback>(ref AssetDatabase.importPackageCompleted, (AssetDatabase.ImportPackageCallback)Delegate.Remove(importPackageCallback2, value), importPackageCallback);
				}
				while (importPackageCallback != importPackageCallback2);
			}
		}

		public static event AssetDatabase.ImportPackageCallback importPackageCancelled
		{
			add
			{
				AssetDatabase.ImportPackageCallback importPackageCallback = AssetDatabase.importPackageCancelled;
				AssetDatabase.ImportPackageCallback importPackageCallback2;
				do
				{
					importPackageCallback2 = importPackageCallback;
					importPackageCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageCallback>(ref AssetDatabase.importPackageCancelled, (AssetDatabase.ImportPackageCallback)Delegate.Combine(importPackageCallback2, value), importPackageCallback);
				}
				while (importPackageCallback != importPackageCallback2);
			}
			remove
			{
				AssetDatabase.ImportPackageCallback importPackageCallback = AssetDatabase.importPackageCancelled;
				AssetDatabase.ImportPackageCallback importPackageCallback2;
				do
				{
					importPackageCallback2 = importPackageCallback;
					importPackageCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageCallback>(ref AssetDatabase.importPackageCancelled, (AssetDatabase.ImportPackageCallback)Delegate.Remove(importPackageCallback2, value), importPackageCallback);
				}
				while (importPackageCallback != importPackageCallback2);
			}
		}

		public static event AssetDatabase.ImportPackageFailedCallback importPackageFailed
		{
			add
			{
				AssetDatabase.ImportPackageFailedCallback importPackageFailedCallback = AssetDatabase.importPackageFailed;
				AssetDatabase.ImportPackageFailedCallback importPackageFailedCallback2;
				do
				{
					importPackageFailedCallback2 = importPackageFailedCallback;
					importPackageFailedCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageFailedCallback>(ref AssetDatabase.importPackageFailed, (AssetDatabase.ImportPackageFailedCallback)Delegate.Combine(importPackageFailedCallback2, value), importPackageFailedCallback);
				}
				while (importPackageFailedCallback != importPackageFailedCallback2);
			}
			remove
			{
				AssetDatabase.ImportPackageFailedCallback importPackageFailedCallback = AssetDatabase.importPackageFailed;
				AssetDatabase.ImportPackageFailedCallback importPackageFailedCallback2;
				do
				{
					importPackageFailedCallback2 = importPackageFailedCallback;
					importPackageFailedCallback = Interlocked.CompareExchange<AssetDatabase.ImportPackageFailedCallback>(ref AssetDatabase.importPackageFailed, (AssetDatabase.ImportPackageFailedCallback)Delegate.Remove(importPackageFailedCallback2, value), importPackageFailedCallback);
				}
				while (importPackageFailedCallback != importPackageFailedCallback2);
			}
		}

		internal static extern bool isLocked
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool Contains(UnityEngine.Object obj)
		{
			return AssetDatabase.Contains(obj.GetInstanceID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Contains(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CreateFolder(string parentFolder, string newFolderName);

		public static bool IsMainAsset(UnityEngine.Object obj)
		{
			return AssetDatabase.IsMainAsset(obj.GetInstanceID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsMainAsset(int instanceID);

		public static bool IsSubAsset(UnityEngine.Object obj)
		{
			return AssetDatabase.IsSubAsset(obj.GetInstanceID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSubAsset(int instanceID);

		public static bool IsForeignAsset(UnityEngine.Object obj)
		{
			return AssetDatabase.IsForeignAsset(obj.GetInstanceID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsForeignAsset(int instanceID);

		public static bool IsNativeAsset(UnityEngine.Object obj)
		{
			return AssetDatabase.IsNativeAsset(obj.GetInstanceID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsNativeAsset(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GenerateUniqueAssetPath(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartAssetEditing();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopAssetEditing();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ValidateMoveAsset(string oldPath, string newPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string MoveAsset(string oldPath, string newPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string RenameAsset(string pathName, string newName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveAssetToTrash(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DeleteAsset(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ImportAsset(string path, [DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);

		[ExcludeFromDocs]
		public static void ImportAsset(string path)
		{
			ImportAssetOptions options = ImportAssetOptions.Default;
			AssetDatabase.ImportAsset(path, options);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyAsset(string path, string newPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool WriteImportSettingsIfDirty(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubFolders(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidFolder(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CreateAsset(UnityEngine.Object asset, string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CreateAssetFromObjects(UnityEngine.Object[] assets, string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddObjectToAsset(UnityEngine.Object objectToAdd, string path);

		public static void AddObjectToAsset(UnityEngine.Object objectToAdd, UnityEngine.Object assetObject)
		{
			AssetDatabase.AddObjectToAsset_OBJ_Internal(objectToAdd, assetObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AddInstanceIDToAssetWithRandomFileId(int instanceIDToAdd, UnityEngine.Object assetObject, bool hide);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddObjectToAsset_OBJ_Internal(UnityEngine.Object newAsset, UnityEngine.Object sameAssetFile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAssetPath(UnityEngine.Object assetObject);

		public static string GetAssetPath(int instanceID)
		{
			return AssetDatabase.GetAssetPathFromInstanceID(instanceID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAssetPathFromInstanceID(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMainAssetInstanceID(string assetPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAssetOrScenePath(UnityEngine.Object assetObject);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetTextMetaFilePathFromAssetPath(string path);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAssetPathFromTextMetaFilePath(string path);

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object LoadAssetAtPath(string assetPath, Type type);

		public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object
		{
			return (T)((object)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object LoadMainAssetAtPath(string assetPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type GetMainAssetTypeAtPath(string assetPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsMainAssetAtPathLoaded(string assetPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] LoadAllAssetRepresentationsAtPath(string assetPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] LoadAllAssetsAtPath(string assetPath);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Refresh([DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);

		[ExcludeFromDocs]
		public static void Refresh()
		{
			ImportAssetOptions options = ImportAssetOptions.Default;
			AssetDatabase.Refresh(options);
		}

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string AssetPathToGUID(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GUIDToAssetPath(string guid);

		public static Hash128 GetAssetDependencyHash(string path)
		{
			Hash128 result;
			AssetDatabase.INTERNAL_CALL_GetAssetDependencyHash(path, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetAssetDependencyHash(string path, out Hash128 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetInstanceIDFromGUID(string guid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SaveAssets();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture GetCachedIcon(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLabels(UnityEngine.Object obj, string[] labels);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetLabels(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearLabels(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] MatchLabelsPartial(UnityEngine.Object obj, string partial);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAllAssetBundleNames();

		[Obsolete("Method GetAssetBundleNames has been deprecated. Use GetAllAssetBundleNames instead.")]
		public string[] GetAssetBundleNames()
		{
			return AssetDatabase.GetAllAssetBundleNames();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetAllAssetBundleNamesWithoutVariant();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetAllAssetBundleVariants();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetUnusedAssetBundleNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RemoveAssetBundleName(string assetBundleName, bool forceRemove);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveUnusedAssetBundleNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAssetPathsFromAssetBundle(string assetBundleName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAssetPathsFromAssetBundleAndAssetName(string assetBundleName, string assetName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAssetBundleDependencies(string assetBundleName, bool recursive);

		public static string[] GetDependencies(string pathName)
		{
			return AssetDatabase.GetDependencies(pathName, true);
		}

		public static string[] GetDependencies(string pathName, bool recursive)
		{
			return AssetDatabase.GetDependencies(new string[]
			{
				pathName
			}, recursive);
		}

		public static string[] GetDependencies(string[] pathNames)
		{
			return AssetDatabase.GetDependencies(pathNames, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetDependencies(string[] pathNames, bool recursive);

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
			if (string.IsNullOrEmpty(packagePath))
			{
				throw new ArgumentException("Path can not be empty or null", "packagePath");
			}
			string packageIconPath;
			bool allowReInstall;
			ImportPackageItem[] array = PackageUtility.ExtractAndPrepareAssetList(packagePath, out packageIconPath, out allowReInstall);
			if (array != null)
			{
				if (interactive)
				{
					PackageImport.ShowImportPackage(packagePath, array, packageIconPath, allowReInstall);
				}
				else
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(packagePath);
					PackageUtility.ImportPackageAssets(fileNameWithoutExtension, array, false);
				}
			}
		}

		internal static void ImportPackageImmediately(string packagePath)
		{
			if (string.IsNullOrEmpty(packagePath))
			{
				throw new ArgumentException("Path can not be empty or null", "packagePath");
			}
			string text;
			bool flag;
			ImportPackageItem[] array = PackageUtility.ExtractAndPrepareAssetList(packagePath, out text, out flag);
			if (array != null && array.Length != 0)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(packagePath);
				PackageUtility.ImportPackageAssetsImmediately(fileNameWithoutExtension, array, false);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetUniquePathNameAtSelectedPath(string fileName);

		public static bool IsOpenForEdit(UnityEngine.Object assetObject)
		{
			string assetOrScenePath = AssetDatabase.GetAssetOrScenePath(assetObject);
			return AssetDatabase.IsOpenForEdit(assetOrScenePath);
		}

		public static bool IsOpenForEdit(string assetOrMetaFilePath)
		{
			string text;
			return AssetDatabase.IsOpenForEdit(assetOrMetaFilePath, out text);
		}

		public static bool IsOpenForEdit(UnityEngine.Object assetObject, out string message)
		{
			string assetOrScenePath = AssetDatabase.GetAssetOrScenePath(assetObject);
			return AssetDatabase.IsOpenForEdit(assetOrScenePath, out message);
		}

		public static bool IsOpenForEdit(string assetOrMetaFilePath, out string message)
		{
			return AssetModificationProcessorInternal.IsOpenForEdit(assetOrMetaFilePath, out message);
		}

		public static bool IsMetaFileOpenForEdit(UnityEngine.Object assetObject)
		{
			string text;
			return AssetDatabase.IsMetaFileOpenForEdit(assetObject, out text);
		}

		public static bool IsMetaFileOpenForEdit(UnityEngine.Object assetObject, out string message)
		{
			string assetOrScenePath = AssetDatabase.GetAssetOrScenePath(assetObject);
			string textMetaFilePathFromAssetPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetOrScenePath);
			return AssetDatabase.IsOpenForEdit(textMetaFilePathFromAssetPath, out message);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetBuiltinExtraResource(Type type, string path);

		public static T GetBuiltinExtraResource<T>(string path) where T : UnityEngine.Object
		{
			return (T)((object)AssetDatabase.GetBuiltinExtraResource(typeof(T), path));
		}

		[RequiredByNativeCode]
		private static void Internal_CallImportPackageStarted(string packageName)
		{
			if (AssetDatabase.importPackageStarted != null)
			{
				AssetDatabase.importPackageStarted(packageName);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_CallImportPackageCompleted(string packageName)
		{
			if (AssetDatabase.importPackageCompleted != null)
			{
				AssetDatabase.importPackageCompleted(packageName);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_CallImportPackageCancelled(string packageName)
		{
			if (AssetDatabase.importPackageCancelled != null)
			{
				AssetDatabase.importPackageCancelled(packageName);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_CallImportPackageFailed(string packageName, string errorMessage)
		{
			if (AssetDatabase.importPackageFailed != null)
			{
				AssetDatabase.importPackageFailed(packageName, errorMessage);
			}
		}

		[Obsolete("GetTextMetaDataPathFromAssetPath has been renamed to GetTextMetaFilePathFromAssetPath (UnityUpgradable) -> GetTextMetaFilePathFromAssetPath(*)")]
		public static string GetTextMetaDataPathFromAssetPath(string path)
		{
			return null;
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
			string[] result;
			if (searchFilter.folders != null && searchFilter.folders.Length > 0)
			{
				result = AssetDatabase.SearchInFolders(searchFilter);
			}
			else
			{
				result = AssetDatabase.SearchAllAssets(searchFilter);
			}
			return result;
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
