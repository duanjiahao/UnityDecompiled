using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	public sealed class Plugin
	{
		private IntPtr m_thisDummy;

		private string m_guid;

		public static extern Plugin[] availablePlugins
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ConfigField[] configFields
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal Plugin()
		{
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();
	}
}
