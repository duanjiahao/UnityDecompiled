using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor.VersionControl
{
	public sealed class ChangeSet
	{
		private IntPtr m_thisDummy;
		public extern string description
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreate();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCopyConstruct(ChangeSet other);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromString(string description);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalCreateFromStringString(string description, string changeSetID);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();
		~ChangeSet()
		{
			this.Dispose();
		}
	}
}
