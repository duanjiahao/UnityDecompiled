using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	public sealed class PrefabUtility
	{
		public delegate void PrefabInstanceUpdated(GameObject instance);

		public static PrefabUtility.PrefabInstanceUpdated prefabInstanceUpdated;

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetPrefabParent(UnityEngine.Object source);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetPrefabObject(UnityEngine.Object targetObject);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern PropertyModification[] GetPropertyModifications(UnityEngine.Object targetPrefab);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPropertyModifications(UnityEngine.Object targetPrefab, PropertyModification[] modifications);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object InstantiateAttachedAsset(UnityEngine.Object targetObject);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RecordPrefabInstancePropertyModifications(UnityEngine.Object targetObject);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MergeAllPrefabInstances(UnityEngine.Object targetObject);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisconnectPrefabInstance(UnityEngine.Object targetObject);

		public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target)
		{
			return PrefabUtility.InternalInstantiatePrefab(target, SceneManager.GetActiveScene());
		}

		public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target, Scene destinationScene)
		{
			return PrefabUtility.InternalInstantiatePrefab(target, destinationScene);
		}

		private static UnityEngine.Object InternalInstantiatePrefab(UnityEngine.Object target, Scene destinationScene)
		{
			return PrefabUtility.INTERNAL_CALL_InternalInstantiatePrefab(target, ref destinationScene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_InternalInstantiatePrefab(UnityEngine.Object target, ref Scene destinationScene);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object CreateEmptyPrefab(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject CreatePrefab(string path, GameObject go, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);

		[ExcludeFromDocs]
		public static GameObject CreatePrefab(string path, GameObject go)
		{
			ReplacePrefabOptions options = ReplacePrefabOptions.Default;
			return PrefabUtility.CreatePrefab(path, go, options);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);

		[ExcludeFromDocs]
		public static GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab)
		{
			ReplacePrefabOptions options = ReplacePrefabOptions.Default;
			return PrefabUtility.ReplacePrefab(go, targetPrefab, options);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject ConnectGameObjectToPrefab(GameObject go, GameObject sourcePrefab);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject FindRootGameObjectWithSameParentPrefab(GameObject target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject FindValidUploadPrefabInstanceRoot(GameObject target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ReconnectToLastPrefab(GameObject go);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ResetToPrefabState(UnityEngine.Object obj);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsComponentAddedToPrefabInstance(UnityEngine.Object source);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RevertPrefabInstance(GameObject go);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern PrefabType GetPrefabType(UnityEngine.Object target);

		[WrapperlessIcall]
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
