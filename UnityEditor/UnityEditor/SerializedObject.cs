using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class SerializedObject
	{
		private IntPtr m_Property;

		public extern UnityEngine.Object targetObject
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern UnityEngine.Object[] targetObjects
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern UnityEngine.Object context
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool hasModifiedProperties
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern InspectorMode inspectorMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isEditingMultipleObjects
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int maxArraySizeForMultiEditing
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public SerializedObject(UnityEngine.Object obj)
		{
			this.InternalCreate(new UnityEngine.Object[]
			{
				obj
			}, null);
		}

		public SerializedObject(UnityEngine.Object obj, UnityEngine.Object context)
		{
			this.InternalCreate(new UnityEngine.Object[]
			{
				obj
			}, context);
		}

		public SerializedObject(UnityEngine.Object[] objs)
		{
			this.InternalCreate(objs, null);
		}

		public SerializedObject(UnityEngine.Object[] objs, UnityEngine.Object context)
		{
			this.InternalCreate(objs, context);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreate(UnityEngine.Object[] monoObjs, UnityEngine.Object context);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetIsDifferentCacheDirty();

		[Obsolete("UpdateIfDirtyOrScript has been deprecated. Use UpdateIfRequiredOrScript instead.", false), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateIfDirtyOrScript();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool UpdateIfRequiredOrScript();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern SerializedProperty GetIterator_Internal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Cache(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern SerializedObject LoadFromCache(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern PropertyModification ExtractPropertyModification(string propertyPath);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ApplyModifiedProperties();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ApplyModifiedPropertiesWithoutUndo();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyFromSerializedProperty(SerializedProperty prop);
	}
}
