using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class InternalGraphUtility
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GenerateGraphName();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] AllGraphsOnGameObject(GameObject go);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] AllGraphsInScene();
	}
}
