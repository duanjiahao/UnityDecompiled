using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[UsedByNativeCode]
	public sealed class Recorder
	{
		internal IntPtr m_Ptr;

		internal static Recorder s_InvalidRecorder = new Recorder();

		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		[ThreadAndSerializationSafe]
		public extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[ThreadAndSerializationSafe]
		public extern long elapsedNanoseconds
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public extern int sampleBlockCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal Recorder()
		{
		}

		~Recorder()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				this.DisposeNative();
			}
		}

		public static Recorder Get(string samplerName)
		{
			Sampler sampler = Sampler.Get(samplerName);
			return sampler.GetRecorder();
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DisposeNative();
	}
}
