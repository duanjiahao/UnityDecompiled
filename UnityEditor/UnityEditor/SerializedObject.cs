using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class SerializedObject
	{
		private IntPtr m_Property;

		public extern UnityEngine.Object targetObject
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern UnityEngine.Object[] targetObjects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool hasModifiedProperties
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern InspectorMode inspectorMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isEditingMultipleObjects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int maxArraySizeForMultiEditing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public SerializedObject(UnityEngine.Object obj)
		{
			this.InternalCreate(new UnityEngine.Object[]
			{
				obj
			});
		}

		public SerializedObject(UnityEngine.Object[] objs)
		{
			this.InternalCreate(objs);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreate(UnityEngine.Object[] monoObjs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetIsDifferentCacheDirty();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateIfDirtyOrScript();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~SerializedObject()
		{
			this.Dispose();
		}

		public SerializedProperty GetIterator()
		{
			SerializedProperty iterator_Internal = this.GetIterator_Internal();
			iterator_Internal.m_SerializedObject = this;
			return iterator_Internal;
		}

		public SerializedProperty FindProperty(string propertyPath)
		{
			SerializedProperty iterator_Internal = this.GetIterator_Internal();
			iterator_Internal.m_SerializedObject = this;
			SerializedProperty result;
			if (iterator_Internal.FindPropertyInternal(propertyPath))
			{
				result = iterator_Internal;
			}
			else
			{
				result = null;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern SerializedProperty GetIterator_Internal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Cache(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern SerializedObject LoadFromCache(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern PropertyModification ExtractPropertyModification(string propertyPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ApplyModifiedProperties();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ApplyModifiedPropertiesWithoutUndo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyFromSerializedProperty(SerializedProperty prop);
	}
}
