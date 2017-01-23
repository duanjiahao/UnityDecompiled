using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class MonoImporter : AssetImporter
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDefaultReferences(string[] name, UnityEngine.Object[] target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MonoScript[] GetAllRuntimeMonoScripts();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetExecutionOrder(MonoScript script, int order);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CopyMonoScriptIconToImporters(MonoScript script);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetExecutionOrder(MonoScript script);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MonoScript GetScript();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern UnityEngine.Object GetDefaultReference(string name);
	}
}
