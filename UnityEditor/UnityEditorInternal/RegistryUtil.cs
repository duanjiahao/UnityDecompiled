using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	public sealed class RegistryUtil
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetRegistryUInt32Value(string subKey, string valueName, uint defaultValue, RegistryView view);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetRegistryStringValue(string subKey, string valueName, string defaultValue, RegistryView view);
	}
}
