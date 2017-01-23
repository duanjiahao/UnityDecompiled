using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class TerrainCollider : Collider
	{
		public extern TerrainData terrainData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
