using System;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	public sealed class PrefabUtility
	{
		public delegate void PrefabInstanceUpdated(GameObject instance);

		public static PrefabUtility.PrefabInstanceUpdated prefabInstanceUpdated;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetPrefabParent(UnityEngine.Object source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetPrefabObject(UnityEngine.Object targetObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern PropertyModification[] GetPropertyModifications(UnityEngine.Object targetPrefab);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPropertyModifications(UnityEngine.Object targetPrefab, PropertyModification[] modifications);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object InstantiateAttachedAsset(UnityEngine.Object targetObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RecordPrefabInstancePropertyModifications(UnityEngine.Object targetObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MergeAllPrefabInstances(UnityEngine.Object targetObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisconnectPrefabInstance(UnityEngine.Object targetObject);

		public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target)
		{
			return PrefabUtility.InternalInstantiatePrefab(target, EditorSceneManager.GetTargetSceneForNewGameObjects());
		}

		public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target, Scene destinationScene)
		{
			return PrefabUtility.InternalInstantiatePrefab(target, destinationScene);
		}

		private static UnityEngine.Object InternalInstantiatePrefab(UnityEngine.Object target, Scene destinationScene)
		{
			return PrefabUtility.INTERNAL_CALL_InternalInstantiatePrefab(target, ref destinationScene);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_InternalInstantiatePrefab(UnityEngine.Object target, ref Scene destinationScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object CreateEmptyPrefab(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject CreatePrefab(string path, GameObject go, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);

		[ExcludeFromDocs]
		public static GameObject CreatePrefab(string path, GameObject go)
		{
			ReplacePrefabOptions options = ReplacePrefabOptions.Default;
			return PrefabUtility.CreatePrefab(path, go, options);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);

		[ExcludeFromDocs]
		public static GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab)
		{
			ReplacePrefabOptions options = ReplacePrefabOptions.Default;
			return PrefabUtility.ReplacePrefab(go, targetPrefab, options);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject ConnectGameObjectToPrefab(GameObject go, GameObject sourcePrefab);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject FindRootGameObjectWithSameParentPrefab(GameObject target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject FindValidUploadPrefabInstanceRoot(GameObject target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ReconnectToLastPrefab(GameObject go);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ResetToPrefabState(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsComponentAddedToPrefabInstance(UnityEngine.Object source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RevertPrefabInstance(GameObject go);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern PrefabType GetPrefabType(UnityEngine.Object target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject FindPrefabRoot(GameObject source);

		private static void Internal_CallPrefabInstanceUpdated(GameObject instance)
		{
			if (PrefabUtility.prefabInstanceUpdated != null)
			{
				PrefabUtility.prefabInstanceUpdated(instance);
			}
		}
	}
}
