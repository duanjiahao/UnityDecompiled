using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class DefaultAsset : UnityEngine.Object
	{
		internal extern string message
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool isWarning
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
