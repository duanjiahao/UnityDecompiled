using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public class GUIUtility
	{
		internal static int s_SkinMode;

		internal static int s_OriginalID;

		internal static Vector2 s_EditorScreenPointOffset = Vector2.zero;

		internal static bool s_HasKeyboardFocus = false;

		internal static float pixelsPerPoint
		{
			get
			{
				return GUIUtility.Internal_GetPixelsPerPoint();
			}
		}

		internal static bool guiIsExiting
		{
			get;
			set;
		}

		public static int hotControl
		{
			get
			{
				return GUIUtility.Internal_GetHotControl();
			}
			set
			{
				GUIUtility.Internal_SetHotControl(value);
			}
		}

		public static extern int keyboardControl
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string systemCopyBuffer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool mouseUsed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool hasModalWindow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool textFieldInput
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static int GetControlID(FocusType focus)
		{
			return GUIUtility.GetControlID(0, focus);
		}

		public static int GetControlID(GUIContent contents, FocusType focus)
		{
			return GUIUtility.GetControlID(contents.hash, focus);
		}

		public static int GetControlID(FocusType focus, Rect position)
		{
			return GUIUtility.Internal_GetNextControlID2(0, focus, position);
		}

		public static int GetControlID(int hint, FocusType focus, Rect position)
		{
			return GUIUtility.Internal_GetNextControlID2(hint, focus, position);
		}

		public static int GetControlID(GUIContent contents, FocusType focus, Rect position)
		{
			return GUIUtility.Internal_GetNextControlID2(contents.hash, focus, position);
		}

		public static object GetStateObject(Type t, int controlID)
		{
			return GUIStateObjects.GetStateObject(t, controlID);
		}

		public static object QueryStateObject(Type t, int controlID)
		{
			return GUIStateObjects.QueryStateObject(t, controlID);
		}

		public static void ExitGUI()
		{
			GUIUtility.guiIsExiting = true;
			throw new ExitGUIException();
		}

		internal static GUISkin GetDefaultSkin()
		{
			return GUIUtility.Internal_GetDefaultSkin(GUIUtility.s_SkinMode);
		}

		internal static GUISkin GetBuiltinSkin(int skin)
		{
			return GUIUtility.Internal_GetBuiltinSkin(skin) as GUISkin;
		}

		internal static void CleanupRoots()
		{
		}

		[RequiredByNativeCode]
		internal static void BeginGUI(int skinMode, int instanceID, int useGUILayout)
		{
			GUIUtility.s_SkinMode = skinMode;
			GUIUtility.s_OriginalID = instanceID;
			GUI.skin = null;
			GUIUtility.guiIsExiting = false;
			if (useGUILayout != 0)
			{
				GUILayoutUtility.SelectIDList(instanceID, false);
				GUILayoutUtility.Begin(instanceID);
			}
			GUI.changed = false;
		}

		[RequiredByNativeCode]
		internal static void SetSkin(int skinMode)
		{
			GUIUtility.s_SkinMode = skinMode;
			GUI.DoSetSkin(null);
		}

		[RequiredByNativeCode]
		internal static void EndGUI(int layoutType)
		{
			try
			{
				if (Event.current.type == EventType.Layout)
				{
					if (layoutType != 0)
					{
						if (layoutType != 1)
						{
							if (layoutType == 2)
							{
								GUILayoutUtility.LayoutFromEditorWindow();
							}
						}
						else
						{
							GUILayoutUtility.Layout();
						}
					}
				}
				GUILayoutUtility.SelectIDList(GUIUtility.s_OriginalID, false);
				GUIContent.ClearStaticCache();
			}
			finally
			{
				GUIUtility.Internal_ExitGUI();
			}
		}

		[RequiredByNativeCode]
		internal static bool EndGUIFromException(Exception exception)
		{
			bool result;
			if (!GUIUtility.ShouldRethrowException(exception))
			{
				result = false;
			}
			else
			{
				GUIUtility.Internal_ExitGUI();
				result = true;
			}
			return result;
		}

		internal static bool ShouldRethrowException(Exception exception)
		{
			while (exception is TargetInvocationException && exception.InnerException != null)
			{
				exception = exception.InnerException;
			}
			return exception is ExitGUIException;
		}

		internal static void CheckOnGUI()
		{
			if (GUIUtility.Internal_GetGUIDepth() <= 0)
			{
				throw new ArgumentException("You can only call GUI functions from inside OnGUI.");
			}
		}

		public static Vector2 GUIToScreenPoint(Vector2 guiPoint)
		{
			return GUIClip.Unclip(guiPoint) + GUIUtility.s_EditorScreenPointOffset;
		}

		internal static Rect GUIToScreenRect(Rect guiRect)
		{
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
			guiRect.x = vector.x;
			guiRect.y = vector.y;
			return guiRect;
		}

		public static Vector2 ScreenToGUIPoint(Vector2 screenPoint)
		{
			return GUIClip.Clip(screenPoint) - GUIUtility.s_EditorScreenPointOffset;
		}

		public static Rect ScreenToGUIRect(Rect screenRect)
		{
			Vector2 vector = GUIUtility.ScreenToGUIPoint(new Vector2(screenRect.x, screenRect.y));
			screenRect.x = vector.x;
			screenRect.y = vector.y;
			return screenRect;
		}

		public static void RotateAroundPivot(float angle, Vector2 pivotPoint)
		{
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 lhs = Matrix4x4.TRS(vector, Quaternion.Euler(0f, 0f, angle), Vector3.one) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = lhs * matrix;
		}

		public static void ScaleAroundPivot(Vector2 scale, Vector2 pivotPoint)
		{
			Matrix4x4 matrix = GUI.matrix;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 lhs = Matrix4x4.TRS(vector, Quaternion.identity, new Vector3(scale.x, scale.y, 1f)) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = lhs * matrix;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float Internal_GetPixelsPerPoint();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetControlID(int hint, FocusType focus);

		private static int Internal_GetNextControlID2(int hint, FocusType focusType, Rect rect)
		{
			return GUIUtility.INTERNAL_CALL_Internal_GetNextControlID2(hint, focusType, ref rect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_Internal_GetNextControlID2(int hint, FocusType focusType, ref Rect rect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPermanentControlID();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetHotControl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHotControl(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateUndoName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GrabMouseControl(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReleaseMouseControl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasMouseControl(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetChanged();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetChanged(bool changed);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDidGUIWindowsEatLastEvent(bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GUISkin Internal_GetDefaultSkin(int skinMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_GetBuiltinSkin(int skin);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ExitGUI();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetGUIDepth();
	}
}
