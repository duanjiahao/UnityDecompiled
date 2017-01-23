using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal sealed class GradientPreviewCache
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearCache();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D GetPropertyPreview(SerializedProperty property);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D GetGradientPreview(Gradient curve);
	}
}
