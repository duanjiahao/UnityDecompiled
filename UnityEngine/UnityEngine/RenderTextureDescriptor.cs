using System;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public struct RenderTextureDescriptor
	{
		private int _depthBufferBits;

		private static int[] depthFormatBits = new int[]
		{
			0,
			16,
			24
		};

		private RenderTextureCreationFlags _flags;

		public int width
		{
			get;
			set;
		}

		public int height
		{
			get;
			set;
		}

		public int msaaSamples
		{
			get;
			set;
		}

		public int volumeDepth
		{
			get;
			set;
		}

		public RenderTextureFormat colorFormat
		{
			get;
			set;
		}

		public int depthBufferBits
		{
			get
			{
				return RenderTextureDescriptor.depthFormatBits[this._depthBufferBits];
			}
			set
			{
				if (value <= 0)
				{
					this._depthBufferBits = 0;
				}
				else if (value <= 16)
				{
					this._depthBufferBits = 1;
				}
				else
				{
					this._depthBufferBits = 2;
				}
			}
		}

		public TextureDimension dimension
		{
			get;
			set;
		}

		public ShadowSamplingMode shadowSamplingMode
		{
			get;
			set;
		}

		public VRTextureUsage vrUsage
		{
			get;
			set;
		}

		public RenderTextureCreationFlags flags
		{
			get
			{
				return this._flags;
			}
		}

		public RenderTextureMemoryless memoryless
		{
			get;
			set;
		}

		public bool sRGB
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.SRGB) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.SRGB);
			}
		}

		public bool useMipMap
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.MipMap) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.MipMap);
			}
		}

		public bool autoGenerateMips
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.AutoGenerateMips) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.AutoGenerateMips);
			}
		}

		public bool enableRandomWrite
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.EnableRandomWrite) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.EnableRandomWrite);
			}
		}

		internal bool createdFromScript
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.CreatedFromScript) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.CreatedFromScript);
			}
		}

		public RenderTextureDescriptor(int width, int height)
		{
			this = new RenderTextureDescriptor(width, height, RenderTextureFormat.Default, 0);
		}

		public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat)
		{
			this = new RenderTextureDescriptor(width, height, colorFormat, 0);
		}

		public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat, int depthBufferBits)
		{
			this = default(RenderTextureDescriptor);
			this.width = width;
			this.height = height;
			this.volumeDepth = 1;
			this.msaaSamples = 1;
			this.colorFormat = colorFormat;
			this.depthBufferBits = depthBufferBits;
			this.dimension = TextureDimension.Tex2D;
			this.shadowSamplingMode = ShadowSamplingMode.None;
			this.vrUsage = VRTextureUsage.None;
			this._flags = (RenderTextureCreationFlags.AutoGenerateMips | RenderTextureCreationFlags.AllowVerticalFlip);
			this.memoryless = RenderTextureMemoryless.None;
		}

		private void SetOrClearRenderTextureCreationFlag(bool value, RenderTextureCreationFlags flag)
		{
			if (value)
			{
				this._flags |= flag;
			}
			else
			{
				this._flags &= ~flag;
			}
		}
	}
}
