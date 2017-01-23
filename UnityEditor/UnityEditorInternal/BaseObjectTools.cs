using System;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	internal sealed class BaseObjectTools
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ClassIDToString(int ID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string NamespaceFromClassID(int ID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StringToClassID(string classString);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StringToClassIDCaseInsensitive(string classString);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBaseObject(int ID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDerivedFromClassID(int classID, int derivedFromClassID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetSuperClassID(int ID);
	}
}
