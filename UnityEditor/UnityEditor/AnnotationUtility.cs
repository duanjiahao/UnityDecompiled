using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	internal sealed class AnnotationUtility
	{
		internal static extern bool use3dGizmos
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool showGrid
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool showSelectionOutline
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool showSelectionWire
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern float iconSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Annotation[] GetAnnotations();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Annotation[] GetRecentlyChangedAnnotations();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetNameOfCurrentSetup();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGizmoEnabled(int classID, string scriptClass, int gizmoEnabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIconEnabled(int classID, string scriptClass, int iconEnabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetPresetList();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LoadPreset(string presetName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SavePreset(string presetName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DeletePreset(string presetName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ResetToFactorySettings();
	}
}
