using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
			string result;
			if (projectBrowserIfExists == null)
			{
				result = "Assets";
			}
			else
			{
				result = projectBrowserIfExists.GetActiveFolderPath();
			}
			return result;
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

		public static void CreateScene()
		{
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateScene>(), "New Scene.unity", EditorGUIUtility.FindTexture("SceneAsset Icon"), null);
		}

		public static void CreatePrefab()
		{
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreatePrefab>(), "New Prefab.prefab", EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D, null);
		}

		private static void CreateScriptAsset(string templatePath, string destName)
		{
			string fileName = Path.GetFileName(templatePath);
			if (fileName.ToLower().Contains("editortest"))
			{
				string text = AssetDatabase.GetUniquePathNameAtSelectedPath(destName);
				if (!text.ToLower().Contains("/editor/"))
				{
					text = text.Substring(0, text.Length - destName.Length - 1);
					string text2 = Path.Combine(text, "Editor");
					if (!Directory.Exists(text2))
					{
						AssetDatabase.CreateFolder(text, "Editor");
					}
					text = Path.Combine(text2, destName);
					text = text.Replace("\\", "/");
				}
				destName = text;
			}
			string extension = Path.GetExtension(destName);
			Texture2D icon;
			if (extension != null)
			{
				if (extension == ".js")
				{
					icon = (EditorGUIUtility.IconContent("js Script Icon").image as Texture2D);
					goto IL_16F;
				}
				if (extension == ".cs")
				{
					icon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
					goto IL_16F;
				}
				if (extension == ".boo")
				{
					icon = (EditorGUIUtility.IconContent("boo Script Icon").image as Texture2D);
					goto IL_16F;
				}
				if (extension == ".shader")
				{
					icon = (EditorGUIUtility.IconContent("Shader Icon").image as Texture2D);
					goto IL_16F;
				}
			}
			icon = (EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D);
			IL_16F:
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

		private static void CreateSpritePolygon(int sides)
		{
			string str;
			switch (sides)
			{
			case 0:
				str = "Square";
				goto IL_8E;
			case 1:
			case 2:
			case 5:
				IL_29:
				if (sides == 42)
				{
					str = "Everythingon";
					goto IL_8E;
				}
				if (sides != 128)
				{
					str = "Polygon";
					goto IL_8E;
				}
				str = "Circle";
				goto IL_8E;
			case 3:
				str = "Triangle";
				goto IL_8E;
			case 4:
				str = "Diamond";
				goto IL_8E;
			case 6:
				str = "Hexagon";
				goto IL_8E;
			}
			goto IL_29;
			IL_8E:
			Texture2D icon = EditorGUIUtility.IconContent("Sprite Icon").image as Texture2D;
			DoCreateSpritePolygon doCreateSpritePolygon = ScriptableObject.CreateInstance<DoCreateSpritePolygon>();
			doCreateSpritePolygon.sides = sides;
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, doCreateSpritePolygon, str + ".png", icon, null);
		}

		internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
		{
			string fullPath = Path.GetFullPath(pathName);
			string text = File.ReadAllText(resourceFile);
			text = text.Replace("#NOTRIM#", "");
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
			text = text.Replace("#NAME#", fileNameWithoutExtension);
			string text2 = fileNameWithoutExtension.Replace(" ", "");
			text = text.Replace("#SCRIPTNAME#", text2);
			if (char.IsUpper(text2, 0))
			{
				text2 = char.ToLower(text2[0]) + text2.Substring(1);
				text = text.Replace("#SCRIPTNAME_LOWER#", text2);
			}
			else
			{
				text2 = "my" + char.ToUpper(text2[0]) + text2.Substring(1);
				text = text.Replace("#SCRIPTNAME_LOWER#", text2);
			}
			UTF8Encoding encoding = new UTF8Encoding(true);
			File.WriteAllText(fullPath, text, encoding);
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
			string title = "";
			if (ProjectWindowUtil.IsFavoritesItem(draggedInstanceID))
			{
				DragAndDrop.SetGenericData(ProjectWindowUtil.k_DraggingFavoriteGenericData, draggedInstanceID);
				DragAndDrop.objectReferences = new UnityEngine.Object[0];
			}
			else
			{
				bool flag = ProjectWindowUtil.IsFolder(draggedInstanceID);
				DragAndDrop.objectReferences = ProjectWindowUtil.GetDragAndDropObjects(draggedInstanceID, selectedInstanceIDs);
				DragAndDrop.SetGenericData(ProjectWindowUtil.k_IsFolderGenericData, (!flag) ? "" : "isFolder");
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
			List<UnityEngine.Object> list = new List<UnityEngine.Object>(selectedInstanceIDs.Count);
			if (selectedInstanceIDs.Contains(draggedInstanceID))
			{
				for (int i = 0; i < selectedInstanceIDs.Count; i++)
				{
					UnityEngine.Object objectFromInstanceID = InternalEditorUtility.GetObjectFromInstanceID(selectedInstanceIDs[i]);
					if (objectFromInstanceID != null)
					{
						list.Add(objectFromInstanceID);
					}
				}
			}
			else
			{
				UnityEngine.Object objectFromInstanceID2 = InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID);
				if (objectFromInstanceID2 != null)
				{
					list.Add(objectFromInstanceID2);
				}
			}
			return list.ToArray();
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
			string[] result;
			if (!string.IsNullOrEmpty(assetPath2))
			{
				if (list.Contains(assetPath2))
				{
					result = list.ToArray();
				}
				else
				{
					result = new string[]
					{
						assetPath2
					};
				}
			}
			else
			{
				result = new string[0];
			}
			return result;
		}

		public static int[] GetAncestors(int instanceID)
		{
			List<int> list = new List<int>();
			int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(AssetDatabase.GetAssetPath(instanceID));
			bool flag = mainAssetInstanceID != instanceID;
			if (flag)
			{
				list.Add(mainAssetInstanceID);
			}
			string containingFolder = ProjectWindowUtil.GetContainingFolder(AssetDatabase.GetAssetPath(mainAssetInstanceID));
			while (!string.IsNullOrEmpty(containingFolder))
			{
				int mainAssetInstanceID2 = AssetDatabase.GetMainAssetInstanceID(containingFolder);
				list.Add(mainAssetInstanceID2);
				containingFolder = ProjectWindowUtil.GetContainingFolder(AssetDatabase.GetAssetPath(mainAssetInstanceID2));
			}
			return list.ToArray();
		}

		public static bool IsFolder(int instanceID)
		{
			return AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(instanceID));
		}

		public static string GetContainingFolder(string path)
		{
			string result;
			if (string.IsNullOrEmpty(path))
			{
				result = null;
			}
			else
			{
				path = path.Trim(new char[]
				{
					'/'
				});
				int num = path.LastIndexOf("/", StringComparison.Ordinal);
				if (num != -1)
				{
					result = path.Substring(0, num);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static string[] GetBaseFolders(string[] folders)
		{
			string[] result;
			if (folders.Length <= 1)
			{
				result = folders;
			}
			else
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>(folders);
				for (int i = 0; i < list2.Count; i++)
				{
					list2[i] = list2[i].Trim(new char[]
					{
						'/'
					});
				}
				list2.Sort();
				for (int j = 0; j < list2.Count; j++)
				{
					if (!list2[j].EndsWith("/"))
					{
						list2[j] += "/";
					}
				}
				string text = list2[0];
				list.Add(text);
				for (int k = 1; k < list2.Count; k++)
				{
					if (list2[k].IndexOf(text, StringComparison.Ordinal) != 0)
					{
						list.Add(list2[k]);
						text = list2[k];
					}
				}
				for (int l = 0; l < list.Count; l++)
				{
					list[l] = list[l].Trim(new char[]
					{
						'/'
					});
				}
				result = list.ToArray();
			}
			return result;
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
