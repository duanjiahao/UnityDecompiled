using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SortingLayerEditorUtility
	{
		private static class Styles
		{
			public static GUIContent m_SortingLayerStyle = EditorGUIUtility.TextContent("Sorting Layer|Name of the Renderer's sorting layer");

			public static GUIContent m_SortingOrderStyle = EditorGUIUtility.TextContent("Order in Layer|Renderer's order within a sorting layer");
		}

		public static void RenderSortingLayerFields(SerializedProperty sortingOrder, SerializedProperty sortingLayer)
		{
			EditorGUILayout.SortingLayerField(SortingLayerEditorUtility.Styles.m_SortingLayerStyle, sortingLayer, EditorStyles.popup, EditorStyles.label);
			EditorGUILayout.PropertyField(sortingOrder, SortingLayerEditorUtility.Styles.m_SortingOrderStyle, new GUILayoutOption[0]);
		}

		public static void RenderSortingLayerFields(Rect r, SerializedProperty sortingOrder, SerializedProperty sortingLayer)
		{
			EditorGUI.SortingLayerField(r, SortingLayerEditorUtility.Styles.m_SortingLayerStyle, sortingLayer, EditorStyles.popup, EditorStyles.label);
			r.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(r, sortingOrder, SortingLayerEditorUtility.Styles.m_SortingOrderStyle);
		}
	}
}
