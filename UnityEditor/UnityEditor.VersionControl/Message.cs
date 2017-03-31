using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor.VersionControl
{
	public sealed class Message
	{
		public enum Severity
		{
			Data,
			Verbose,
			Info,
			Warning,
			Error
		}

		private IntPtr m_thisDummy;

		[ThreadAndSerializationSafe]
		public extern Message.Severity severity
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public extern string message
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal Message()
		{
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~Message()
		{
			this.Dispose();
		}

		public void Show()
		{
			Message.Info(this.message);
		}

		private static void Info(string message)
		{
			Debug.Log("Version control:\n" + message);
		}
	}
}
