using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.ProjectWindowCallback;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	public class ProjectWindowUtil
	{
		internal static int k_FavoritesStartInstanceID = 1000000000;
		internal static string k_DraggingFavoriteGenericData = "DraggingFavorite";
		internal static string k_IsFolderGenericData = "IsFolder";
		[MenuItem("Assets/Create/GUI Skin", false, 601)]
		public static void CreateNewGUISkin()
		{
			GUISkin gUISkin = ScriptableObject.CreateInstance<GUISkin>();
			GUISkin gUISkin2 = Resources.GetBuiltinResource(typeof(GUISkin), "GameSkin/GameSkin.guiskin") as GUISkin;
			if (gUISkin2)
			{
				EditorUtility.CopySerialized(gUISkin2, gUISkin);
			}
			else
			{
				Debug.LogError("Internal error: unable to load builtin GUIskin");
			}
			ProjectWindowUtil.CreateAsset(gUISkin, "New GUISkin.guiskin");
		}
		internal static string GetActiveFolderPath()
		{
			ProjectBrowser projectBrowserIfExists = ProjectWindowUtil.GetProjectBrowserIfExists();
			if (projectBrowserIfExists == null)
			{
				return "Assets";
			}
			return projectBrowserIfExists.GetActiveFolderPath();
		}
		internal static void EndNameEditAction(EndNameEditAction action, int instanceId, string pathName, string resourceFile)
		{
			pathName = AssetDatabase.GenerateUniqueAssetPath(pathName);
			if (action != null)
			{
				action.Action(instanceId, pathName, resourceFile);
				action.CleanUp();
			}
		}
		public static void CreateAsset(UnityEngine.Object asset, string pathName)
		{
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), ScriptableObject.CreateInstance<DoCreateNewAsset>(), pathName, AssetPreview.GetMiniThumbnail(asset), null);
		}
		public static void CreateFolder()
		{
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateFolder>(), "New Folder", EditorGUIUtility.IconContent(EditorResourcesUtility.emptyFolderIconName).image as Texture2D, null);
		}
		public static void CreatePrefab()
		{
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreatePrefab>(), "New Prefab.prefab", EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D, null);
		}
		private static void CreateScriptAsset(string templatePath, string destName)
		{
			string extension = Path.GetExtension(destName);
			Texture2D icon;
			switch (extension)
			{
			case ".js":
				icon = (EditorGUIUtility.IconContent("js Script Icon").image as Texture2D);
				goto IL_105;
			case ".cs":
				icon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
				goto IL_105;
			case ".boo":
				icon = (EditorGUIUtility.IconContent("boo Script Icon").image as Texture2D);
				goto IL_105;
			case ".shader":
				icon = (EditorGUIUtility.IconContent("Shader Icon").image as Texture2D);
				goto IL_105;
			}
			icon = (EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D);
			IL_105:
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateScriptAsset>(), destName, icon, templatePath);
		}
		public static void ShowCreatedAsset(UnityEngine.Object o)
		{
			Selection.activeObject = o;
			if (o)
			{
				ProjectWindowUtil.FrameObjectInProjectWindow(o.GetInstanceID());
			}
		}
		private static void CreateAnimatorController()
		{
			Texture2D icon = EditorGUIUtility.IconContent("AnimatorController Icon").image as Texture2D;
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAnimatorController>(), "New Animator Controller.controller", icon, null);
		}
		private static void CreateAudioMixer()
		{
			Texture2D icon = EditorGUIUtility.IconContent("AudioMixerController Icon").image as Texture2D;
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAudioMixer>(), "NewAudioMixer.mixer", icon, null);
		}
		internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
		{
			string fullPath = Path.GetFullPath(pathName);
			StreamReader streamReader = new StreamReader(resourceFile);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
			text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
			string text2 = Regex.Replace(fileNameWithoutExtension, " ", string.Empty);
			text = Regex.Replace(text, "#SCRIPTNAME#", text2);
			if (char.IsUpper(text2, 0))
			{
				text2 = char.ToLower(text2[0]) + text2.Substring(1);
				text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", text2);
			}
			else
			{
				text2 = "my" + char.ToUpper(text2[0]) + text2.Substring(1);
				text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", text2);
			}
			bool encoderShouldEmitUTF8Identifier = true;
			bool throwOnInvalidBytes = false;
			UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
			bool append = false;
			StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
			streamWriter.Write(text);
			streamWriter.Close();
			AssetDatabase.ImportAsset(pathName);
			return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
		}
		public static void StartNameEditingIfProjectWindowExists(int instanceID, EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
		{
			ProjectBrowser projectBrowserIfExists = ProjectWindowUtil.GetProjectBrowserIfExists();
			if (projectBrowserIfExists)
			{
				projectBrowserIfExists.Focus();
				projectBrowserIfExists.BeginPreimportedNameEditing(instanceID, endAction, pathName, icon, resourceFile);
				projectBrowserIfExists.Repaint();
			}
			else
			{
				if (!pathName.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
				{
					pathName = "Assets/" + pathName;
				}
				ProjectWindowUtil.EndNameEditAction(endAction, instanceID, pathName, resourceFile);
				Selection.activeObject = EditorUtility.InstanceIDToObject(instanceID);
			}
		}
		private static ProjectBrowser GetProjectBrowserIfExists()
		{
			return ProjectBrowser.s_LastInteractedProjectBrowser;
		}
		internal static void FrameObjectInProjectWindow(int instanceID)
		{
			ProjectBrowser projectBrowserIfExists = ProjectWindowUtil.GetProjectBrowserIfExists();
			if (projectBrowserIfExists)
			{
				projectBrowserIfExists.FrameObject(instanceID, false);
			}
		}
		internal static bool IsFavoritesItem(int instanceID)
		{
			return instanceID >= ProjectWindowUtil.k_FavoritesStartInstanceID;
		}
		internal static void StartDrag(int draggedInstanceID, List<int> selectedInstanceIDs)
		{
			DragAndDrop.PrepareStartDrag();
			string title = string.Empty;
			if (ProjectWindowUtil.IsFavoritesItem(draggedInstanceID))
			{
				DragAndDrop.SetGenericData(ProjectWindowUtil.k_DraggingFavoriteGenericData, draggedInstanceID);
				DragAndDrop.objectReferences = new UnityEngine.Object[0];
			}
			else
			{
				bool flag = false;
				HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
				if (hierarchyProperty.Find(draggedInstanceID, null))
				{
					flag = hierarchyProperty.isFolder;
				}
				DragAndDrop.objectReferences = ProjectWindowUtil.GetDragAndDropObjects(draggedInstanceID, selectedInstanceIDs);
				DragAndDrop.SetGenericData(ProjectWindowUtil.k_IsFolderGenericData, (!flag) ? string.Empty : "isFolder");
				string[] dragAndDropPaths = ProjectWindowUtil.GetDragAndDropPaths(draggedInstanceID, selectedInstanceIDs);
				if (dragAndDropPaths.Length > 0)
				{
					DragAndDrop.paths = dragAndDropPaths;
				}
				if (DragAndDrop.objectReferences.Length > 1)
				{
					title = "<Multiple>";
				}
				else
				{
					title = ObjectNames.GetDragAndDropTitle(InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID));
				}
			}
			DragAndDrop.StartDrag(title);
		}
		internal static UnityEngine.Object[] GetDragAndDropObjects(int draggedInstanceID, List<int> selectedInstanceIDs)
		{
			if (selectedInstanceIDs.Contains(draggedInstanceID))
			{
				UnityEngine.Object[] array = new UnityEngine.Object[selectedInstanceIDs.Count];
				for (int i = 0; i < selectedInstanceIDs.Count; i++)
				{
					array[i] = InternalEditorUtility.GetObjectFromInstanceID(selectedInstanceIDs[i]);
				}
				return array;
			}
			return new UnityEngine.Object[]
			{
				InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID)
			};
		}
		internal static string[] GetDragAndDropPaths(int draggedInstanceID, List<int> selectedInstanceIDs)
		{
			List<string> list = new List<string>();
			foreach (int current in selectedInstanceIDs)
			{
				if (AssetDatabase.IsMainAsset(current))
				{
					string assetPath = AssetDatabase.GetAssetPath(current);
					list.Add(assetPath);
				}
			}
			string assetPath2 = AssetDatabase.GetAssetPath(draggedInstanceID);
			if (string.IsNullOrEmpty(assetPath2))
			{
				return new string[0];
			}
			if (list.Contains(assetPath2))
			{
				return list.ToArray();
			}
			return new string[]
			{
				assetPath2
			};
		}
		public static string[] GetBaseFolders(string[] folders)
		{
			if (folders.Length < 2)
			{
				return folders;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>(folders);
			list2.Sort();
			string text = list2[0];
			list.Add(text);
			for (int i = 1; i < list2.Count; i++)
			{
				if (list2[i].IndexOf(text) < 0)
				{
					list.Add(list2[i]);
					text = list2[i];
				}
			}
			return list.ToArray();
		}
		internal static void DuplicateSelectedAssets()
		{
			AssetDatabase.Refresh();
			UnityEngine.Object[] objects = Selection.objects;
			bool flag = true;
			UnityEngine.Object[] array = objects;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object @object = array[i];
				AnimationClip animationClip = @object as AnimationClip;
				if (animationClip == null || (animationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None || !AssetDatabase.Contains(animationClip))
				{
					flag = false;
				}
			}
			ArrayList arrayList = new ArrayList();
			bool flag2 = false;
			if (flag)
			{
				UnityEngine.Object[] array2 = objects;
				for (int j = 0; j < array2.Length; j++)
				{
					UnityEngine.Object object2 = array2[j];
					AnimationClip animationClip2 = object2 as AnimationClip;
					if (animationClip2 != null && (animationClip2.hideFlags & HideFlags.NotEditable) != HideFlags.None)
					{
						string path = AssetDatabase.GetAssetPath(object2);
						path = Path.Combine(Path.GetDirectoryName(path), animationClip2.name) + ".anim";
						string text = AssetDatabase.GenerateUniqueAssetPath(path);
						AnimationClip animationClip3 = new AnimationClip();
						EditorUtility.CopySerialized(animationClip2, animationClip3);
						AssetDatabase.CreateAsset(animationClip3, text);
						arrayList.Add(text);
					}
				}
			}
			else
			{
				UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
				UnityEngine.Object[] array3 = filtered;
				for (int k = 0; k < array3.Length; k++)
				{
					UnityEngine.Object assetObject = array3[k];
					string assetPath = AssetDatabase.GetAssetPath(assetObject);
					string text2 = AssetDatabase.GenerateUniqueAssetPath(assetPath);
					if (text2.Length != 0)
					{
						flag2 |= !AssetDatabase.CopyAsset(assetPath, text2);
					}
					else
					{
						flag2 |= true;
					}
					if (!flag2)
					{
						arrayList.Add(text2);
					}
				}
			}
			AssetDatabase.Refresh();
			UnityEngine.Object[] array4 = new UnityEngine.Object[arrayList.Count];
			for (int l = 0; l < arrayList.Count; l++)
			{
				array4[l] = AssetDatabase.LoadMainAssetAtPath(arrayList[l] as string);
			}
			Selection.objects = array4;
		}
	}
}
