using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public static class EditorJsonUtility
	{
		public static string ToJson(UnityEngine.Object obj)
		{
			return EditorJsonUtility.ToJson(obj, false);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ToJson(UnityEngine.Object obj, bool prettyPrint);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FromJsonOverwrite(string json, UnityEngine.Object objectToOverwrite);
	}
}
