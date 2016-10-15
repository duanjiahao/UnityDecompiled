using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal sealed class RuntimeUndo
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetTransformParent(Transform transform, Transform newParent, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RecordObject(Object objectToUndo, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RecordObjects(Object[] objectsToUndo, string name);
	}
}
