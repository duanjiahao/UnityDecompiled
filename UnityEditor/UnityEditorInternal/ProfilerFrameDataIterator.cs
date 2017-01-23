using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	public sealed class ProfilerFrameDataIterator
	{
		private IntPtr m_Ptr;

		public extern int group
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string path
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int sampleId
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int instanceId
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float frameTimeMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float startTimeMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float durationMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Use instanceId instead (UnityUpgradable) -> instanceId")]
		public int id
		{
			get
			{
				return this.instanceId;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProfilerFrameDataIterator();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ProfilerFrameDataIterator()
		{
			this.Dispose();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Next(bool enterChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetThreadCount(int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern double GetFrameStartS(int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetGroupCount(int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetGroupName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetThreadName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetRoot(int frame, int threadIdx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetIsNativeAllocation();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetAllocationCallstack(ref string resolvedStack);
	}
}
