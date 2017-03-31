using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[UsedByNativeCode]
	public class Sampler
	{
		internal IntPtr m_Ptr;

		internal static Sampler s_InvalidSampler = new Sampler();

		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		[ThreadAndSerializationSafe]
		public extern string name
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal Sampler()
		{
		}

		public Recorder GetRecorder()
		{
			Recorder recorderInternal = this.GetRecorderInternal();
			return recorderInternal ?? Recorder.s_InvalidRecorder;
		}

		public static Sampler Get(string name)
		{
			Sampler samplerInternal = Sampler.GetSamplerInternal(name);
			return samplerInternal ?? Sampler.s_InvalidSampler;
		}

		public static int GetNames(List<string> names)
		{
			return Sampler.GetSamplerNamesInternal(names);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Recorder GetRecorderInternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Sampler GetSamplerInternal(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSamplerNamesInternal(object namesScriptingPtr);
	}
}
