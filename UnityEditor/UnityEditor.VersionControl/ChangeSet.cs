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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public extern string id
		{
			[WrapperlessIcall]
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

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreate();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCopyConstruct(ChangeSet other);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromString(string description);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromStringString(string description, string changeSetID);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ChangeSet()
		{
			this.Dispose();
		}
	}
}
