using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class ComponentUtility
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveComponentUp(Component component);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveComponentDown(Component component);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyComponent(Component component);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentValues(Component component);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentAsNew(GameObject go);
	}
}
