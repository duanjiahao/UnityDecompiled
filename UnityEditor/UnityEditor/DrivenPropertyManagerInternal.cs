using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class DrivenPropertyManagerInternal
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDriven(UnityEngine.Object target, string propertyPath);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDriving(UnityEngine.Object driver, UnityEngine.Object target, string propertyPath);
	}
}
