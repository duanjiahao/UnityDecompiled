using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	public sealed class Undo
	{
		public delegate void UndoRedoCallback();

		public delegate void WillFlushUndoRecord();

		public delegate UndoPropertyModification[] PostprocessModifications(UndoPropertyModification[] modifications);

		public static Undo.UndoRedoCallback undoRedoPerformed;

		public static Undo.WillFlushUndoRecord willFlushUndoRecord;

		public static Undo.PostprocessModifications postprocessModifications;

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRecordsInternal(object undoRecords, object redoRecords);

		internal static void GetRecords(List<string> undoRecords, List<string> redoRecords)
		{
			Undo.GetRecordsInternal(undoRecords, redoRecords);
		}

		public static void RegisterCompleteObjectUndo(UnityEngine.Object objectToUndo, string name)
		{
			UnityEngine.Object[] objectsToUndo = new UnityEngine.Object[]
			{
				objectToUndo
			};
			Undo.RegisterCompleteObjectUndoMultiple(objectToUndo, objectsToUndo, name, 0);
		}

		public static void RegisterCompleteObjectUndo(UnityEngine.Object[] objectsToUndo, string name)
		{
			if (objectsToUndo.Length > 0)
			{
				Undo.RegisterCompleteObjectUndoMultiple(objectsToUndo[0], objectsToUndo, name, 0);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterCompleteObjectUndoMultiple(UnityEngine.Object identifier, UnityEngine.Object[] objectsToUndo, string name, int namePriority);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetTransformParent(Transform transform, Transform newParent, string name);

		public static void MoveGameObjectToScene(GameObject go, Scene scene, string name)
		{
			Undo.INTERNAL_CALL_MoveGameObjectToScene(go, ref scene, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveGameObjectToScene(GameObject go, ref Scene scene, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterCreatedObjectUndo(UnityEngine.Object objectToUndo, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyObjectImmediate(UnityEngine.Object objectToUndo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Component AddComponent(GameObject gameObject, Type type);

		public static T AddComponent<T>(GameObject gameObject) where T : Component
		{
			return Undo.AddComponent(gameObject, typeof(T)) as T;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterFullObjectHierarchyUndo(UnityEngine.Object objectToUndo, string name);

		[Obsolete("Use Undo.RegisterFullObjectHierarchyUndo(Object, string) instead")]
		public static void RegisterFullObjectHierarchyUndo(UnityEngine.Object objectToUndo)
		{
			Undo.RegisterFullObjectHierarchyUndo(objectToUndo, "Full Object Hierarchy");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RecordObject(UnityEngine.Object objectToUndo, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RecordObjects(UnityEngine.Object[] objectsToUndo, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearUndo(UnityEngine.Object identifier);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PerformUndo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PerformRedo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IncrementCurrentGroup();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCurrentGroup();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetCurrentGroupName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCurrentGroupName(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RevertAllInCurrentGroup();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RevertAllDownToGroup(int group);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CollapseUndoOperations(int groupIndex);

		[Obsolete("Use Undo.RegisterCompleteObjectUndo instead")]
		public static void RegisterUndo(UnityEngine.Object objectToUndo, string name)
		{
			Undo.RegisterCompleteObjectUndo(objectToUndo, name);
		}

		[Obsolete("Use Undo.RegisterCompleteObjectUndo instead")]
		public static void RegisterUndo(UnityEngine.Object[] objectsToUndo, string name)
		{
			Undo.RegisterCompleteObjectUndo(objectsToUndo, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FlushUndoRecordObjects();

		private static UndoPropertyModification[] InvokePostprocessModifications(UndoPropertyModification[] modifications)
		{
			UndoPropertyModification[] result;
			if (Undo.postprocessModifications != null)
			{
				result = Undo.postprocessModifications(modifications);
			}
			else
			{
				result = modifications;
			}
			return result;
		}

		private static void Internal_CallWillFlushUndoRecord()
		{
			if (Undo.willFlushUndoRecord != null)
			{
				Undo.willFlushUndoRecord();
			}
		}

		private static void Internal_CallUndoRedoPerformed()
		{
			if (Undo.undoRedoPerformed != null)
			{
				Undo.undoRedoPerformed();
			}
		}

		[Obsolete("Use Undo.RecordObject instead")]
		public static void SetSnapshotTarget(UnityEngine.Object objectToUndo, string name)
		{
		}

		[Obsolete("Use Undo.RecordObject instead")]
		public static void SetSnapshotTarget(UnityEngine.Object[] objectsToUndo, string name)
		{
		}

		[Obsolete("Use Undo.RecordObject instead")]
		public static void ClearSnapshotTarget()
		{
		}

		[Obsolete("Use Undo.RecordObject instead")]
		public static void CreateSnapshot()
		{
		}

		[Obsolete("Use Undo.RecordObject instead")]
		public static void RestoreSnapshot()
		{
		}

		[Obsolete("Use Undo.RecordObject instead")]
		public static void RegisterSnapshot()
		{
		}

		[Obsolete("Use DestroyObjectImmediate, RegisterCreatedObjectUndo or RegisterUndo instead.")]
		public static void RegisterSceneUndo(string name)
		{
		}
	}
}
