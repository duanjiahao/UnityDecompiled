using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class CullingGroup : IDisposable
	{
		public delegate void StateChanged(CullingGroupEvent sphere);

		internal IntPtr m_Ptr;

		private CullingGroup.StateChanged m_OnStateChanged = null;

		public CullingGroup.StateChanged onStateChanged
		{
			get
			{
				return this.m_OnStateChanged;
			}
			set
			{
				this.m_OnStateChanged = value;
			}
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Camera targetCamera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public CullingGroup()
		{
			this.Init();
		}

		~CullingGroup()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				this.FinalizerFailure();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBoundingSpheres(BoundingSphere[] array);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBoundingSphereCount(int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EraseSwapBack(int index);

		public static void EraseSwapBack<T>(int index, T[] myArray, ref int size)
		{
			size--;
			myArray[index] = myArray[size];
		}

		public int QueryIndices(bool visible, int[] result, int firstIndex)
		{
			return this.QueryIndices(visible, -1, CullingQueryOptions.IgnoreDistance, result, firstIndex);
		}

		public int QueryIndices(int distanceIndex, int[] result, int firstIndex)
		{
			return this.QueryIndices(false, distanceIndex, CullingQueryOptions.IgnoreVisibility, result, firstIndex);
		}

		public int QueryIndices(bool visible, int distanceIndex, int[] result, int firstIndex)
		{
			return this.QueryIndices(visible, distanceIndex, CullingQueryOptions.Normal, result, firstIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int QueryIndices(bool visible, int distanceIndex, CullingQueryOptions options, int[] result, int firstIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsVisible(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetDistance(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBoundingDistances(float[] distances);

		public void SetDistanceReferencePoint(Vector3 point)
		{
			CullingGroup.INTERNAL_CALL_SetDistanceReferencePoint(this, ref point);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetDistanceReferencePoint(CullingGroup self, ref Vector3 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDistanceReferencePoint(Transform transform);

		[SecuritySafeCritical, RequiredByNativeCode]
		private unsafe static void SendEvents(CullingGroup cullingGroup, IntPtr eventsPtr, int count)
		{
			CullingGroupEvent* ptr = (CullingGroupEvent*)eventsPtr.ToPointer();
			if (cullingGroup.m_OnStateChanged != null)
			{
				for (int i = 0; i < count; i++)
				{
					cullingGroup.m_OnStateChanged(ptr[i]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Init();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void FinalizerFailure();
	}
}
