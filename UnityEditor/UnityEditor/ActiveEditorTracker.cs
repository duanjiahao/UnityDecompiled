using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	public sealed class ActiveEditorTracker
	{
		private MonoReloadableIntPtrClear m_Property;

		public extern Editor[] activeEditors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isDirty
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isLocked
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern InspectorMode inspectorMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasComponentsWhichCannotBeMultiEdited
		{
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

		[ThreadAndSerializationSafe]
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

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Dispose();

		~ActiveEditorTracker()
		{
			this.Dispose();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Destroy();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetVisible(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetVisible(int index, int visible);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearDirty();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RebuildIfNecessary();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ForceRebuild();

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetupSharedTracker(ActiveEditorTracker sharedTracker);
	}
}
