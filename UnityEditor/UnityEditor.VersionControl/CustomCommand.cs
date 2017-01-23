using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	internal sealed class CustomCommand
	{
		private IntPtr m_thisDummy;

		[ThreadAndSerializationSafe]
		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public extern string label
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public extern CommandContext context
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal CustomCommand()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Task StartTask();
	}
}
