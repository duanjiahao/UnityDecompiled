using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Sprite : Object
	{
		public Bounds bounds
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_bounds(out result);
				return result;
			}
		}

		public Rect rect
		{
			get
			{
				Rect result;
				this.INTERNAL_get_rect(out result);
				return result;
			}
		}

		public extern float pixelsPerUnit
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Texture2D texture
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Texture2D associatedAlphaSplitTexture
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Rect textureRect
		{
			get
			{
				Rect result;
				this.INTERNAL_get_textureRect(out result);
				return result;
			}
		}

		public Vector2 textureRectOffset
		{
			get
			{
				Vector2 result;
				Sprite.Internal_GetTextureRectOffset(this, out result);
				return result;
			}
		}

		public extern bool packed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SpritePackingMode packingMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SpritePackingRotation packingRotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector2 pivot
		{
			get
			{
				Vector2 result;
				Sprite.Internal_GetPivot(this, out result);
				return result;
			}
		}

		public Vector4 border
		{
			get
			{
				Vector4 result;
				this.INTERNAL_get_border(out result);
				return result;
			}
		}

		public extern Vector2[] vertices
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ushort[] triangles
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Vector2[] uv
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, [DefaultValue("100.0f")] float pixelsPerUnit, [DefaultValue("0")] uint extrude, [DefaultValue("SpriteMeshType.Tight")] SpriteMeshType meshType, [DefaultValue("Vector4.zero")] Vector4 border)
		{
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType)
		{
			Vector4 zero = Vector4.zero;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType meshType = SpriteMeshType.Tight;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType meshType = SpriteMeshType.Tight;
			uint extrude = 0u;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType meshType = SpriteMeshType.Tight;
			uint extrude = 0u;
			float pixelsPerUnit = 100f;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Sprite INTERNAL_CALL_Create(Texture2D texture, ref Rect rect, ref Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, ref Vector4 border);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bounds(out Bounds value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rect(out Rect value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_textureRect(out Rect value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTextureRectOffset(Sprite sprite, out Vector2 output);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetPivot(Sprite sprite, out Vector2 output);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_border(out Vector4 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OverrideGeometry(Vector2[] vertices, ushort[] triangles);
	}
}
