using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEditor.SceneManagement
{
	public sealed class EditorSceneManager : SceneManager
	{
		public delegate void NewSceneCreatedCallback(Scene scene, NewSceneSetup setup, NewSceneMode mode);

		public delegate void SceneOpeningCallback(string path, OpenSceneMode mode);

		public delegate void SceneOpenedCallback(Scene scene, OpenSceneMode mode);

		public delegate void SceneClosingCallback(Scene scene, bool removingScene);

		public delegate void SceneClosedCallback(Scene scene);

		public delegate void SceneSavingCallback(Scene scene, string path);

		public delegate void SceneSavedCallback(Scene scene);

		internal static UnityAction<Scene, NewSceneMode> sceneWasCreated;

		internal static UnityAction<Scene, OpenSceneMode> sceneWasOpened;

		public static event EditorSceneManager.NewSceneCreatedCallback newSceneCreated
		{
			add
			{
				EditorSceneManager.NewSceneCreatedCallback newSceneCreatedCallback = EditorSceneManager.newSceneCreated;
				EditorSceneManager.NewSceneCreatedCallback newSceneCreatedCallback2;
				do
				{
					newSceneCreatedCallback2 = newSceneCreatedCallback;
					newSceneCreatedCallback = Interlocked.CompareExchange<EditorSceneManager.NewSceneCreatedCallback>(ref EditorSceneManager.newSceneCreated, (EditorSceneManager.NewSceneCreatedCallback)Delegate.Combine(newSceneCreatedCallback2, value), newSceneCreatedCallback);
				}
				while (newSceneCreatedCallback != newSceneCreatedCallback2);
			}
			remove
			{
				EditorSceneManager.NewSceneCreatedCallback newSceneCreatedCallback = EditorSceneManager.newSceneCreated;
				EditorSceneManager.NewSceneCreatedCallback newSceneCreatedCallback2;
				do
				{
					newSceneCreatedCallback2 = newSceneCreatedCallback;
					newSceneCreatedCallback = Interlocked.CompareExchange<EditorSceneManager.NewSceneCreatedCallback>(ref EditorSceneManager.newSceneCreated, (EditorSceneManager.NewSceneCreatedCallback)Delegate.Remove(newSceneCreatedCallback2, value), newSceneCreatedCallback);
				}
				while (newSceneCreatedCallback != newSceneCreatedCallback2);
			}
		}

		public static event EditorSceneManager.SceneOpeningCallback sceneOpening
		{
			add
			{
				EditorSceneManager.SceneOpeningCallback sceneOpeningCallback = EditorSceneManager.sceneOpening;
				EditorSceneManager.SceneOpeningCallback sceneOpeningCallback2;
				do
				{
					sceneOpeningCallback2 = sceneOpeningCallback;
					sceneOpeningCallback = Interlocked.CompareExchange<EditorSceneManager.SceneOpeningCallback>(ref EditorSceneManager.sceneOpening, (EditorSceneManager.SceneOpeningCallback)Delegate.Combine(sceneOpeningCallback2, value), sceneOpeningCallback);
				}
				while (sceneOpeningCallback != sceneOpeningCallback2);
			}
			remove
			{
				EditorSceneManager.SceneOpeningCallback sceneOpeningCallback = EditorSceneManager.sceneOpening;
				EditorSceneManager.SceneOpeningCallback sceneOpeningCallback2;
				do
				{
					sceneOpeningCallback2 = sceneOpeningCallback;
					sceneOpeningCallback = Interlocked.CompareExchange<EditorSceneManager.SceneOpeningCallback>(ref EditorSceneManager.sceneOpening, (EditorSceneManager.SceneOpeningCallback)Delegate.Remove(sceneOpeningCallback2, value), sceneOpeningCallback);
				}
				while (sceneOpeningCallback != sceneOpeningCallback2);
			}
		}

		public static event EditorSceneManager.SceneOpenedCallback sceneOpened
		{
			add
			{
				EditorSceneManager.SceneOpenedCallback sceneOpenedCallback = EditorSceneManager.sceneOpened;
				EditorSceneManager.SceneOpenedCallback sceneOpenedCallback2;
				do
				{
					sceneOpenedCallback2 = sceneOpenedCallback;
					sceneOpenedCallback = Interlocked.CompareExchange<EditorSceneManager.SceneOpenedCallback>(ref EditorSceneManager.sceneOpened, (EditorSceneManager.SceneOpenedCallback)Delegate.Combine(sceneOpenedCallback2, value), sceneOpenedCallback);
				}
				while (sceneOpenedCallback != sceneOpenedCallback2);
			}
			remove
			{
				EditorSceneManager.SceneOpenedCallback sceneOpenedCallback = EditorSceneManager.sceneOpened;
				EditorSceneManager.SceneOpenedCallback sceneOpenedCallback2;
				do
				{
					sceneOpenedCallback2 = sceneOpenedCallback;
					sceneOpenedCallback = Interlocked.CompareExchange<EditorSceneManager.SceneOpenedCallback>(ref EditorSceneManager.sceneOpened, (EditorSceneManager.SceneOpenedCallback)Delegate.Remove(sceneOpenedCallback2, value), sceneOpenedCallback);
				}
				while (sceneOpenedCallback != sceneOpenedCallback2);
			}
		}

		public static event EditorSceneManager.SceneClosingCallback sceneClosing
		{
			add
			{
				EditorSceneManager.SceneClosingCallback sceneClosingCallback = EditorSceneManager.sceneClosing;
				EditorSceneManager.SceneClosingCallback sceneClosingCallback2;
				do
				{
					sceneClosingCallback2 = sceneClosingCallback;
					sceneClosingCallback = Interlocked.CompareExchange<EditorSceneManager.SceneClosingCallback>(ref EditorSceneManager.sceneClosing, (EditorSceneManager.SceneClosingCallback)Delegate.Combine(sceneClosingCallback2, value), sceneClosingCallback);
				}
				while (sceneClosingCallback != sceneClosingCallback2);
			}
			remove
			{
				EditorSceneManager.SceneClosingCallback sceneClosingCallback = EditorSceneManager.sceneClosing;
				EditorSceneManager.SceneClosingCallback sceneClosingCallback2;
				do
				{
					sceneClosingCallback2 = sceneClosingCallback;
					sceneClosingCallback = Interlocked.CompareExchange<EditorSceneManager.SceneClosingCallback>(ref EditorSceneManager.sceneClosing, (EditorSceneManager.SceneClosingCallback)Delegate.Remove(sceneClosingCallback2, value), sceneClosingCallback);
				}
				while (sceneClosingCallback != sceneClosingCallback2);
			}
		}

		public static event EditorSceneManager.SceneClosedCallback sceneClosed
		{
			add
			{
				EditorSceneManager.SceneClosedCallback sceneClosedCallback = EditorSceneManager.sceneClosed;
				EditorSceneManager.SceneClosedCallback sceneClosedCallback2;
				do
				{
					sceneClosedCallback2 = sceneClosedCallback;
					sceneClosedCallback = Interlocked.CompareExchange<EditorSceneManager.SceneClosedCallback>(ref EditorSceneManager.sceneClosed, (EditorSceneManager.SceneClosedCallback)Delegate.Combine(sceneClosedCallback2, value), sceneClosedCallback);
				}
				while (sceneClosedCallback != sceneClosedCallback2);
			}
			remove
			{
				EditorSceneManager.SceneClosedCallback sceneClosedCallback = EditorSceneManager.sceneClosed;
				EditorSceneManager.SceneClosedCallback sceneClosedCallback2;
				do
				{
					sceneClosedCallback2 = sceneClosedCallback;
					sceneClosedCallback = Interlocked.CompareExchange<EditorSceneManager.SceneClosedCallback>(ref EditorSceneManager.sceneClosed, (EditorSceneManager.SceneClosedCallback)Delegate.Remove(sceneClosedCallback2, value), sceneClosedCallback);
				}
				while (sceneClosedCallback != sceneClosedCallback2);
			}
		}

		public static event EditorSceneManager.SceneSavingCallback sceneSaving
		{
			add
			{
				EditorSceneManager.SceneSavingCallback sceneSavingCallback = EditorSceneManager.sceneSaving;
				EditorSceneManager.SceneSavingCallback sceneSavingCallback2;
				do
				{
					sceneSavingCallback2 = sceneSavingCallback;
					sceneSavingCallback = Interlocked.CompareExchange<EditorSceneManager.SceneSavingCallback>(ref EditorSceneManager.sceneSaving, (EditorSceneManager.SceneSavingCallback)Delegate.Combine(sceneSavingCallback2, value), sceneSavingCallback);
				}
				while (sceneSavingCallback != sceneSavingCallback2);
			}
			remove
			{
				EditorSceneManager.SceneSavingCallback sceneSavingCallback = EditorSceneManager.sceneSaving;
				EditorSceneManager.SceneSavingCallback sceneSavingCallback2;
				do
				{
					sceneSavingCallback2 = sceneSavingCallback;
					sceneSavingCallback = Interlocked.CompareExchange<EditorSceneManager.SceneSavingCallback>(ref EditorSceneManager.sceneSaving, (EditorSceneManager.SceneSavingCallback)Delegate.Remove(sceneSavingCallback2, value), sceneSavingCallback);
				}
				while (sceneSavingCallback != sceneSavingCallback2);
			}
		}

		public static event EditorSceneManager.SceneSavedCallback sceneSaved
		{
			add
			{
				EditorSceneManager.SceneSavedCallback sceneSavedCallback = EditorSceneManager.sceneSaved;
				EditorSceneManager.SceneSavedCallback sceneSavedCallback2;
				do
				{
					sceneSavedCallback2 = sceneSavedCallback;
					sceneSavedCallback = Interlocked.CompareExchange<EditorSceneManager.SceneSavedCallback>(ref EditorSceneManager.sceneSaved, (EditorSceneManager.SceneSavedCallback)Delegate.Combine(sceneSavedCallback2, value), sceneSavedCallback);
				}
				while (sceneSavedCallback != sceneSavedCallback2);
			}
			remove
			{
				EditorSceneManager.SceneSavedCallback sceneSavedCallback = EditorSceneManager.sceneSaved;
				EditorSceneManager.SceneSavedCallback sceneSavedCallback2;
				do
				{
					sceneSavedCallback2 = sceneSavedCallback;
					sceneSavedCallback = Interlocked.CompareExchange<EditorSceneManager.SceneSavedCallback>(ref EditorSceneManager.sceneSaved, (EditorSceneManager.SceneSavedCallback)Delegate.Remove(sceneSavedCallback2, value), sceneSavedCallback);
				}
				while (sceneSavedCallback != sceneSavedCallback2);
			}
		}

		public static extern int loadedSceneCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool preventCrossSceneReferences
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Scene OpenScene(string scenePath, [DefaultValue("OpenSceneMode.Single")] OpenSceneMode mode)
		{
			Scene result;
			EditorSceneManager.INTERNAL_CALL_OpenScene(scenePath, mode, out result);
			return result;
		}

		[ExcludeFromDocs]
		public static Scene OpenScene(string scenePath)
		{
			OpenSceneMode mode = OpenSceneMode.Single;
			Scene result;
			EditorSceneManager.INTERNAL_CALL_OpenScene(scenePath, mode, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_OpenScene(string scenePath, OpenSceneMode mode, out Scene value);

		public static Scene NewScene(NewSceneSetup setup, [DefaultValue("NewSceneMode.Single")] NewSceneMode mode)
		{
			Scene result;
			EditorSceneManager.INTERNAL_CALL_NewScene(setup, mode, out result);
			return result;
		}

		[ExcludeFromDocs]
		public static Scene NewScene(NewSceneSetup setup)
		{
			NewSceneMode mode = NewSceneMode.Single;
			Scene result;
			EditorSceneManager.INTERNAL_CALL_NewScene(setup, mode, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_NewScene(NewSceneSetup setup, NewSceneMode mode, out Scene value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CreateSceneAsset(string scenePath, bool createDefaultGameObjects);

		public static bool CloseScene(Scene scene, bool removeScene)
		{
			return EditorSceneManager.INTERNAL_CALL_CloseScene(ref scene, removeScene);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CloseScene(ref Scene scene, bool removeScene);

		internal static bool ReloadScene(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_ReloadScene(ref scene);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ReloadScene(ref Scene scene);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTargetSceneForNewGameObjects(int sceneHandle);

		internal static Scene GetTargetSceneForNewGameObjects()
		{
			Scene result;
			EditorSceneManager.INTERNAL_CALL_GetTargetSceneForNewGameObjects(out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTargetSceneForNewGameObjects(out Scene value);

		internal static Scene GetSceneByHandle(int handle)
		{
			Scene result;
			EditorSceneManager.INTERNAL_CALL_GetSceneByHandle(handle, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByHandle(int handle, out Scene value);

		public static void MoveSceneBefore(Scene src, Scene dst)
		{
			EditorSceneManager.INTERNAL_CALL_MoveSceneBefore(ref src, ref dst);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveSceneBefore(ref Scene src, ref Scene dst);

		public static void MoveSceneAfter(Scene src, Scene dst)
		{
			EditorSceneManager.INTERNAL_CALL_MoveSceneAfter(ref src, ref dst);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveSceneAfter(ref Scene src, ref Scene dst);

		internal static bool SaveSceneAs(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_SaveSceneAs(ref scene);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SaveSceneAs(ref Scene scene);

		public static bool SaveScene(Scene scene, [DefaultValue("\"\"")] string dstScenePath, [DefaultValue("false")] bool saveAsCopy)
		{
			return EditorSceneManager.INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
		}

		[ExcludeFromDocs]
		public static bool SaveScene(Scene scene, string dstScenePath)
		{
			bool saveAsCopy = false;
			return EditorSceneManager.INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
		}

		[ExcludeFromDocs]
		public static bool SaveScene(Scene scene)
		{
			bool saveAsCopy = false;
			string dstScenePath = "";
			return EditorSceneManager.INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SaveScene(ref Scene scene, string dstScenePath, bool saveAsCopy);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveOpenScenes();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveScenes(Scene[] scenes);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveCurrentModifiedScenesIfUserWantsTo();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveModifiedScenesIfUserWantsTo(Scene[] scenes);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EnsureUntitledSceneHasBeenSaved(string dialogContent);

		public static bool MarkSceneDirty(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_MarkSceneDirty(ref scene);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_MarkSceneDirty(ref Scene scene);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MarkAllScenesDirty();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern SceneSetup[] GetSceneManagerSetup();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RestoreSceneManagerSetup(SceneSetup[] value);

		public static bool DetectCrossSceneReferences(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_DetectCrossSceneReferences(ref scene);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_DetectCrossSceneReferences(ref Scene scene);

		private static void Internal_NewSceneWasCreated(Scene scene, NewSceneMode mode)
		{
			if (EditorSceneManager.sceneWasCreated != null)
			{
				EditorSceneManager.sceneWasCreated(scene, mode);
			}
		}

		private static void Internal_SceneWasOpened(Scene scene, OpenSceneMode mode)
		{
			if (EditorSceneManager.sceneWasOpened != null)
			{
				EditorSceneManager.sceneWasOpened(scene, mode);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_NewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
		{
			if (EditorSceneManager.newSceneCreated != null)
			{
				EditorSceneManager.newSceneCreated(scene, setup, mode);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneOpening(string path, OpenSceneMode mode)
		{
			if (EditorSceneManager.sceneOpening != null)
			{
				EditorSceneManager.sceneOpening(path, mode);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneOpened(Scene scene, OpenSceneMode mode)
		{
			if (EditorSceneManager.sceneOpened != null)
			{
				EditorSceneManager.sceneOpened(scene, mode);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneClosing(Scene scene, bool removingScene)
		{
			if (EditorSceneManager.sceneClosing != null)
			{
				EditorSceneManager.sceneClosing(scene, removingScene);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneClosed(Scene scene)
		{
			if (EditorSceneManager.sceneClosed != null)
			{
				EditorSceneManager.sceneClosed(scene);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneSaving(Scene scene, string path)
		{
			if (EditorSceneManager.sceneSaving != null)
			{
				EditorSceneManager.sceneSaving(scene, path);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneSaved(Scene scene)
		{
			if (EditorSceneManager.sceneSaved != null)
			{
				EditorSceneManager.sceneSaved(scene);
			}
		}
	}
}
