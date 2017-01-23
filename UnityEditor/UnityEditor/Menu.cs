using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public sealed class Menu
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetChecked(string menuPath, bool isChecked);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetChecked(string menuPath);
	}
}
