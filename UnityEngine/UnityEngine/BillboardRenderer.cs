using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class BillboardRenderer : Renderer
	{
		public extern BillboardAsset billboard
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
