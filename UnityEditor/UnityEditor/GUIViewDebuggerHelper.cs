using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal class GUIViewDebuggerHelper
	{
		internal static void GetViews(List<GUIView> views)
		{
			GUIViewDebuggerHelper.GetViewsInternal(views);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetViewsInternal(object views);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DebugWindow(GUIView view);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetInstructionCount();

		public static Rect GetRectFromInstruction(int instructionIndex)
		{
			Rect result;
			GUIViewDebuggerHelper.INTERNAL_CALL_GetRectFromInstruction(instructionIndex, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetRectFromInstruction(int instructionIndex, out Rect value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GUIStyle GetStyleFromInstruction(int instructionIndex);

		internal static GUIContent GetContentFromInstruction(int instructionIndex)
		{
			return new GUIContent
			{
				text = GUIViewDebuggerHelper.GetContentTextFromInstruction(instructionIndex),
				image = GUIViewDebuggerHelper.GetContentImageFromInstruction(instructionIndex)
			};
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetContentTextFromInstruction(int instructionIndex);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture GetContentImageFromInstruction(int instructionIndex);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackFrame[] GetManagedStackTrace(int instructionIndex);
	}
}
