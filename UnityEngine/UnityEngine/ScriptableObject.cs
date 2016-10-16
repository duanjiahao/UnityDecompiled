using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class ScriptableObject : Object
	{
		public ScriptableObject()
		{
			ScriptableObject.Internal_CreateScriptableObject(this);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateScriptableObject([Writable] ScriptableObject self);

		[Obsolete("Use EditorUtility.SetDirty instead")]
		public void SetDirty()
		{
			ScriptableObject.INTERNAL_CALL_SetDirty(this);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetDirty(ScriptableObject self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ScriptableObject CreateInstance(string className);

		public static ScriptableObject CreateInstance(Type type)
		{
			return ScriptableObject.CreateInstanceFromType(type);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ScriptableObject CreateInstanceFromType(Type type);

		public static T CreateInstance<T>() where T : ScriptableObject
		{
			return (T)((object)ScriptableObject.CreateInstance(typeof(T)));
		}
	}
}
