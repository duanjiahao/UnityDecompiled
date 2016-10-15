using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class TerrainCollider : Collider
	{
		public extern TerrainData terrainData
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
