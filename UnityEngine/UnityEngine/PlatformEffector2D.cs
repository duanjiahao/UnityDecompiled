using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class PlatformEffector2D : Effector2D
	{
		public extern bool useOneWay
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useOneWayGrouping
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useSideFriction
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useSideBounce
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float surfaceArc
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float sideArc
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("PlatformEffector2D.oneWay has been deprecated. Use PlatformEffector2D.useOneWay instead (UnityUpgradable) -> useOneWay", true)]
		public bool oneWay
		{
			get
			{
				return this.useOneWay;
			}
			set
			{
				this.useOneWay = value;
			}
		}

		[Obsolete("PlatformEffector2D.sideFriction has been deprecated. Use PlatformEffector2D.useSideFriction instead (UnityUpgradable) -> useSideFriction", true)]
		public bool sideFriction
		{
			get
			{
				return this.useSideFriction;
			}
			set
			{
				this.useSideFriction = value;
			}
		}

		[Obsolete("PlatformEffector2D.sideBounce has been deprecated. Use PlatformEffector2D.useSideBounce instead (UnityUpgradable) -> useSideBounce", true)]
		public bool sideBounce
		{
			get
			{
				return this.useSideBounce;
			}
			set
			{
				this.useSideBounce = value;
			}
		}

		[Obsolete("PlatformEffector2D.sideAngleVariance has been deprecated. Use PlatformEffector2D.sideArc instead (UnityUpgradable) -> sideArc", true)]
		public float sideAngleVariance
		{
			get
			{
				return this.sideArc;
			}
			set
			{
				this.sideArc = value;
			}
		}
	}
}
