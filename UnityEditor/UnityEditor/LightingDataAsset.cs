using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class LightingDataAsset : UnityEngine.Object
	{
		internal extern bool isValid
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern string validityErrorMessage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
