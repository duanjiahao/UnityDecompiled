using System;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	public sealed class RegistryUtil
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetRegistryUInt32Value32(string subKey, string valueName, uint defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetRegistryStringValue32(string subKey, string valueName, string defaultValue);
	}
}
