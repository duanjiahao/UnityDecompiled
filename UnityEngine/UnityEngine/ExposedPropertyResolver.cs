using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public struct ExposedPropertyResolver
	{
		internal IntPtr table;

		internal static Object ResolveReferenceInternal(IntPtr ptr, PropertyName name, out bool isValid)
		{
			return ExposedPropertyResolver.INTERNAL_CALL_ResolveReferenceInternal(ptr, ref name, out isValid);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object INTERNAL_CALL_ResolveReferenceInternal(IntPtr ptr, ref PropertyName name, out bool isValid);
	}
}
