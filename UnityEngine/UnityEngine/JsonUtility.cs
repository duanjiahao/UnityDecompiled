using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class JsonUtility
	{
		public static string ToJson(object obj)
		{
			return JsonUtility.ToJson(obj, false);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ToJson(object obj, bool prettyPrint);

		public static T FromJson<T>(string json)
		{
			return (T)((object)JsonUtility.FromJson(json, typeof(T)));
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object FromJson(string json, Type type);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FromJsonOverwrite(string json, object objectToOverwrite);
	}
}
