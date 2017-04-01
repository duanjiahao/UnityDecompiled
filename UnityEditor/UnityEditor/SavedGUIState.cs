using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal struct SavedGUIState
	{
		internal GUILayoutUtility.LayoutCache layoutCache;

		internal IntPtr guiState;

		internal Vector2 screenManagerSize;

		internal Rect renderManagerRect;

		internal GUISkin skin;

		internal int instanceID;

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetupSavedGUIState(out IntPtr state, out Vector2 screenManagerSize);

		private static void Internal_ApplySavedGUIState(IntPtr state, Vector2 screenManagerSize)
		{
			SavedGUIState.INTERNAL_CALL_Internal_ApplySavedGUIState(state, ref screenManagerSize);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_ApplySavedGUIState(IntPtr state, ref Vector2 screenManagerSize);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetGUIDepth();

		internal static SavedGUIState Create()
		{
			SavedGUIState result = default(SavedGUIState);
			if (SavedGUIState.Internal_GetGUIDepth() > 0)
			{
				result.skin = GUI.skin;
				result.layoutCache = new GUILayoutUtility.LayoutCache(GUILayoutUtility.current);
				result.instanceID = GUIUtility.s_OriginalID;
				SavedGUIState.Internal_SetupSavedGUIState(out result.guiState, out result.screenManagerSize);
			}
			return result;
		}

		internal void ApplyAndForget()
		{
			if (this.layoutCache != null)
			{
				GUILayoutUtility.current = this.layoutCache;
				GUI.skin = this.skin;
				GUIUtility.s_OriginalID = this.instanceID;
				SavedGUIState.Internal_ApplySavedGUIState(this.guiState, this.screenManagerSize);
				GUIClip.Reapply();
			}
		}
	}
}
