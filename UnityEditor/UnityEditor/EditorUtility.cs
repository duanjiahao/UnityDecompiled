using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Scripting.Compilers;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class EditorUtility
	{
		public delegate void SelectMenuItemFunction(object userData, string[] options, int selected);

		public static extern bool audioMasterMute
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool audioProfilingEnabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool scriptCompilationFailed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RevealInFinder(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDirty(UnityEngine.Object target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LoadPlatformSupportModuleNativeDllInternal(string target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LoadPlatformSupportNativeLibrary(string nativeLibrary);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetDirtyIndex(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsDirty(int instanceID);

		public static bool LoadWindowLayout(string path)
		{
			bool newProjectLayoutWasCreated = false;
			return WindowLayout.LoadWindowLayout(path, newProjectLayoutWasCreated);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPersistent(UnityEngine.Object target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DisplayDialog(string title, string message, string ok, [DefaultValue("\"\"")] string cancel);

		[ExcludeFromDocs]
		public static bool DisplayDialog(string title, string message, string ok)
		{
			string cancel = "";
			return EditorUtility.DisplayDialog(title, message, ok, cancel);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int DisplayDialogComplex(string title, string message, string ok, string cancel, string alt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string OpenFilePanel(string title, string directory, string extension);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string OpenFilePanelWithFilters(string title, string directory, string[] filters);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string SaveFilePanel(string title, string directory, string defaultName, string extension);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string SaveBuildPanel(BuildTarget target, string title, string directory, string defaultName, string extension, out bool updateExistingBuild);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int NaturalCompare(string a, string b);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int NaturalCompareObjectNames(UnityEngine.Object a, UnityEngine.Object b);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string OpenFolderPanel(string title, string folder, string defaultName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string SaveFolderPanel(string title, string folder, string defaultName);

		public static string SaveFilePanelInProject(string title, string defaultName, string extension, string message)
		{
			return EditorUtility.Internal_SaveFilePanelInProject(title, defaultName, extension, message, "Assets");
		}

		public static string SaveFilePanelInProject(string title, string defaultName, string extension, string message, string path)
		{
			return EditorUtility.Internal_SaveFilePanelInProject(title, defaultName, extension, message, path);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_SaveFilePanelInProject(string title, string defaultName, string extension, string message, string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool WarnPrefab(UnityEngine.Object target, string title, string warning, string okButton);

		[Obsolete("use AssetDatabase.LoadAssetAtPath"), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object FindAsset(string path, Type type);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object InstanceIDToObject(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CompressTexture(Texture2D texture, TextureFormat format, int quality);

		public static void CompressTexture(Texture2D texture, TextureFormat format, TextureCompressionQuality quality)
		{
			EditorUtility.CompressTexture(texture, format, (int)quality);
		}

		private static void CompressTexture(Texture2D texture, TextureFormat format)
		{
			EditorUtility.CompressTexture(texture, format, TextureCompressionQuality.Normal);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string InvokeDiffTool(string leftTitle, string leftFile, string rightTitle, string rightFile, string ancestorTitle, string ancestorFile);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopySerialized(UnityEngine.Object source, UnityEngine.Object dest);

		public static void CopySerializedIfDifferent(UnityEngine.Object source, UnityEngine.Object dest)
		{
			if (source == null)
			{
				throw new ArgumentNullException("Argument 'source' is null");
			}
			if (dest == null)
			{
				throw new ArgumentNullException("Argument 'dest' is null");
			}
			EditorUtility.InternalCopySerializedIfDifferent(source, dest);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalCopySerializedIfDifferent(UnityEngine.Object source, UnityEngine.Object dest);

		[Obsolete("Use AssetDatabase.GetAssetPath")]
		public static string GetAssetPath(UnityEngine.Object asset)
		{
			return AssetDatabase.GetAssetPath(asset);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] CollectDependencies(UnityEngine.Object[] roots);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] CollectDeepHierarchy(UnityEngine.Object[] roots);

		internal static void InitInstantiatedPreviewRecursive(GameObject go)
		{
			go.hideFlags = HideFlags.HideAndDontSave;
			go.layer = Camera.PreviewCullingLayer;
			IEnumerator enumerator = go.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					EditorUtility.InitInstantiatedPreviewRecursive(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal static GameObject InstantiateForAnimatorPreview(UnityEngine.Object original)
		{
			if (original == null)
			{
				throw new ArgumentException("The prefab you want to instantiate is null.");
			}
			GameObject gameObject = EditorUtility.InstantiateRemoveAllNonAnimationComponents(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name += "AnimatorPreview";
			gameObject.tag = "Untagged";
			EditorUtility.InitInstantiatedPreviewRecursive(gameObject);
			Animator[] componentsInChildren = gameObject.GetComponentsInChildren<Animator>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Animator animator = componentsInChildren[i];
				animator.enabled = false;
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				animator.logWarnings = false;
				animator.fireEvents = false;
			}
			if (componentsInChildren.Length == 0)
			{
				Animator animator2 = gameObject.AddComponent<Animator>();
				animator2.enabled = false;
				animator2.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				animator2.logWarnings = false;
				animator2.fireEvents = false;
			}
			return gameObject;
		}

		internal static UnityEngine.Object InstantiateRemoveAllNonAnimationComponents(UnityEngine.Object original, Vector3 position, Quaternion rotation)
		{
			if (original == null)
			{
				throw new ArgumentException("The prefab you want to instantiate is null.");
			}
			return EditorUtility.Internal_InstantiateRemoveAllNonAnimationComponentsSingle(original, position, rotation);
		}

		private static UnityEngine.Object Internal_InstantiateRemoveAllNonAnimationComponentsSingle(UnityEngine.Object data, Vector3 pos, Quaternion rot)
		{
			return EditorUtility.INTERNAL_CALL_Internal_InstantiateRemoveAllNonAnimationComponentsSingle(data, ref pos, ref rot);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_Internal_InstantiateRemoveAllNonAnimationComponentsSingle(UnityEngine.Object data, ref Vector3 pos, ref Quaternion rot);

		[Obsolete("Use EditorUtility.UnloadUnusedAssetsImmediate instead"), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnloadUnusedAssets();

		[Obsolete("Use EditorUtility.UnloadUnusedAssetsImmediate instead"), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnloadUnusedAssetsIgnoreManagedReferences();

		public static void UnloadUnusedAssetsImmediate()
		{
			EditorUtility.UnloadUnusedAssetsImmediateInternal(true);
		}

		public static void UnloadUnusedAssetsImmediate(bool includeMonoReferencesAsRoots)
		{
			EditorUtility.UnloadUnusedAssetsImmediateInternal(includeMonoReferencesAsRoots);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnloadUnusedAssetsImmediateInternal(bool includeMonoReferencesAsRoots);

		[Obsolete("Use BuildPipeline.BuildAssetBundle instead")]
		public static bool BuildResourceFile(UnityEngine.Object[] selection, string pathName)
		{
			return BuildPipeline.BuildAssetBundle(null, selection, pathName, BuildAssetBundleOptions.CompleteAssets);
		}

		internal static void Internal_DisplayPopupMenu(Rect position, string menuItemPath, UnityEngine.Object context, int contextUserData)
		{
			EditorUtility.Private_DisplayPopupMenu(position, menuItemPath, context, contextUserData);
		}

		private static void Private_DisplayPopupMenu(Rect position, string menuItemPath, UnityEngine.Object context, int contextUserData)
		{
			EditorUtility.INTERNAL_CALL_Private_DisplayPopupMenu(ref position, menuItemPath, context, contextUserData);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Private_DisplayPopupMenu(ref Rect position, string menuItemPath, UnityEngine.Object context, int contextUserData);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_UpdateMenuTitleForLanguage(SystemLanguage newloc);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_UpdateAllMenus();

		public static void DisplayPopupMenu(Rect position, string menuItemPath, MenuCommand command)
		{
			if (menuItemPath == "CONTEXT" || menuItemPath == "CONTEXT/" || menuItemPath == "CONTEXT\\")
			{
				bool flag = false;
				if (command == null)
				{
					flag = true;
				}
				if (command != null && command.context == null)
				{
					flag = true;
				}
				if (flag)
				{
					Debug.LogError("DisplayPopupMenu: invalid arguments: using CONTEXT requires a valid MenuCommand object. If you want a custom context menu then try using the GenericMenu.");
					return;
				}
			}
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
			position.x = vector.x;
			position.y = vector.y;
			EditorUtility.Internal_DisplayPopupMenu(position, menuItemPath, (command != null) ? command.context : null, (command != null) ? command.userData : 0);
			EditorUtility.ResetMouseDown();
		}

		internal static void DisplayObjectContextMenu(Rect position, UnityEngine.Object context, int contextUserData)
		{
			EditorUtility.DisplayObjectContextMenu(position, new UnityEngine.Object[]
			{
				context
			}, contextUserData);
		}

		internal static void DisplayObjectContextMenu(Rect position, UnityEngine.Object[] context, int contextUserData)
		{
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
			position.x = vector.x;
			position.y = vector.y;
			EditorUtility.Internal_DisplayObjectContextMenu(position, context, contextUserData);
			EditorUtility.ResetMouseDown();
		}

		internal static void Internal_DisplayObjectContextMenu(Rect position, UnityEngine.Object[] context, int contextUserData)
		{
			EditorUtility.INTERNAL_CALL_Internal_DisplayObjectContextMenu(ref position, context, contextUserData);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DisplayObjectContextMenu(ref Rect position, UnityEngine.Object[] context, int contextUserData);

		public static void DisplayCustomMenu(Rect position, GUIContent[] options, int selected, EditorUtility.SelectMenuItemFunction callback, object userData)
		{
			EditorUtility.DisplayCustomMenu(position, options, selected, callback, userData, false);
		}

		public static void DisplayCustomMenu(Rect position, GUIContent[] options, int selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey)
		{
			int[] selected2 = new int[]
			{
				selected
			};
			string[] array = new string[options.Length];
			for (int i = 0; i < options.Length; i++)
			{
				array[i] = options[i].text;
			}
			EditorUtility.DisplayCustomMenu(position, array, selected2, callback, userData, showHotkey);
		}

		internal static void DisplayCustomMenu(Rect position, string[] options, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData)
		{
			EditorUtility.DisplayCustomMenu(position, options, selected, callback, userData, false);
		}

		internal static void DisplayCustomMenu(Rect position, string[] options, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey)
		{
			bool[] separator = new bool[options.Length];
			EditorUtility.DisplayCustomMenuWithSeparators(position, options, separator, selected, callback, userData, showHotkey);
		}

		internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] separator, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData)
		{
			EditorUtility.DisplayCustomMenuWithSeparators(position, options, separator, selected, callback, userData, false);
		}

		internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] separator, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey)
		{
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
			position.x = vector.x;
			position.y = vector.y;
			int[] array = new int[options.Length];
			int[] array2 = new int[options.Length];
			for (int i = 0; i < options.Length; i++)
			{
				array[i] = 1;
				array2[i] = 0;
			}
			EditorUtility.Internal_DisplayCustomMenu(position, options, array, array2, selected, callback, userData, showHotkey);
			EditorUtility.ResetMouseDown();
		}

		internal static void DisplayCustomMenu(Rect position, string[] options, bool[] enabled, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData)
		{
			EditorUtility.DisplayCustomMenu(position, options, enabled, selected, callback, userData, false);
		}

		internal static void DisplayCustomMenu(Rect position, string[] options, bool[] enabled, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey)
		{
			bool[] separator = new bool[options.Length];
			EditorUtility.DisplayCustomMenuWithSeparators(position, options, enabled, separator, selected, callback, userData, showHotkey);
		}

		internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] enabled, bool[] separator, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData)
		{
			EditorUtility.DisplayCustomMenuWithSeparators(position, options, enabled, separator, selected, callback, userData, false);
		}

		internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] enabled, bool[] separator, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey)
		{
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
			position.x = vector.x;
			position.y = vector.y;
			int[] array = new int[options.Length];
			int[] array2 = new int[options.Length];
			for (int i = 0; i < options.Length; i++)
			{
				array[i] = ((!enabled[i]) ? 0 : 1);
				array2[i] = ((!separator[i]) ? 0 : 1);
			}
			EditorUtility.Internal_DisplayCustomMenu(position, options, array, array2, selected, callback, userData, showHotkey);
			EditorUtility.ResetMouseDown();
		}

		private static void Internal_DisplayCustomMenu(Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey)
		{
			EditorUtility.Private_DisplayCustomMenu(screenPosition, options, enabled, separator, selected, callback, userData, showHotkey);
		}

		private static void Private_DisplayCustomMenu(Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey)
		{
			EditorUtility.INTERNAL_CALL_Private_DisplayCustomMenu(ref screenPosition, options, enabled, separator, selected, callback, userData, showHotkey);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Private_DisplayCustomMenu(ref Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, EditorUtility.SelectMenuItemFunction callback, object userData, bool showHotkey);

		internal static void ResetMouseDown()
		{
			Tools.s_ButtonDown = -1;
			GUIUtility.hotControl = 0;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FocusProjectWindow();

		public static string FormatBytes(int bytes)
		{
			return EditorUtility.FormatBytes((long)bytes);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string FormatBytes(long bytes);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisplayProgressBar(string title, string info, float progress);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DisplayCancelableProgressBar(string title, string info, float progress);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearProgressBar();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetObjectEnabled(UnityEngine.Object target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetObjectEnabled(UnityEngine.Object target, bool enabled);

		[Obsolete("Use EditorUtility.SetSelectedRenderState")]
		public static void SetSelectedWireframeHidden(Renderer renderer, bool enabled)
		{
			EditorUtility.SetSelectedRenderState(renderer, (!enabled) ? (EditorSelectedRenderState.Wireframe | EditorSelectedRenderState.Highlight) : EditorSelectedRenderState.Hidden);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSelectedRenderState(Renderer renderer, EditorSelectedRenderState renderState);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ForceReloadInspectors();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ForceRebuildInspectors();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ExtractOggFile(UnityEngine.Object obj, string path);

		public static GameObject CreateGameObjectWithHideFlags(string name, HideFlags flags, params Type[] components)
		{
			GameObject gameObject = EditorUtility.Internal_CreateGameObjectWithHideFlags(name, flags);
			gameObject.AddComponent(typeof(Transform));
			for (int i = 0; i < components.Length; i++)
			{
				Type componentType = components[i];
				gameObject.AddComponent(componentType);
			}
			return gameObject;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern GameObject Internal_CreateGameObjectWithHideFlags(string name, HideFlags flags);

		public static string[] CompileCSharp(string[] sources, string[] references, string[] defines, string outputFile)
		{
			return MonoCSharpCompiler.Compile(sources, references, defines, outputFile);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenWithDefaultApp(string fileName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool WSACreateTestCertificate(string path, string publisher, string password, bool overwrite);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsWindows10OrGreater();

		[Obsolete("Use PrefabUtility.InstantiatePrefab")]
		public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target)
		{
			return PrefabUtility.InstantiatePrefab(target);
		}

		[Obsolete("Use PrefabUtility.ReplacePrefab")]
		public static GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab, ReplacePrefabOptions options)
		{
			return PrefabUtility.ReplacePrefab(go, targetPrefab, options);
		}

		[Obsolete("Use PrefabUtility.ReplacePrefab")]
		public static GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab)
		{
			return PrefabUtility.ReplacePrefab(go, targetPrefab, ReplacePrefabOptions.Default);
		}

		[Obsolete("Use PrefabUtility.CreateEmptyPrefab")]
		public static UnityEngine.Object CreateEmptyPrefab(string path)
		{
			return PrefabUtility.CreateEmptyPrefab(path);
		}

		[Obsolete("Use PrefabUtility.CreateEmptyPrefab")]
		public static bool ReconnectToLastPrefab(GameObject go)
		{
			return PrefabUtility.ReconnectToLastPrefab(go);
		}

		[Obsolete("Use PrefabUtility.GetPrefabType")]
		public static PrefabType GetPrefabType(UnityEngine.Object target)
		{
			return PrefabUtility.GetPrefabType(target);
		}

		[Obsolete("Use PrefabUtility.GetPrefabParent")]
		public static UnityEngine.Object GetPrefabParent(UnityEngine.Object source)
		{
			return PrefabUtility.GetPrefabParent(source);
		}

		[Obsolete("Use PrefabUtility.FindPrefabRoot")]
		public static GameObject FindPrefabRoot(GameObject source)
		{
			return PrefabUtility.FindPrefabRoot(source);
		}

		[Obsolete("Use PrefabUtility.ResetToPrefabState")]
		public static bool ResetToPrefabState(UnityEngine.Object source)
		{
			return PrefabUtility.ResetToPrefabState(source);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetCameraAnimateMaterials(Camera camera, bool animate);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetInvalidFilenameChars();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetActiveNativePlatformSupportModuleName();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool EventHasDragCopyModifierPressed(Event evt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool EventHasDragMoveModifierPressed(Event evt);
	}
}
