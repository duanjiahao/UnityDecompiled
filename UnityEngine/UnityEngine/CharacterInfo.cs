using System;
namespace UnityEngine
{
	public struct CharacterInfo
	{
		public int index;
		[Obsolete("CharacterInfo.uv is deprecated. Use uvBottomLeft, uvBottomRight, uvTopRight or uvTopLeft instead.")]
		public Rect uv;
		[Obsolete("CharacterInfo.vert is deprecated. Use minX, maxX, minY, maxY instead.")]
		public Rect vert;
		[Obsolete("CharacterInfo.width is deprecated. Use advance instead.")]
		public float width;
		public int size;
		public FontStyle style;
		[Obsolete("CharacterInfo.flipped is deprecated. Use uvBottomLeft, uvBottomRight, uvTopRight or uvTopLeft instead, which will be correct regardless of orientation.")]
		public bool flipped;
		private int ascent;
		public int advance
		{
			get
			{
				return (int)this.width;
			}
		}
		public int glyphWidth
		{
			get
			{
				return (int)this.vert.width;
			}
		}
		public int glyphHeight
		{
			get
			{
				return (int)(-(int)this.vert.height);
			}
		}
		public int bearing
		{
			get
			{
				return (int)this.vert.x;
			}
		}
		public int minY
		{
			get
			{
				return this.ascent + (int)(this.vert.y + this.vert.height);
			}
		}
		public int maxY
		{
			get
			{
				return this.ascent + (int)this.vert.y;
			}
		}
		public int minX
		{
			get
			{
				return (int)this.vert.x;
			}
		}
		public int maxX
		{
			get
			{
				return (int)(this.vert.x + this.vert.width);
			}
		}
		internal Vector2 uvBottomLeftUnFlipped
		{
			get
			{
				return new Vector2(this.uv.x, this.uv.y);
			}
		}
		internal Vector2 uvBottomRightUnFlipped
		{
			get
			{
				return new Vector2(this.uv.x + this.uv.width, this.uv.y);
			}
		}
		internal Vector2 uvTopRightUnFlipped
		{
			get
			{
				return new Vector2(this.uv.x + this.uv.width, this.uv.y + this.uv.height);
			}
		}
		internal Vector2 uvTopLeftUnFlipped
		{
			get
			{
				return new Vector2(this.uv.x, this.uv.y + this.uv.height);
			}
		}
		public Vector2 uvBottomLeft
		{
			get
			{
				return (!this.flipped) ? this.uvBottomLeftUnFlipped : this.uvBottomLeftUnFlipped;
			}
		}
		public Vector2 uvBottomRight
		{
			get
			{
				return (!this.flipped) ? this.uvBottomRightUnFlipped : this.uvTopLeftUnFlipped;
			}
		}
		public Vector2 uvTopRight
		{
			get
			{
				return (!this.flipped) ? this.uvTopRightUnFlipped : this.uvTopRightUnFlipped;
			}
		}
		public Vector2 uvTopLeft
		{
			get
			{
				return (!this.flipped) ? this.uvTopLeftUnFlipped : this.uvBottomRightUnFlipped;
			}
		}
	}
}
