using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[Serializable]
	public sealed class ActiveEditorTracker
	{
		private MonoReloadableIntPtrClear m_Property;

		public extern Editor[] activeEditors
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isDirty
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isLocked
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern InspectorMode inspectorMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasComponentsWhichCannotBeMultiEdited
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static ActiveEditorTracker sharedTracker
		{
			get
			{
				ActiveEditorTracker activeEditorTracker = new ActiveEditorTracker();
				ActiveEditorTracker.SetupSharedTracker(activeEditorTracker);
				return activeEditorTracker;
			}
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ActiveEditorTracker();

		public override bool Equals(object o)
		{
			ActiveEditorTracker activeEditorTracker = o as ActiveEditorTracker;
			return this.m_Property.m_IntPtr == activeEditorTracker.m_Property.m_IntPtr;
		}

		public override int GetHashCode()
		{
			return this.m_Property.m_IntPtr.GetHashCode();
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Dispose();

		~ActiveEditorTracker()
		{
			this.Dispose();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Destroy();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetVisible(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetVisible(int index, int visible);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearDirty();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RebuildIfNecessary();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ForceRebuild();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void VerifyModifiedMonoBehaviours();

		[Obsolete("Use Editor.CreateEditor instead")]
		public static Editor MakeCustomEditor(UnityEngine.Object obj)
		{
			return Editor.CreateEditor(obj);
		}

		public static bool HasCustomEditor(UnityEngine.Object obj)
		{
			return CustomEditorAttributes.FindCustomEditorType(obj, false) != null;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetupSharedTracker(ActiveEditorTracker sharedTracker);
	}
}
