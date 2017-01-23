using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class MonoScript : TextAsset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MonoScript();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Type GetClass();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MonoScript FromMonoBehaviour(MonoBehaviour behaviour);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MonoScript FromScriptableObject(ScriptableObject scriptableObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetScriptTypeWasJustCreatedFromComponentMenu();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetScriptTypeWasJustCreatedFromComponentMenu();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Init(string scriptContents, string className, string nameSpace, string assemblyName, bool isEditorScript);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetNamespace();
	}
}
