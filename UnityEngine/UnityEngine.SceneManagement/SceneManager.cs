using System;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement
{
	[RequiredByNativeCode]
	public class SceneManager
	{
		public static event UnityAction<Scene, LoadSceneMode> sceneLoaded
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				SceneManager.sceneLoaded = (UnityAction<Scene, LoadSceneMode>)Delegate.Combine(SceneManager.sceneLoaded, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				SceneManager.sceneLoaded = (UnityAction<Scene, LoadSceneMode>)Delegate.Remove(SceneManager.sceneLoaded, value);
			}
		}

		public static event UnityAction<Scene> sceneUnloaded
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				SceneManager.sceneUnloaded = (UnityAction<Scene>)Delegate.Combine(SceneManager.sceneUnloaded, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				SceneManager.sceneUnloaded = (UnityAction<Scene>)Delegate.Remove(SceneManager.sceneUnloaded, value);
			}
		}

		public static event UnityAction<Scene, Scene> activeSceneChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				SceneManager.activeSceneChanged = (UnityAction<Scene, Scene>)Delegate.Combine(SceneManager.activeSceneChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				SceneManager.activeSceneChanged = (UnityAction<Scene, Scene>)Delegate.Remove(SceneManager.activeSceneChanged, value);
			}
		}

		public static extern int sceneCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int sceneCountInBuildSettings
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Scene GetActiveScene()
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetActiveScene(out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetActiveScene(out Scene value);

		public static bool SetActiveScene(Scene scene)
		{
			return SceneManager.INTERNAL_CALL_SetActiveScene(ref scene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SetActiveScene(ref Scene scene);

		public static Scene GetSceneByPath(string scenePath)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneByPath(scenePath, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByPath(string scenePath, out Scene value);

		public static Scene GetSceneByName(string name)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneByName(name, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByName(string name, out Scene value);

		public static Scene GetSceneAt(int index)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneAt(index, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneAt(int index, out Scene value);

		[Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
		public static Scene[] GetAllScenes()
		{
			Scene[] array = new Scene[SceneManager.sceneCount];
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				array[i] = SceneManager.GetSceneAt(i);
			}
			return array;
		}

		[ExcludeFromDocs]
		public static void LoadScene(string sceneName)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			SceneManager.LoadScene(sceneName, mode);
		}

		public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, true);
		}

		[ExcludeFromDocs]
		public static void LoadScene(int sceneBuildIndex)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			SceneManager.LoadScene(sceneBuildIndex, mode);
		}

		public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, true);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(string sceneName)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			return SceneManager.LoadSceneAsync(sceneName, mode);
		}

		public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, false);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			return SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
		}

		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, false);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);

		public static Scene CreateScene(string sceneName)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_CreateScene(sceneName, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateScene(string sceneName, out Scene value);

		public static bool UnloadScene(int sceneBuildIndex)
		{
			return SceneManager.UnloadSceneNameIndexInternal(string.Empty, sceneBuildIndex);
		}

		public static bool UnloadScene(string sceneName)
		{
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1);
		}

		public static bool UnloadScene(Scene scene)
		{
			return SceneManager.INTERNAL_CALL_UnloadScene(ref scene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_UnloadScene(ref Scene scene);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex);

		public static void MergeScenes(Scene sourceScene, Scene destinationScene)
		{
			SceneManager.INTERNAL_CALL_MergeScenes(ref sourceScene, ref destinationScene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MergeScenes(ref Scene sourceScene, ref Scene destinationScene);

		public static void MoveGameObjectToScene(GameObject go, Scene scene)
		{
			SceneManager.INTERNAL_CALL_MoveGameObjectToScene(go, ref scene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveGameObjectToScene(GameObject go, ref Scene scene);

		[RequiredByNativeCode]
		private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (SceneManager.sceneLoaded != null)
			{
				SceneManager.sceneLoaded(scene, mode);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneUnloaded(Scene scene)
		{
			if (SceneManager.sceneUnloaded != null)
			{
				SceneManager.sceneUnloaded(scene);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
		{
			if (SceneManager.activeSceneChanged != null)
			{
				SceneManager.activeSceneChanged(previousActiveScene, newActiveScene);
			}
		}
	}
}
