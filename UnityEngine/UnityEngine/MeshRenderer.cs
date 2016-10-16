using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class MeshRenderer : Renderer
	{
		public extern Mesh additionalVertexStreams
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
