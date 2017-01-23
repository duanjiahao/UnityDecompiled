using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	public sealed class ChangeSet
	{
		private IntPtr m_thisDummy;

		public static string defaultID = "-1";

		[ThreadAndSerializationSafe]
		public extern string description
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public extern string id
		{
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

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreate();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCopyConstruct(ChangeSet other);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromString(string description);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromStringString(string description, string changeSetID);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ChangeSet()
		{
			this.Dispose();
		}
	}
}
