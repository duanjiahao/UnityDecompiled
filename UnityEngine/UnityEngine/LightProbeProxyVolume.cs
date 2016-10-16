using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LightProbeProxyVolume : Behaviour
	{
		public enum ResolutionMode
		{
			Automatic,
			Custom
		}

		public enum BoundingBoxMode
		{
			AutomaticLocal,
			AutomaticWorld,
			Custom
		}

		public enum ProbePositionMode
		{
			CellCorner,
			CellCenter
		}

		public enum RefreshMode
		{
			Automatic,
			EveryFrame,
			ViaScripting
		}

		public Bounds boundsGlobal
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_boundsGlobal(out result);
				return result;
			}
		}

		public Vector3 sizeCustom
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_sizeCustom(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_sizeCustom(ref value);
			}
		}

		public Vector3 originCustom
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_originCustom(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_originCustom(ref value);
			}
		}

		public extern LightProbeProxyVolume.BoundingBoxMode boundingBoxMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern LightProbeProxyVolume.ResolutionMode resolutionMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern LightProbeProxyVolume.ProbePositionMode probePositionMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern LightProbeProxyVolume.RefreshMode refreshMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float probeDensity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int gridResolutionX
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int gridResolutionY
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int gridResolutionZ
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isFeatureSupported
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_boundsGlobal(out Bounds value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_sizeCustom(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_sizeCustom(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_originCustom(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_originCustom(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update();
	}
}
