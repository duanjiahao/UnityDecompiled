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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetViewsInternal(object views);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DebugWindow(GUIView view);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopDebugging();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetInstructionCount();

		public static Rect GetRectFromInstruction(int instructionIndex)
		{
			Rect result;
			GUIViewDebuggerHelper.INTERNAL_CALL_GetRectFromInstruction(instructionIndex, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetRectFromInstruction(int instructionIndex, out Rect value);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetContentTextFromInstruction(int instructionIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture GetContentImageFromInstruction(int instructionIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackFrame[] GetManagedStackTrace(int instructionIndex);

		internal static void GetClipInstructions(List<IMGUIClipInstruction> clipInstructions)
		{
			GUIViewDebuggerHelper.GetClipInstructionsInternal(clipInstructions);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetClipInstructionsInternal(object clipInstructions);

		internal static void GetLayoutInstructions(List<IMGUILayoutInstruction> layoutInstructions)
		{
			GUIViewDebuggerHelper.GetLayoutInstructionsInternal(layoutInstructions);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLayoutInstructionsInternal(object layoutInstructions);

		internal static void GetUnifiedInstructions(List<IMGUIInstruction> layoutInstructions)
		{
			GUIViewDebuggerHelper.GetUnifiedInstructionsInternal(layoutInstructions);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetUnifiedInstructionsInternal(object instructions);
	}
}
