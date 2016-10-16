using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	internal sealed class ModuleMetadata
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetModuleNames();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetModuleStrippable(string moduleName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int[] GetModuleClasses(string moduleName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetICallModule(string icall);
	}
}
