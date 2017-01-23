using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LineRenderer : Renderer
	{
		public extern float startWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float endWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimationCurve widthCurve
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float widthMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color startColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_startColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_startColor(ref value);
			}
		}

		public Color endColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_endColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_endColor(ref value);
			}
		}

		public extern Gradient colorGradient
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int numPositions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useWorldSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int numCornerVertices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int numCapVertices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern LineTextureMode textureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("SetWidth has been deprecated. Please use the startWidth, endWidth, or widthCurve properties instead.")]
		public void SetWidth(float start, float end)
		{
			this.startWidth = start;
			this.endWidth = end;
		}

		[Obsolete("SetColors has been deprecated. Please use the startColor, endColor, or colorGradient properties instead.")]
		public void SetColors(Color start, Color end)
		{
			this.startColor = start;
			this.endColor = end;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_startColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_startColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_endColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_endColor(ref Color value);

		[Obsolete("SetVertexCount has been deprecated. Please use the numPositions property instead.")]
		public void SetVertexCount(int count)
		{
			this.numPositions = count;
		}

		public void SetPosition(int index, Vector3 position)
		{
			LineRenderer.INTERNAL_CALL_SetPosition(this, index, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPosition(LineRenderer self, int index, ref Vector3 position);

		public Vector3 GetPosition(int index)
		{
			Vector3 result;
			LineRenderer.INTERNAL_CALL_GetPosition(this, index, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPosition(LineRenderer self, int index, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPositions(Vector3[] positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetPositions(Vector3[] positions);
	}
}
