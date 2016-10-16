using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	public sealed class RegistryUtil
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetRegistryUInt32Value32(string subKey, string valueName, uint defaultValue);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetRegistryStringValue32(string subKey, string valueName, string defaultValue);
	}
}
