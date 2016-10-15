using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class MonoScript : TextAsset
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MonoScript();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Type GetClass();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MonoScript FromMonoBehaviour(MonoBehaviour behaviour);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MonoScript FromScriptableObject(ScriptableObject scriptableObject);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetScriptTypeWasJustCreatedFromComponentMenu();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetScriptTypeWasJustCreatedFromComponentMenu();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Init(string scriptContents, string className, string nameSpace, string assemblyName, bool isEditorScript);
	}
}
