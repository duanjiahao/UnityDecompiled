using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	[UsedByNativeCode]
	public struct CullingParameters
	{
		[CompilerGenerated, UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 160)]
		public struct <_cullingPlanes>__FixedBuffer0
		{
			public float FixedElementField;
		}

		[CompilerGenerated, UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <_layerFarCullDistances>__FixedBuffer1
		{
			public float FixedElementField;
		}

		public int isOrthographic;

		public LODParameters lodParameters;

		private CullingParameters.<_cullingPlanes>__FixedBuffer0 _cullingPlanes;

		public int cullingPlaneCount;

		public int cullingMask;

		private CullingParameters.<_layerFarCullDistances>__FixedBuffer1 _layerFarCullDistances;

		private int layerCull;

		public Matrix4x4 cullingMatrix;

		public Vector3 position;

		public float shadowDistance;

		private int _cullingFlags;

		private int _cameraInstanceID;

		public ReflectionProbeSortOptions reflectionProbeSortOptions;

		public unsafe float GetLayerCullDistance(int layerIndex)
		{
			if (layerIndex < 0 || layerIndex >= 32)
			{
				throw new IndexOutOfRangeException("Invalid layer index");
			}
			return *(ref this._layerFarCullDistances.FixedElementField + (IntPtr)layerIndex * 4);
		}

		public unsafe void SetLayerCullDistance(int layerIndex, float distance)
		{
			if (layerIndex < 0 || layerIndex >= 32)
			{
				throw new IndexOutOfRangeException("Invalid layer index");
			}
			fixed (float* ptr = &this._layerFarCullDistances.FixedElementField)
			{
				ptr[(IntPtr)layerIndex * 4] = distance;
			}
		}

		public unsafe Plane GetCullingPlane(int index)
		{
			if (index < 0 || index >= this.cullingPlaneCount || index >= 10)
			{
				throw new IndexOutOfRangeException("Invalid plane index");
			}
			return new Plane(new Vector3(*(ref this._cullingPlanes.FixedElementField + (IntPtr)(index * 4) * 4), *(ref this._cullingPlanes.FixedElementField + (IntPtr)(index * 4 + 1) * 4), *(ref this._cullingPlanes.FixedElementField + (IntPtr)(index * 4 + 2) * 4)), *(ref this._cullingPlanes.FixedElementField + (IntPtr)(index * 4 + 3) * 4));
		}

		public unsafe void SetCullingPlane(int index, Plane plane)
		{
			if (index < 0 || index >= this.cullingPlaneCount || index >= 10)
			{
				throw new IndexOutOfRangeException("Invalid plane index");
			}
			fixed (float* ptr = &this._cullingPlanes.FixedElementField)
			{
				ptr[(IntPtr)(index * 4) * 4] = plane.normal.x;
				ptr[(IntPtr)(index * 4 + 1) * 4] = plane.normal.y;
				ptr[(IntPtr)(index * 4 + 2) * 4] = plane.normal.z;
				ptr[(IntPtr)(index * 4 + 3) * 4] = plane.distance;
			}
		}
	}
}
