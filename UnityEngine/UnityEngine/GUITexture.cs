using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class GUITexture : GUIElement
	{
		public Color color
		{
			get
			{
				Color result;
				this.INTERNAL_get_color(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_color(ref value);
			}
		}

		public extern Texture texture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Rect pixelInset
		{
			get
			{
				Rect result;
				this.INTERNAL_get_pixelInset(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_pixelInset(ref value);
			}
		}

		public extern RectOffset border
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_color(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_color(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pixelInset(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_pixelInset(ref Rect value);
	}
}
