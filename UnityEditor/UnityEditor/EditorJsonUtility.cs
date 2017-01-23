using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public static class EditorJsonUtility
	{
		public static string ToJson(object obj)
		{
			return EditorJsonUtility.ToJson(obj, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ToJson(object obj, bool prettyPrint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FromJsonOverwrite(string json, object objectToOverwrite);
	}
}
