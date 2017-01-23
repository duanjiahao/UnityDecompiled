using System;
using System.Runtime.CompilerServices;
using System.Threading;
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
			add
			{
				UnityAction<Scene, LoadSceneMode> unityAction = SceneManager.sceneLoaded;
				UnityAction<Scene, LoadSceneMode> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<Scene, LoadSceneMode>>(ref SceneManager.sceneLoaded, (UnityAction<Scene, LoadSceneMode>)Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction<Scene, LoadSceneMode> unityAction = SceneManager.sceneLoaded;
				UnityAction<Scene, LoadSceneMode> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<Scene, LoadSceneMode>>(ref SceneManager.sceneLoaded, (UnityAction<Scene, LoadSceneMode>)Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public static event UnityAction<Scene> sceneUnloaded
		{
			add
			{
				UnityAction<Scene> unityAction = SceneManager.sceneUnloaded;
				UnityAction<Scene> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<Scene>>(ref SceneManager.sceneUnloaded, (UnityAction<Scene>)Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction<Scene> unityAction = SceneManager.sceneUnloaded;
				UnityAction<Scene> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<Scene>>(ref SceneManager.sceneUnloaded, (UnityAction<Scene>)Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public static event UnityAction<Scene, Scene> activeSceneChanged
		{
			add
			{
				UnityAction<Scene, Scene> unityAction = SceneManager.activeSceneChanged;
				UnityAction<Scene, Scene> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<Scene, Scene>>(ref SceneManager.activeSceneChanged, (UnityAction<Scene, Scene>)Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction<Scene, Scene> unityAction = SceneManager.activeSceneChanged;
				UnityAction<Scene, Scene> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<Scene, Scene>>(ref SceneManager.activeSceneChanged, (UnityAction<Scene, Scene>)Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public static extern int sceneCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int sceneCountInBuildSettings
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Scene GetActiveScene()
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetActiveScene(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetActiveScene(out Scene value);

		public static bool SetActiveScene(Scene scene)
		{
			return SceneManager.INTERNAL_CALL_SetActiveScene(ref scene);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SetActiveScene(ref Scene scene);

		public static Scene GetSceneByPath(string scenePath)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneByPath(scenePath, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByPath(string scenePath, out Scene value);

		public static Scene GetSceneByName(string name)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneByName(name, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByName(string name, out Scene value);

		public static Scene GetSceneByBuildIndex(int buildIndex)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneByBuildIndex(buildIndex, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByBuildIndex(int buildIndex, out Scene value);

		public static Scene GetSceneAt(int index)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneAt(index, out result);
			return result;
		}

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);

		public static Scene CreateScene(string sceneName)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_CreateScene(sceneName, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateScene(string sceneName, out Scene value);

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(Scene scene)
		{
			bool result;
			SceneManager.UnloadSceneNameIndexInternal("", scene.buildIndex, true, out result);
			return result;
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(int sceneBuildIndex)
		{
			bool result;
			SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, true, out result);
			return result;
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(string sceneName)
		{
			bool result;
			SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, true, out result);
			return result;
		}

		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, false, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(string sceneName)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, false, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(Scene scene)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal("", scene.buildIndex, false, out flag);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, out bool outSuccess);

		public static void MergeScenes(Scene sourceScene, Scene destinationScene)
		{
			SceneManager.INTERNAL_CALL_MergeScenes(ref sourceScene, ref destinationScene);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MergeScenes(ref Scene sourceScene, ref Scene destinationScene);

		public static void MoveGameObjectToScene(GameObject go, Scene scene)
		{
			SceneManager.INTERNAL_CALL_MoveGameObjectToScene(go, ref scene);
		}

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
