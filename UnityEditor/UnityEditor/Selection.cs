using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class Selection
	{
		public static Action selectionChanged;

		public static extern Transform[] transforms
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Transform activeTransform
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern GameObject[] gameObjects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GameObject activeGameObject
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UnityEngine.Object activeObject
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int activeInstanceID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UnityEngine.Object[] objects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int[] instanceIDs
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string[] assetGUIDsDeepSelection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string[] assetGUIDs
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool Contains(int instanceID)
		{
			return Array.IndexOf<int>(Selection.instanceIDs, instanceID) != -1;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Transform[] GetTransforms(SelectionMode mode);

		public static UnityEngine.Object[] GetFiltered(Type type, SelectionMode mode)
		{
			ArrayList arrayList = new ArrayList();
			if (type == typeof(Component) || type.IsSubclassOf(typeof(Component)))
			{
				Transform[] transforms = Selection.GetTransforms(mode);
				Transform[] array = transforms;
				for (int i = 0; i < array.Length; i++)
				{
					Transform transform = array[i];
					Component component = transform.GetComponent(type);
					if (component)
					{
						arrayList.Add(component);
					}
				}
			}
			else if (type == typeof(GameObject) || type.IsSubclassOf(typeof(GameObject)))
			{
				Transform[] transforms2 = Selection.GetTransforms(mode);
				Transform[] array2 = transforms2;
				for (int j = 0; j < array2.Length; j++)
				{
					Transform transform2 = array2[j];
					arrayList.Add(transform2.gameObject);
				}
			}
			else
			{
				UnityEngine.Object[] objectsMode = Selection.GetObjectsMode(mode);
				UnityEngine.Object[] array3 = objectsMode;
				for (int k = 0; k < array3.Length; k++)
				{
					UnityEngine.Object @object = array3[k];
					if (@object != null)
					{
						if (@object.GetType() == type || @object.GetType().IsSubclassOf(type))
						{
							arrayList.Add(@object);
						}
					}
				}
			}
			return (UnityEngine.Object[])arrayList.ToArray(typeof(UnityEngine.Object));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object[] GetObjectsMode(SelectionMode mode);

		private static void Internal_CallSelectionChanged()
		{
			if (Selection.selectionChanged != null)
			{
				Selection.selectionChanged();
			}
		}
	}
}
