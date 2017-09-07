using System;
using System.Runtime.InteropServices;

namespace UnityEngine.CSSLayout
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate CSSSize CSSMeasureFunc(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode);
}
