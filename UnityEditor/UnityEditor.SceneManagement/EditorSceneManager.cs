using System;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;

namespace UnityEditor.SceneManagement
{
	public sealed class EditorSceneManager : SceneManager
	{
		internal static UnityAction<Scene, NewSceneMode> sceneWasCreated;

		internal static UnityAction<Scene, OpenSceneMode> sceneWasOpened;

		public static extern int loadedSceneCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool preventCrossSceneReferences
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_NewScene(NewSceneSetup setup, NewSceneMode mode, out Scene value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CreateSceneAsset(string scenePath, bool createDefaultGameObjects);

		public static bool CloseScene(Scene scene, bool removeScene)
		{
			return EditorSceneManager.INTERNAL_CALL_CloseScene(ref scene, removeScene);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CloseScene(ref Scene scene, bool removeScene);

		internal static bool ReloadScene(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_ReloadScene(ref scene);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ReloadScene(ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTargetSceneForNewGameObjects(int sceneHandle);

		internal static Scene GetTargetSceneForNewGameObjects()
		{
			Scene result;
			EditorSceneManager.INTERNAL_CALL_GetTargetSceneForNewGameObjects(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTargetSceneForNewGameObjects(out Scene value);

		internal static Scene GetSceneByHandle(int handle)
		{
			Scene result;
			EditorSceneManager.INTERNAL_CALL_GetSceneByHandle(handle, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByHandle(int handle, out Scene value);

		public static void MoveSceneBefore(Scene src, Scene dst)
		{
			EditorSceneManager.INTERNAL_CALL_MoveSceneBefore(ref src, ref dst);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveSceneBefore(ref Scene src, ref Scene dst);

		public static void MoveSceneAfter(Scene src, Scene dst)
		{
			EditorSceneManager.INTERNAL_CALL_MoveSceneAfter(ref src, ref dst);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveSceneAfter(ref Scene src, ref Scene dst);

		internal static bool SaveSceneAs(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_SaveSceneAs(ref scene);
		}

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SaveScene(ref Scene scene, string dstScenePath, bool saveAsCopy);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveOpenScenes();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveScenes(Scene[] scenes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveCurrentModifiedScenesIfUserWantsTo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SaveModifiedScenesIfUserWantsTo(Scene[] scenes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool EnsureUntitledSceneHasBeenSaved(string operation);

		public static bool MarkSceneDirty(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_MarkSceneDirty(ref scene);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_MarkSceneDirty(ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MarkAllScenesDirty();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern SceneSetup[] GetSceneManagerSetup();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RestoreSceneManagerSetup(SceneSetup[] value);

		public static bool DetectCrossSceneReferences(Scene scene)
		{
			return EditorSceneManager.INTERNAL_CALL_DetectCrossSceneReferences(ref scene);
		}

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
	}
}
