using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	internal sealed class ModuleMetadata
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetModuleNames();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetModuleStrippable(string moduleName);

		public static UnityType[] GetModuleTypes(string moduleName)
		{
			int[] moduleClasses = ModuleMetadata.GetModuleClasses(moduleName);
			return (from id in moduleClasses
			select UnityType.FindTypeByPersistentTypeID(id)).ToArray<UnityType>();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int[] GetModuleClasses(string moduleName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetICallModule(string icall);
	}
}
