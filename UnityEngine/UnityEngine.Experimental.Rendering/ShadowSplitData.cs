using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	[UsedByNativeCode]
	public struct ShadowSplitData
	{
		[CompilerGenerated, UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 160)]
		public struct <_cullingPlanes>__FixedBuffer2
		{
			public float FixedElementField;
		}

		public int cullingPlaneCount;

		private ShadowSplitData.<_cullingPlanes>__FixedBuffer2 _cullingPlanes;

		public Vector4 cullingSphere;

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
