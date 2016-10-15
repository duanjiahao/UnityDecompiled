using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LineRenderer : Renderer
	{
		public extern bool useWorldSpace
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void SetWidth(float start, float end)
		{
			LineRenderer.INTERNAL_CALL_SetWidth(this, start, end);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetWidth(LineRenderer self, float start, float end);

		public void SetColors(Color start, Color end)
		{
			LineRenderer.INTERNAL_CALL_SetColors(this, ref start, ref end);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetColors(LineRenderer self, ref Color start, ref Color end);

		public void SetVertexCount(int count)
		{
			LineRenderer.INTERNAL_CALL_SetVertexCount(this, count);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetVertexCount(LineRenderer self, int count);

		public void SetPosition(int index, Vector3 position)
		{
			LineRenderer.INTERNAL_CALL_SetPosition(this, index, ref position);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPosition(LineRenderer self, int index, ref Vector3 position);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPositions(Vector3[] positions);
	}
}
