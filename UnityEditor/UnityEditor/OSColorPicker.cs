using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class OSColorPicker
	{
		public static extern bool visible
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Color color
		{
			get
			{
				Color result;
				OSColorPicker.INTERNAL_get_color(out result);
				return result;
			}
			set
			{
				OSColorPicker.INTERNAL_set_color(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Show(bool showAlpha);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Close();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_color(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_color(ref Color value);
	}
}
