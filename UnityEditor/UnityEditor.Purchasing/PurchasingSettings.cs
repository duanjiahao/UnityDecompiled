using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Purchasing
{
	public static class PurchasingSettings
	{
		[ThreadAndSerializationSafe]
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
