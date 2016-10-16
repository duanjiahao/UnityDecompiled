using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	[Obsolete("This component is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
	public sealed class ParticleRenderer : Renderer
	{
		public extern ParticleRenderMode particleRenderMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float lengthScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float velocityScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float cameraVelocityScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxParticleSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int uvAnimationXTile
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int uvAnimationYTile
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float uvAnimationCycles
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("animatedTextureCount has been replaced by uvAnimationXTile and uvAnimationYTile.")]
		public int animatedTextureCount
		{
			get
			{
				return this.uvAnimationXTile;
			}
			set
			{
				this.uvAnimationXTile = value;
			}
		}

		public float maxPartileSize
		{
			get
			{
				return this.maxParticleSize;
			}
			set
			{
				this.maxParticleSize = value;
			}
		}

		public extern Rect[] uvTiles
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("This function has been removed.", true)]
		public AnimationCurve widthCurve
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("This function has been removed.", true)]
		public AnimationCurve heightCurve
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("This function has been removed.", true)]
		public AnimationCurve rotationCurve
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
	}
}
