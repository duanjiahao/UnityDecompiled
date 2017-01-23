using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	public sealed class ProfilerProperty
	{
		private IntPtr m_Ptr;

		public extern bool HasChildren
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool onlyShowGPUSamples
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int[] instanceIDs
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string propertyPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string frameFPS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string frameTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string frameGpuTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool frameDataReady
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProfilerProperty();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ProfilerProperty()
		{
			this.Dispose();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Cleanup();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Next(bool enterChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetRoot(int frame, ProfilerColumn profilerSortColumn, ProfilerViewType viewType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitializeDetailProperty(ProfilerProperty source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetTooltip(ProfilerColumn column);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetColumn(ProfilerColumn column);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AudioProfilerGroupInfo[] GetAudioProfilerGroupInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AudioProfilerDSPInfo[] GetAudioProfilerDSPInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AudioProfilerClipInfo[] GetAudioProfilerClipInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAudioProfilerNameByOffset(int offset);
	}
}
