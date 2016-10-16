using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class BillboardRenderer : Renderer
	{
		public extern BillboardAsset billboard
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
