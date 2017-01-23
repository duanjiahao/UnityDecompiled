using System;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class EditorApplication
	{
		public delegate void ProjectWindowItemCallback(string guid, Rect selectionRect);

		public delegate void HierarchyWindowItemCallback(int instanceID, Rect selectionRect);

		public delegate void CallbackFunction();

		public static EditorApplication.ProjectWindowItemCallback projectWindowItemOnGUI;

		public static EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemOnGUI;

		public static EditorApplication.CallbackFunction update;

		public static EditorApplication.CallbackFunction delayCall;

		public static EditorApplication.CallbackFunction hierarchyWindowChanged;

		public static EditorApplication.CallbackFunction projectWindowChanged;

		public static EditorApplication.CallbackFunction searchChanged;

		internal static EditorApplication.CallbackFunction assetLabelsChanged;

		internal static EditorApplication.CallbackFunction assetBundleNameChanged;

		public static EditorApplication.CallbackFunction modifierKeysChanged;

		public static EditorApplication.CallbackFunction playmodeStateChanged;

		internal static EditorApplication.CallbackFunction globalEventHandler;

		internal static EditorApplication.CallbackFunction windowsReordered;

		private static EditorApplication.CallbackFunction delayedCallback;

		private static float s_DelayedCallbackTime = 0f;

		internal static UnityAction projectWasLoaded;

		internal static UnityAction editorApplicationQuit;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache1;

		public static extern bool isPlaying
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isPlayingOrWillChangePlaymode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isPaused
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isCompiling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isUpdating
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isRemoteConnected
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public static extern string applicationContentsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string applicationPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern string userJavascriptPackagesPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern UnityEngine.Object tagManager
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern UnityEngine.Object renderSettings
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern double timeSinceStartup
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static bool supportsHiDPI
		{
			get
			{
				return Application.platform == RuntimePlatform.OSXEditor;
			}
		}

		[Obsolete("Use Scene.isDirty instead. Use EditorSceneManager.GetScene API to get each open scene")]
		public static bool isSceneDirty
		{
			get
			{
				return SceneManager.GetActiveScene().isDirty;
			}
		}

		[Obsolete("Use EditorSceneManager to see which scenes are currently loaded")]
		public static string currentScene
		{
			get
			{
				Scene activeScene = SceneManager.GetActiveScene();
				string result;
				if (activeScene.IsValid())
				{
					result = activeScene.path;
				}
				else
				{
					result = "";
				}
				return result;
			}
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadLevelInPlayMode(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadLevelAdditiveInPlayMode(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation LoadLevelAsyncInPlayMode(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation LoadLevelAdditiveAsyncInPlayMode(string path);

		public static void OpenProject(string projectPath, params string[] args)
		{
			EditorApplication.OpenProjectInternal(projectPath, args);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OpenProjectInternal(string projectPath, string[] args);

		[Obsolete("Use AssetDatabase.SaveAssets instead (UnityUpgradable) -> AssetDatabase.SaveAssets()", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SaveAssets();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Step();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LockReloadAssemblies();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ExecuteMenuItem(string menuItemPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ExecuteMenuItemOnGameObjects(string menuItemPath, GameObject[] objects);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ExecuteMenuItemWithTemporaryContext(string menuItemPath, UnityEngine.Object[] objects);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnlockReloadAssemblies();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Exit(int returnValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSceneRepaintDirty();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateSceneIfNeeded();

		public static void RepaintProjectWindow()
		{
			foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
			{
				current.Repaint();
			}
		}

		public static void RepaintAnimationWindow()
		{
			foreach (AnimEditor current in AnimEditor.GetAllAnimationWindows())
			{
				current.Repaint();
			}
		}

		public static void RepaintHierarchyWindow()
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow));
			for (int i = 0; i < array.Length; i++)
			{
				SceneHierarchyWindow sceneHierarchyWindow = (SceneHierarchyWindow)array[i];
				sceneHierarchyWindow.Repaint();
			}
		}

		public static void DirtyHierarchyWindowSorting()
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow));
			for (int i = 0; i < array.Length; i++)
			{
				SceneHierarchyWindow sceneHierarchyWindow = (SceneHierarchyWindow)array[i];
				sceneHierarchyWindow.DirtySortingMethods();
			}
		}

		private static void Internal_CallUpdateFunctions()
		{
			if (EditorApplication.update != null)
			{
				EditorApplication.update();
			}
		}

		private static void Internal_CallDelayFunctions()
		{
			EditorApplication.CallbackFunction callbackFunction = EditorApplication.delayCall;
			EditorApplication.delayCall = null;
			if (callbackFunction != null)
			{
				callbackFunction();
			}
		}

		private static void Internal_SwitchSkin()
		{
			EditorGUIUtility.Internal_SwitchSkin();
		}

		internal static void RequestRepaintAllViews()
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GUIView));
			for (int i = 0; i < array.Length; i++)
			{
				GUIView gUIView = (GUIView)array[i];
				gUIView.Repaint();
			}
		}

		private static void Internal_CallHierarchyWindowHasChanged()
		{
			if (EditorApplication.hierarchyWindowChanged != null)
			{
				EditorApplication.hierarchyWindowChanged();
			}
		}

		private static void Internal_CallProjectWindowHasChanged()
		{
			if (EditorApplication.projectWindowChanged != null)
			{
				EditorApplication.projectWindowChanged();
			}
		}

		internal static void Internal_CallSearchHasChanged()
		{
			if (EditorApplication.searchChanged != null)
			{
				EditorApplication.searchChanged();
			}
		}

		internal static void Internal_CallAssetLabelsHaveChanged()
		{
			if (EditorApplication.assetLabelsChanged != null)
			{
				EditorApplication.assetLabelsChanged();
			}
		}

		internal static void Internal_CallAssetBundleNameChanged()
		{
			if (EditorApplication.assetBundleNameChanged != null)
			{
				EditorApplication.assetBundleNameChanged();
			}
		}

		internal static void CallDelayed(EditorApplication.CallbackFunction function, float timeFromNow)
		{
			EditorApplication.delayedCallback = function;
			EditorApplication.s_DelayedCallbackTime = Time.realtimeSinceStartup + timeFromNow;
			Delegate arg_35_0 = EditorApplication.update;
			if (EditorApplication.<>f__mg$cache0 == null)
			{
				EditorApplication.<>f__mg$cache0 = new EditorApplication.CallbackFunction(EditorApplication.CheckCallDelayed);
			}
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(arg_35_0, EditorApplication.<>f__mg$cache0);
		}

		private static void CheckCallDelayed()
		{
			if (Time.realtimeSinceStartup > EditorApplication.s_DelayedCallbackTime)
			{
				Delegate arg_33_0 = EditorApplication.update;
				if (EditorApplication.<>f__mg$cache1 == null)
				{
					EditorApplication.<>f__mg$cache1 = new EditorApplication.CallbackFunction(EditorApplication.CheckCallDelayed);
				}
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(arg_33_0, EditorApplication.<>f__mg$cache1);
				EditorApplication.delayedCallback();
			}
		}

		private static void Internal_PlaymodeStateChanged()
		{
			if (EditorApplication.playmodeStateChanged != null)
			{
				EditorApplication.playmodeStateChanged();
			}
		}

		private static void Internal_CallKeyboardModifiersChanged()
		{
			if (EditorApplication.modifierKeysChanged != null)
			{
				EditorApplication.modifierKeysChanged();
			}
		}

		private static void Internal_CallWindowsReordered()
		{
			if (EditorApplication.windowsReordered != null)
			{
				EditorApplication.windowsReordered();
			}
		}

		[RequiredByNativeCode]
		private static void Internal_CallGlobalEventHandler()
		{
			if (EditorApplication.globalEventHandler != null)
			{
				EditorApplication.globalEventHandler();
			}
			WindowLayout.MaximizeKeyHandler();
			Event.current = null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Beep();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReportUNetWeaver(string filename, string msg, bool isError);

		private static void Internal_ProjectWasLoaded()
		{
			if (EditorApplication.projectWasLoaded != null)
			{
				EditorApplication.projectWasLoaded();
			}
		}

		private static void Internal_EditorApplicationQuit()
		{
			if (EditorApplication.editorApplicationQuit != null)
			{
				EditorApplication.editorApplicationQuit();
			}
		}

		[Obsolete("Use EditorSceneManager.NewScene (NewSceneSetup.DefaultGameObjects)")]
		public static void NewScene()
		{
			EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
		}

		[Obsolete("Use EditorSceneManager.NewScene (NewSceneSetup.EmptyScene)")]
		public static void NewEmptyScene()
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
		}

		[Obsolete("Use EditorSceneManager.OpenScene")]
		public static bool OpenScene(string path)
		{
			if (!EditorApplication.isPlaying)
			{
				return EditorSceneManager.OpenScene(path).IsValid();
			}
			throw new InvalidOperationException("EditorApplication.OpenScene() cannot be called when in the Unity Editor is in play mode.");
		}

		[Obsolete("Use EditorSceneManager.OpenScene")]
		public static void OpenSceneAdditive(string path)
		{
			if (Application.isPlaying)
			{
				Debug.LogWarning("Exiting playmode.\nOpenSceneAdditive was called at a point where there was no active scene.\nThis usually means it was called in a PostprocessScene function during scene loading or it was called during playmode.\nThis is no longer allowed. Use SceneManager.LoadScene to load scenes at runtime or in playmode.");
			}
			Scene sourceScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
			Scene activeScene = SceneManager.GetActiveScene();
			SceneManager.MergeScenes(sourceScene, activeScene);
		}

		[Obsolete("Use EditorSceneManager.SaveScene")]
		public static bool SaveScene()
		{
			return EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
		}

		[Obsolete("Use EditorSceneManager.SaveScene")]
		public static bool SaveScene(string path)
		{
			return EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path, false);
		}

		[Obsolete("Use EditorSceneManager.SaveScene")]
		public static bool SaveScene(string path, bool saveAsCopy)
		{
			return EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path, saveAsCopy);
		}

		[Obsolete("Use EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo")]
		public static bool SaveCurrentSceneIfUserWantsTo()
		{
			return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		}

		[Obsolete("This function is internal and no longer supported")]
		internal static bool SaveCurrentSceneIfUserWantsToForce()
		{
			return false;
		}

		[Obsolete("Use EditorSceneManager.MarkSceneDirty or EditorSceneManager.MarkAllScenesDirty")]
		public static void MarkSceneDirty()
		{
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
	}
}
