using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class Selection
	{
		public static Action selectionChanged;

		public static extern Transform[] transforms
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Transform activeTransform
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern GameObject[] gameObjects
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GameObject activeGameObject
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UnityEngine.Object activeObject
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UnityEngine.Object activeContext
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int activeInstanceID
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UnityEngine.Object[] objects
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int[] instanceIDs
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string[] assetGUIDsDeepSelection
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string[] assetGUIDs
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool Contains(int instanceID)
		{
			return Array.IndexOf<int>(Selection.instanceIDs, instanceID) != -1;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetActiveObjectWithContext(UnityEngine.Object obj, UnityEngine.Object context);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Transform[] GetTransforms(SelectionMode mode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object[] GetObjectsMode(SelectionMode mode);

		private static void Internal_CallSelectionChanged()
		{
			if (Selection.selectionChanged != null)
			{
				Selection.selectionChanged();
			}
		}

		public static bool Contains(UnityEngine.Object obj)
		{
			return Selection.Contains(obj.GetInstanceID());
		}

		internal static void Add(int instanceID)
		{
			List<int> list = new List<int>(Selection.instanceIDs);
			if (list.IndexOf(instanceID) < 0)
			{
				list.Add(instanceID);
				Selection.instanceIDs = list.ToArray();
			}
		}

		internal static void Add(UnityEngine.Object obj)
		{
			if (obj != null)
			{
				Selection.Add(obj.GetInstanceID());
			}
		}

		internal static void Remove(int instanceID)
		{
			List<int> list = new List<int>(Selection.instanceIDs);
			list.Remove(instanceID);
			Selection.instanceIDs = list.ToArray();
		}

		internal static void Remove(UnityEngine.Object obj)
		{
			if (obj != null)
			{
				Selection.Remove(obj.GetInstanceID());
			}
		}

		private static IEnumerable GetFilteredInternal(Type type, SelectionMode mode)
		{
			IEnumerable result;
			if (typeof(Component).IsAssignableFrom(type) || type.IsInterface)
			{
				result = from t in Selection.GetTransforms(mode)
				select t.GetComponent(type) into c
				where c != null
				select c;
			}
			else if (typeof(GameObject).IsAssignableFrom(type))
			{
				result = from t in Selection.GetTransforms(mode)
				select t.gameObject;
			}
			else
			{
				result = from o in Selection.GetObjectsMode(mode)
				where o != null && type.IsAssignableFrom(o.GetType())
				select o;
			}
			return result;
		}

		public static T[] GetFiltered<T>(SelectionMode mode)
		{
			return Selection.GetFilteredInternal(typeof(T), mode).Cast<T>().ToArray<T>();
		}

		public static UnityEngine.Object[] GetFiltered(Type type, SelectionMode mode)
		{
			return Selection.GetFilteredInternal(type, mode).Cast<UnityEngine.Object>().ToArray<UnityEngine.Object>();
		}
	}
}
