using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	public sealed class ConfigField
	{
		private IntPtr m_thisDummy;

		private string m_guid;

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string label
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string description
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isRequired
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isPassword
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal ConfigField()
		{
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ConfigField()
		{
			this.Dispose();
		}
	}
}
