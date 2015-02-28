using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
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
		public static EditorApplication.CallbackFunction modifierKeysChanged;
		public static EditorApplication.CallbackFunction playmodeStateChanged;
		internal static EditorApplication.CallbackFunction globalEventHandler;
		internal static EditorApplication.CallbackFunction windowsReordered;
		private static EditorApplication.CallbackFunction delayedCallback;
		private static float s_DelayedCallbackTime;
		public static extern string currentScene
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isPlaying
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern bool isPlayingOrWillChangePlaymode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isPaused
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern bool isCompiling
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isUpdating
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isRemoteConnected
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string applicationContentsPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string applicationPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal static extern UnityEngine.Object tagManager
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal static extern UnityEngine.Object renderSettings
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern double timeSinceStartup
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void NewScene();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void NewEmptyScene();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool OpenScene(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenSceneAdditive(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadLevelInPlayMode(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadLevelAdditiveInPlayMode(string path);
		public static AsyncOperation LoadLevelAsyncInPlayMode(string path)
		{
			return EditorApplication.LoadLevelAsyncInPlayMode(path, false);
		}
		public static AsyncOperation LoadLevelAdditiveAsyncInPlayMode(string path)
		{
			return EditorApplication.LoadLevelAsyncInPlayMode(path, true);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation LoadLevelAsyncInPlayMode(string path, bool isAdditive);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveScene([DefaultValue("\"\"")] string path, [DefaultValue("false")] bool saveAsCopy);
		[ExcludeFromDocs]
		public static bool SaveScene(string path)
		{
			bool saveAsCopy = false;
			return EditorApplication.SaveScene(path, saveAsCopy);
		}
		[ExcludeFromDocs]
		public static bool SaveScene()
		{
			bool saveAsCopy = false;
			string empty = string.Empty;
			return EditorApplication.SaveScene(empty, saveAsCopy);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveCurrentSceneIfUserWantsTo();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SaveCurrentSceneIfUserWantsToForce();
		public static void OpenProject(string projectPath, params string[] args)
		{
			EditorApplication.OpenProjectInternal(projectPath, args);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OpenProjectInternal(string projectPath, string[] args);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SaveAssets();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Step();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LockReloadAssemblies();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ExecuteMenuItem(string menuItemPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ExecuteMenuItemOnGameObjects(string menuItemPath, GameObject[] objects);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ExecuteMenuItemWithTemporaryContext(string menuItemPath, UnityEngine.Object[] objects);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnlockReloadAssemblies();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Exit(int returnValue);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSceneRepaintDirty();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MarkSceneDirty();
		public static void RepaintProjectWindow()
		{
			foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
			{
				current.Repaint();
			}
		}
		public static void RepaintAnimationWindow()
		{
			foreach (AnimationWindow current in AnimationWindow.GetAllAnimationWindows())
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
			EditorApplication.Internal_RepaintAllViews();
		}
		private static void Internal_RepaintAllViews()
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
		internal static void CallDelayed(EditorApplication.CallbackFunction function, float timeFromNow)
		{
			EditorApplication.delayedCallback = function;
			EditorApplication.s_DelayedCallbackTime = Time.realtimeSinceStartup + timeFromNow;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(EditorApplication.CheckCallDelayed));
		}
		private static void CheckCallDelayed()
		{
			if (Time.realtimeSinceStartup > EditorApplication.s_DelayedCallbackTime)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(EditorApplication.CheckCallDelayed));
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
		private static void Internal_CallGlobalEventHandler()
		{
			if (EditorApplication.globalEventHandler != null)
			{
				EditorApplication.globalEventHandler();
			}
			WindowLayout.MaximizeKeyHandler();
			Event.current = null;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Beep();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReportUNetWeaver(string filename, string msg, bool isError);
	}
}
