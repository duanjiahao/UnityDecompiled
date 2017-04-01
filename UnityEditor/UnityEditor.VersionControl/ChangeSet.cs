using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor.VersionControl
{
	public sealed class ChangeSet
	{
		private IntPtr m_thisDummy;

		public static string defaultID = "-1";

		[ThreadAndSerializationSafe]
		public extern string description
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public extern string id
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public ChangeSet()
		{
			this.InternalCreate();
		}

		public ChangeSet(string description)
		{
			this.InternalCreateFromString(description);
		}

		public ChangeSet(string description, string revision)
		{
			this.InternalCreateFromStringString(description, revision);
		}

		public ChangeSet(ChangeSet other)
		{
			this.InternalCopyConstruct(other);
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreate();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCopyConstruct(ChangeSet other);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromString(string description);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromStringString(string description, string changeSetID);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ChangeSet()
		{
			this.Dispose();
		}
	}
}
