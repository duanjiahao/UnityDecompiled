using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class ClothRenderer : Renderer
	{
		public extern bool pauseWhenNotVisible
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
