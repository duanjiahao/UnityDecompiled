using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA
{
	public sealed class SurfaceObserver : IDisposable
	{
		public delegate void SurfaceChangedDelegate(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, DateTime updateTime);

		public delegate void SurfaceDataReadyDelegate(SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds);

		private IntPtr m_Observer;

		public SurfaceObserver()
		{
			this.m_Observer = this.Internal_Create();
		}

		[RequiredByNativeCode]
		private static void InvokeSurfaceChangedEvent(SurfaceObserver.SurfaceChangedDelegate onSurfaceChanged, int surfaceId, SurfaceChange changeType, Bounds bounds, long updateTime)
		{
			if (onSurfaceChanged != null)
			{
				SurfaceId surfaceId2;
				surfaceId2.handle = surfaceId;
				onSurfaceChanged(surfaceId2, changeType, bounds, DateTime.FromFileTime(updateTime));
			}
		}

		[RequiredByNativeCode]
		private static void InvokeSurfaceDataReadyEvent(SurfaceObserver.SurfaceDataReadyDelegate onDataReady, int surfaceId, MeshFilter outputMesh, WorldAnchor outputAnchor, MeshCollider outputCollider, float trisPerCubicMeter, bool bakeCollider, bool outputWritten, float elapsedBakeTimeSeconds)
		{
			if (onDataReady != null)
			{
				SurfaceData bakedData;
				bakedData.id.handle = surfaceId;
				bakedData.outputMesh = outputMesh;
				bakedData.outputAnchor = outputAnchor;
				bakedData.outputCollider = outputCollider;
				bakedData.trianglesPerCubicMeter = trisPerCubicMeter;
				bakedData.bakeCollider = bakeCollider;
				onDataReady(bakedData, outputWritten, elapsedBakeTimeSeconds);
			}
		}

		~SurfaceObserver()
		{
			if (this.m_Observer != IntPtr.Zero)
			{
				SurfaceObserver.DestroyThreaded(this.m_Observer);
				this.m_Observer = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		public void SetVolumeAsAxisAlignedBox(Vector3 origin, Vector3 extents)
		{
			this.Internal_SetVolumeAsAxisAlignedBox(this.m_Observer, origin, extents);
		}

		public void SetVolumeAsSphere(Vector3 origin, float radiusMeters)
		{
			this.Internal_SetVolumeAsSphere(this.m_Observer, origin, radiusMeters);
		}

		public void SetVolumeAsOrientedBox(Vector3 origin, Vector3 extents, Quaternion orientation)
		{
			this.Internal_SetVolumeAsOrientedBox(this.m_Observer, origin, extents, orientation);
		}

		public void SetVolumeAsFrustum(Plane[] planes)
		{
			if (planes == null)
			{
				throw new ArgumentNullException("planes");
			}
			if (planes.Length != 6)
			{
				throw new ArgumentException("Planes array must be 6 items long", "planes");
			}
			this.Internal_SetVolumeAsFrustum(this.m_Observer, planes);
		}

		public void Update(SurfaceObserver.SurfaceChangedDelegate onSurfaceChanged)
		{
			if (onSurfaceChanged == null)
			{
				throw new ArgumentNullException("onSurfaceChanged");
			}
			SurfaceObserver.Internal_Update(this.m_Observer, onSurfaceChanged);
		}

		public bool RequestMeshAsync(SurfaceData dataRequest, SurfaceObserver.SurfaceDataReadyDelegate onDataReady)
		{
			if (onDataReady == null)
			{
				throw new ArgumentNullException("onDataReady");
			}
			if (dataRequest.outputMesh == null)
			{
				throw new ArgumentNullException("dataRequest.outputMesh");
			}
			if (dataRequest.outputAnchor == null)
			{
				throw new ArgumentNullException("dataRequest.outputAnchor");
			}
			if (dataRequest.outputCollider == null && dataRequest.bakeCollider)
			{
				throw new ArgumentException("dataRequest.outputCollider must be non-NULL if dataRequest.bakeCollider is true", "dataRequest.outputCollider");
			}
			if ((double)dataRequest.trianglesPerCubicMeter < 0.0)
			{
				throw new ArgumentException("dataRequest.trianglesPerCubicMeter must be greater than zero", "dataRequest.trianglesPerCubicMeter");
			}
			bool flag = SurfaceObserver.Internal_AddToWorkQueue(this.m_Observer, onDataReady, dataRequest.id.handle, dataRequest.outputMesh, dataRequest.outputAnchor, dataRequest.outputCollider, dataRequest.trianglesPerCubicMeter, dataRequest.bakeCollider);
			if (!flag)
			{
				Debug.LogError("RequestMeshAsync has failed.  Is your surface ID valid?");
			}
			return flag;
		}

		public void Dispose()
		{
			if (this.m_Observer != IntPtr.Zero)
			{
				SurfaceObserver.Destroy(this.m_Observer);
				this.m_Observer = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		private IntPtr Internal_Create()
		{
			IntPtr result;
			SurfaceObserver.INTERNAL_CALL_Internal_Create(this, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Create(SurfaceObserver self, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr observer);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr observer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Update(IntPtr observer, SurfaceObserver.SurfaceChangedDelegate onSurfaceChanged);

		private void Internal_SetVolumeAsAxisAlignedBox(IntPtr observer, Vector3 origin, Vector3 extents)
		{
			SurfaceObserver.INTERNAL_CALL_Internal_SetVolumeAsAxisAlignedBox(this, observer, ref origin, ref extents);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetVolumeAsAxisAlignedBox(SurfaceObserver self, IntPtr observer, ref Vector3 origin, ref Vector3 extents);

		private void Internal_SetVolumeAsOrientedBox(IntPtr observer, Vector3 origin, Vector3 extents, Quaternion orientation)
		{
			SurfaceObserver.INTERNAL_CALL_Internal_SetVolumeAsOrientedBox(this, observer, ref origin, ref extents, ref orientation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetVolumeAsOrientedBox(SurfaceObserver self, IntPtr observer, ref Vector3 origin, ref Vector3 extents, ref Quaternion orientation);

		private void Internal_SetVolumeAsSphere(IntPtr observer, Vector3 origin, float radiusMeters)
		{
			SurfaceObserver.INTERNAL_CALL_Internal_SetVolumeAsSphere(this, observer, ref origin, radiusMeters);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetVolumeAsSphere(SurfaceObserver self, IntPtr observer, ref Vector3 origin, float radiusMeters);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetVolumeAsFrustum(IntPtr observer, Plane[] planes);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_AddToWorkQueue(IntPtr observer, SurfaceObserver.SurfaceDataReadyDelegate onDataReady, int surfaceId, MeshFilter filter, WorldAnchor wa, MeshCollider mc, float trisPerCubicMeter, bool createColliderData);
	}
}
