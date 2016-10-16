using System;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class EditorGUILayoutUtilityInternal : GUILayoutUtility
	{
		internal new static GUILayoutGroup topLevel
		{
			get
			{
				return GUILayoutUtility.topLevel;
			}
		}

		internal new static GUILayoutGroup BeginLayoutArea(GUIStyle style, Type LayoutType)
		{
			return GUILayoutUtility.DoBeginLayoutArea(style, LayoutType);
		}
	}
}
