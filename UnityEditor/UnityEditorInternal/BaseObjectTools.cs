using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	internal sealed class BaseObjectTools
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ClassIDToString(int ID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StringToClassID(string classString);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StringToClassIDCaseInsensitive(string classString);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBaseObject(int ID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDerivedFromClassID(int classID, int derivedFromClassID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetSuperClassID(int ID);
	}
}
